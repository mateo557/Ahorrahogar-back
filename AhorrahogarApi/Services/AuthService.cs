using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AhorrahogarApi.Data;
using AhorrahogarApi.DTOs;
using AhorrahogarApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace AhorrahogarApi.Services
{
    public class AuthService : IAuthService
    {
        // Se compara contra este hash cuando el correo no existe, para que
        // responder tarde lo mismo y no se pueda saber qué correos están registrados
        private static readonly string HashFalso = BCrypt.Net.BCrypt.HashPassword("contraseña-que-nadie-usa");

        private readonly AppDbContext _context;
        private readonly IConfiguration _config;

        public AuthService(AppDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        public async Task<AuthResponseDto?> RegisterAsync(RegisterDto dto)
        {
            var email = dto.email.Trim().ToLowerInvariant();

            if (await _context.integrantes.AnyAsync(m => m.Email == email))
                return null;

            var usuario = new Members
            {
                nombre = dto.nombre.Trim(),
                Email = email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.password),
                Hogar_Id = null,
                Rol = "miembro",
                Creado_En = DateTime.Now
            };

            _context.integrantes.Add(usuario);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                // Dos registros a la vez con el mismo correo: gana el primero (el correo es único en la base)
                return null;
            }

            return new AuthResponseDto
            {
                token = CrearToken(usuario),
                usuario = await ArmarMeAsync(usuario)
            };
        }

        public async Task<AuthResponseDto?> LoginAsync(LoginDto dto)
        {
            var email = dto.email.Trim().ToLowerInvariant();
            var usuario = await _context.integrantes.FirstOrDefaultAsync(m => m.Email == email);

            if (usuario is null || string.IsNullOrEmpty(usuario.PasswordHash))
            {
                BCrypt.Net.BCrypt.Verify(dto.password, HashFalso);
                return null;
            }

            if (!BCrypt.Net.BCrypt.Verify(dto.password, usuario.PasswordHash))
                return null;

            return new AuthResponseDto
            {
                token = CrearToken(usuario),
                usuario = await ArmarMeAsync(usuario)
            };
        }

        public async Task<MeDto?> GetMeAsync(int usuarioId)
        {
            var usuario = await _context.integrantes.AsNoTracking().FirstOrDefaultAsync(m => m.Id == usuarioId);
            return usuario is null ? null : await ArmarMeAsync(usuario);
        }

        public async Task<CambioPassword> CambiarPasswordAsync(int usuarioId, CambiarPasswordDto dto)
        {
            var usuario = await _context.integrantes.FirstOrDefaultAsync(m => m.Id == usuarioId);
            if (usuario is null || string.IsNullOrEmpty(usuario.PasswordHash))
                return CambioPassword.UsuarioNoExiste;

            // Se pide la contraseña actual para que quien encuentre el teléfono desbloqueado no pueda cambiarla
            if (!BCrypt.Net.BCrypt.Verify(dto.password_actual, usuario.PasswordHash))
                return CambioPassword.ActualIncorrecta;

            if (dto.password_actual == dto.password_nueva)
                return CambioPassword.IgualALaActual;

            usuario.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.password_nueva);
            await _context.SaveChangesAsync();
            return CambioPassword.Ok;
        }

        // El hogar y el rol se leen de la base de datos, no del token, para que
        // siempre estén al día cuando el usuario crea un hogar o lo aceptan en uno
        private async Task<MeDto> ArmarMeAsync(Members usuario)
        {
            string? hogarNombre = null;

            if (usuario.Hogar_Id is int hogarId)
            {
                hogarNombre = await _context.hogares
                    .AsNoTracking()
                    .Where(h => h.Id == hogarId)
                    .Select(h => h.Nombre)
                    .FirstOrDefaultAsync();
            }

            string? solicitudHogar = null;

            if (usuario.Hogar_Id is null)
            {
                solicitudHogar = await (
                    from s in _context.solicitudes_hogar.AsNoTracking()
                    join h in _context.hogares.AsNoTracking() on s.Hogar_Id equals h.Id
                    where s.Integrante_Id == usuario.Id && s.Estado == "pendiente"
                    select h.Nombre
                ).FirstOrDefaultAsync();
            }

            return new MeDto
            {
                id = usuario.Id,
                nombre = usuario.nombre,
                email = usuario.Email ?? string.Empty,
                rol = usuario.Rol,
                hogar_id = usuario.Hogar_Id,
                hogar_nombre = hogarNombre,
                tiene_hogar = usuario.Hogar_Id.HasValue,
                solicitud_pendiente_hogar = solicitudHogar
            };
        }

        private string CrearToken(Members usuario)
        {
            var clave = _config["Jwt:Key"]!;
            var credenciales = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(clave)),
                SecurityAlgorithms.HmacSha256);

            // El token solo identifica al usuario; nada más
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim(ClaimTypes.Name, usuario.nombre)
            };

            var minutos = _config.GetValue<int?>("Jwt:ExpireMinutes") ?? 480;

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(minutos),
                signingCredentials: credenciales);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
