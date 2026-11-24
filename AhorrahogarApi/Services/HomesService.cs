using System.Security.Cryptography;
using AhorrahogarApi.Data;
using AhorrahogarApi.DTOs;
using AhorrahogarApi.Models;
using Microsoft.EntityFrameworkCore;

namespace AhorrahogarApi.Services
{
    public class HomesService : IHogarService
    {
        // Sin 0/O ni 1/I para que el código no se confunda al leerlo o dictarlo
        private const string Alfabeto = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
        private const int LargoCodigo = 6;

        private readonly AppDbContext _context;
        private readonly IAuthService _auth;

        public HomesService(AppDbContext context, IAuthService auth)
        {
            _context = context;
            _auth = auth;
        }

        private static HogarResultado<T> Error<T>(int status, string mensaje) => new(status, mensaje);
        private static HogarResultado<T> Exito<T>(T datos, int status = 200) => new(status, null, datos);

        // ---------- Crear un hogar ----------
        public async Task<HogarResultado<MeDto>> CrearAsync(int usuarioId, string nombre)
        {
            var usuario = await _context.integrantes.FindAsync(usuarioId);
            if (usuario is null) return Error<MeDto>(401, "Sesión no válida.");
            if (usuario.Hogar_Id.HasValue) return Error<MeDto>(409, "Ya perteneces a un hogar.");

            // El hogar se guarda primero para tener su id; todo va en una transacción
            await using var tx = await _context.Database.BeginTransactionAsync();

            var hogar = new Homes
            {
                Nombre = nombre.Trim(),
                Creado_En = DateTime.Now,
                Codigo_Invitacion = await GenerarCodigoUnicoAsync()
            };
            _context.hogares.Add(hogar);
            await _context.SaveChangesAsync();

            usuario.Hogar_Id = hogar.Id;
            usuario.Rol = "admin";

            // Si había pedido unirse a otro hogar, esa solicitud se cancela
            var pendientes = await _context.solicitudes_hogar
                .Where(s => s.Integrante_Id == usuarioId && s.Estado == "pendiente")
                .ToListAsync();
            foreach (var s in pendientes)
            {
                s.Estado = "cancelada";
                s.Resuelto_En = DateTime.Now;
            }

            await _context.SaveChangesAsync();
            await tx.CommitAsync();

            var me = await _auth.GetMeAsync(usuarioId);
            return Exito(me!, 201);
        }

        // ---------- Ver mi hogar ----------
        public async Task<HogarResultado<HogarDto>> GetMiHogarAsync(int usuarioId)
        {
            var usuario = await _context.integrantes.AsNoTracking().FirstOrDefaultAsync(m => m.Id == usuarioId);
            if (usuario?.Hogar_Id is not int hogarId)
                return Error<HogarDto>(404, "Todavía no perteneces a un hogar.");

            var hogar = await _context.hogares.AsNoTracking().FirstOrDefaultAsync(h => h.Id == hogarId);
            if (hogar is null) return Error<HogarDto>(404, "No encontramos tu hogar.");

            var miembros = await _context.integrantes
                .AsNoTracking()
                .Where(m => m.Hogar_Id == hogarId)
                .OrderBy(m => m.Creado_En)
                .Select(m => new MiembroDto
                {
                    id = m.Id,
                    nombre = m.nombre,
                    email = m.Email ?? string.Empty,
                    rol = m.Rol
                })
                .ToListAsync();

            return Exito(new HogarDto
            {
                id = hogar.Id,
                nombre = hogar.Nombre,
                // Solo el administrador ve el código
                codigo_invitacion = usuario.Rol == "admin" ? hogar.Codigo_Invitacion : null,
                miembros = miembros
            });
        }

