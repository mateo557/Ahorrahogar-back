using System.Security.Claims;
using AhorrahogarApi.Data;
using Microsoft.EntityFrameworkCore;

namespace AhorrahogarApi.Services
{
    // Quién está haciendo la petición y a qué hogar pertenece AHORA.
    // El hogar se lee de la base de datos (no del token) para que, si entra o sale de un hogar,
    // el cambio se note de inmediato sin tener que volver a iniciar sesión.
    public interface IUsuarioActual
    {
        int Id { get; }
        Task<int?> HogarIdAsync();
    }

    public class Users : IUsuarioActual
    {
        private readonly IHttpContextAccessor _http;
        private readonly AppDbContext _context;
        private int? _hogarId;
        private bool _cargado;

        public Users(IHttpContextAccessor http, AppDbContext context)
        {
            _http = http;
            _context = context;
        }

        public int Id =>
            int.TryParse(_http.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : 0;

        public async Task<int?> HogarIdAsync()
        {
            if (_cargado) return _hogarId;

            _hogarId = await _context.integrantes
                .AsNoTracking()
                .Where(m => m.Id == Id)
                .Select(m => m.Hogar_Id)
                .FirstOrDefaultAsync();
            _cargado = true;
            return _hogarId;
        }
    }
}
