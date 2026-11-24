using System.ComponentModel.DataAnnotations.Schema;

namespace AhorrahogarApi.Models
{
    public class Members
    {
        public int Id { get; set; }

        // Vacío mientras el usuario todavía no crea un hogar ni se une a uno
        public int? Hogar_Id { get; set; }

        public string nombre { get; set; } = string.Empty;

        public DateTime Creado_En { get; set; }
<<<<<<< Updated upstream
=======

        public string? Email { get; set; }

        // Hash de BCrypt; la contraseña real nunca se guarda
        [Column("password_hash")]
        public string? PasswordHash { get; set; }

        // "admin" (quien creó el hogar) o "miembro"
        public string Rol { get; set; } = "miembro";
>>>>>>> Stashed changes
    }
}