        // ---------- Pedir unirse con un código ----------
        public async Task<HogarResultado<MeDto>> UnirseAsync(int usuarioId, string codigo)
        {
            var usuario = await _context.integrantes.AsNoTracking().FirstOrDefaultAsync(m => m.Id == usuarioId);
            if (usuario is null) return Error<MeDto>(401, "Sesión no válida.");
            if (usuario.Hogar_Id.HasValue) return Error<MeDto>(409, "Ya perteneces a un hogar.");

            var yaPidio = await _context.solicitudes_hogar
                .AnyAsync(s => s.Integrante_Id == usuarioId && s.Estado == "pendiente");
            if (yaPidio)
                return Error<MeDto>(409, "Ya tienes una solicitud pendiente. Cancélala si quieres pedir otra.");

            var codigoLimpio = codigo.Trim().ToUpperInvariant();
            var hogar = await _context.hogares.AsNoTracking()
                .FirstOrDefaultAsync(h => h.Codigo_Invitacion == codigoLimpio);
            if (hogar is null) return Error<MeDto>(404, "Ese código no es válido.");

            _context.solicitudes_hogar.Add(new Solicitud
            {
                Hogar_Id = hogar.Id,
                Integrante_Id = usuarioId,
                Estado = "pendiente",
                Creado_En = DateTime.Now
            });
            await _context.SaveChangesAsync();

            var me = await _auth.GetMeAsync(usuarioId);
            return Exito(me!, 201);
        }

        public async Task<HogarResultado<MeDto>> CancelarSolicitudAsync(int usuarioId)
        {
            var pendientes = await _context.solicitudes_hogar
                .Where(s => s.Integrante_Id == usuarioId && s.Estado == "pendiente")
                .ToListAsync();

            foreach (var s in pendientes)
            {
                s.Estado = "cancelada";
                s.Resuelto_En = DateTime.Now;
            }
            await _context.SaveChangesAsync();

            var me = await _auth.GetMeAsync(usuarioId);
            if (me is null) return Error<MeDto>(401, "Sesión no válida.");
            return Exito(me);
        }

        // ---------- Solo el administrador ----------
        public async Task<HogarResultado<List<SolicitudDto>>> GetSolicitudesAsync(int usuarioId)
        {
            var admin = await ObtenerAdminAsync(usuarioId);
            if (admin is null) return Error<List<SolicitudDto>>(403, "Solo el administrador del hogar puede hacer esto.");

            var hogarId = admin.Hogar_Id!.Value;

            var lista = await (
                from s in _context.solicitudes_hogar.AsNoTracking()
                join m in _context.integrantes.AsNoTracking() on s.Integrante_Id equals m.Id
                where s.Hogar_Id == hogarId && s.Estado == "pendiente"
                orderby s.Creado_En
                select new SolicitudDto
                {
                    id = s.Id,
                    integrante_id = m.Id,
                    nombre = m.nombre,
                    email = m.Email ?? string.Empty,
                    creado_en = s.Creado_En
                }
            ).ToListAsync();

            return Exito(lista);
        }

        public async Task<HogarResultado<bool>> ResolverSolicitudAsync(int usuarioId, int solicitudId, bool aceptar)
        {
            var admin = await ObtenerAdminAsync(usuarioId);
            if (admin is null) return Error<bool>(403, "Solo el administrador del hogar puede hacer esto.");

            var hogarId = admin.Hogar_Id!.Value;

            // Solo se pueden resolver solicitudes pendientes de MI hogar
            var solicitud = await _context.solicitudes_hogar
                .FirstOrDefaultAsync(s => s.Id == solicitudId && s.Hogar_Id == hogarId && s.Estado == "pendiente");
            if (solicitud is null) return Error<bool>(404, "No encontramos esa solicitud.");

            solicitud.Resuelto_En = DateTime.Now;

            if (!aceptar)
            {
                solicitud.Estado = "rechazada";
                await _context.SaveChangesAsync();
                return Exito(true);
            }

            var solicitante = await _context.integrantes.FindAsync(solicitud.Integrante_Id);
            if (solicitante is null || solicitante.Hogar_Id.HasValue)
            {
                solicitud.Estado = "rechazada";
                await _context.SaveChangesAsync();
                return Error<bool>(409, "Esa persona ya no puede unirse porque ya pertenece a un hogar.");
            }

            solicitante.Hogar_Id = hogarId;
            solicitante.Rol = "miembro";
            solicitud.Estado = "aceptada";

            // Cualquier otra solicitud pendiente de esa persona queda sin efecto
            var otras = await _context.solicitudes_hogar
                .Where(s => s.Integrante_Id == solicitante.Id && s.Id != solicitud.Id && s.Estado == "pendiente")
                .ToListAsync();
            foreach (var otra in otras)
            {
                otra.Estado = "cancelada";
                otra.Resuelto_En = DateTime.Now;
            }

            // Un solo SaveChanges: o se guarda todo o no se guarda nada
            await _context.SaveChangesAsync();
            return Exito(true);
        }

