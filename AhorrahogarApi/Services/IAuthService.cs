using AhorrahogarApi.DTOs;

namespace AhorrahogarApi.Services
{
    public enum CambioPassword { Ok, UsuarioNoExiste, ActualIncorrecta, IgualALaActual }

    public interface IAuthService
    {
        // null si el correo ya está registrado
        Task<AuthResponseDto?> RegisterAsync(RegisterDto dto);

        // null si el correo o la contraseña no son correctos
        Task<AuthResponseDto?> LoginAsync(LoginDto dto);

        Task<MeDto?> GetMeAsync(int usuarioId);

        Task<CambioPassword> CambiarPasswordAsync(int usuarioId, CambiarPasswordDto dto);
    }
}
