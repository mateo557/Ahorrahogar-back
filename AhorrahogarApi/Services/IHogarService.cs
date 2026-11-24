using AhorrahogarApi.DTOs;

namespace AhorrahogarApi.Services
{
    // Resultado de una operación: un código HTTP, un mensaje si falló y los datos si salió bien
    public record HogarResultado<T>(int Status, string? Mensaje = null, T? Datos = default)
    {
        public bool Ok => Status >= 200 && Status < 300;
    }

    public interface IHogarService
    {
        Task<HogarResultado<MeDto>> CrearAsync(int usuarioId, string nombre);
        Task<HogarResultado<HogarDto>> GetMiHogarAsync(int usuarioId);
        Task<HogarResultado<MeDto>> UnirseAsync(int usuarioId, string codigo);
        Task<HogarResultado<MeDto>> CancelarSolicitudAsync(int usuarioId);
        Task<HogarResultado<List<SolicitudDto>>> GetSolicitudesAsync(int usuarioId);
        Task<HogarResultado<bool>> ResolverSolicitudAsync(int usuarioId, int solicitudId, bool aceptar);
        Task<HogarResultado<CodigoDto>> RegenerarCodigoAsync(int usuarioId);
        Task<HogarResultado<bool>> RemoverMiembroAsync(int usuarioId, int miembroId);
        Task<HogarResultado<MeDto>> SalirAsync(int usuarioId);
    }
}