        public async Task<HogarResultado<CodigoDto>> RegenerarCodigoAsync(int usuarioId)
        {
            var admin = await ObtenerAdminAsync(usuarioId);
            if (admin is null) return Error<CodigoDto>(403, "Solo el administrador del hogar puede hacer esto.");

            var hogar = await _context.hogares.FindAsync(admin.Hogar_Id!.Value);
            if (hogar is null) return Error<CodigoDto>(404, "No encontramos tu hogar.");

            hogar.Codigo_Invitacion = await GenerarCodigoUnicoAsync();
            await _context.SaveChangesAsync();

            return Exito(new CodigoDto { codigo_invitacion = hogar.Codigo_Invitacion });
        }

        public async Task<HogarResultado<bool>> RemoverMiembroAsync(int usuarioId, int miembroId)
        {
            var admin = await ObtenerAdminAsync(usuarioId);
            if (admin is null) return Error<bool>(403, "Solo el administrador del hogar puede hacer esto.");

            if (miembroId == admin.Id)
                return Error<bool>(400, "No puedes quitarte a ti mismo del hogar.");

            var miembro = await _context.integrantes.FindAsync(miembroId);
            if (miembro is null || miembro.Hogar_Id != admin.Hogar_Id)
                return Error<bool>(404, "No encontramos a esa persona en tu hogar.");

            if (miembro.Rol == "admin")
                return Error<bool>(403, "No se puede quitar a otro administrador.");

            miembro.Hogar_Id = null;
            miembro.Rol = "miembro";
            await _context.SaveChangesAsync();

            return Exito(true);
        }

        // ---------- Salir del hogar ----------
        public async Task<HogarResultado<MeDto>> SalirAsync(int usuarioId)
        {
            var usuario = await _context.integrantes.FindAsync(usuarioId);
            if (usuario is null) return Error<MeDto>(401, "Sesión no válida.");
            if (!usuario.Hogar_Id.HasValue) return Error<MeDto>(409, "No perteneces a ningún hogar.");

            if (usuario.Rol == "admin")
                return Error<MeDto>(409, "El administrador no puede salir del hogar.");

            usuario.Hogar_Id = null;
            usuario.Rol = "miembro";
            await _context.SaveChangesAsync();

            var me = await _auth.GetMeAsync(usuarioId);
            return Exito(me!);
        }

        // ---------- Utilidades ----------

        // El usuario, solo si es administrador de un hogar (con seguimiento para poder modificarlo)
        private async Task<Members?> ObtenerAdminAsync(int usuarioId)
        {
            var usuario = await _context.integrantes.FindAsync(usuarioId);
            return usuario is not null && usuario.Rol == "admin" && usuario.Hogar_Id.HasValue ? usuario : null;
        }

        private async Task<string> GenerarCodigoUnicoAsync()
        {
            for (var intento = 0; intento < 10; intento++)
            {
                var codigo = new string(
                    Enumerable.Range(0, LargoCodigo)
                        .Select(_ => Alfabeto[RandomNumberGenerator.GetInt32(Alfabeto.Length)])
                        .ToArray());

                if (!await _context.hogares.AnyAsync(h => h.Codigo_Invitacion == codigo))
                    return codigo;
            }

            throw new InvalidOperationException("No se pudo generar un código de invitación único.");
        }
    }
}
