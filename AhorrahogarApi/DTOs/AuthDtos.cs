using System.ComponentModel.DataAnnotations;

namespace AhorrahogarApi.DTOs
{
    public class RegisterDto
    {
        [Required, StringLength(100, MinimumLength = 2)]
        public string nombre { get; set; } = string.Empty;

        [Required, EmailAddress, StringLength(150)]
        public string email { get; set; } = string.Empty;

        // BCrypt solo usa los primeros 72 caracteres, por eso ese es el máximo
        [Required, StringLength(72, MinimumLength = 8)]
        public string password { get; set; } = string.Empty;
    }

    public class LoginDto
    {
        [Required, EmailAddress]
        public string email { get; set; } = string.Empty;

        [Required]
        public string password { get; set; } = string.Empty;
    }

    public class CambiarPasswordDto
    {
        [Required]
        public string password_actual { get; set; } = string.Empty;

        [Required, StringLength(72, MinimumLength = 8)]
        public string password_nueva { get; set; } = string.Empty;
    }

    // Lo que la app necesita saber del usuario que inició sesión
    public class MeDto
    {
        public int id { get; set; }
        public string nombre { get; set; } = string.Empty;
        public string email { get; set; } = string.Empty;
        public string rol { get; set; } = "miembro";
        public int? hogar_id { get; set; }
        public string? hogar_nombre { get; set; }
        public bool tiene_hogar { get; set; }

        // Nombre del hogar al que pidió unirse y todavía no le responden (null si no hay)
        public string? solicitud_pendiente_hogar { get; set; }
    }

    public class AuthResponseDto
    {
        public string token { get; set; } = string.Empty;
        public MeDto usuario { get; set; } = new();
    }
}
