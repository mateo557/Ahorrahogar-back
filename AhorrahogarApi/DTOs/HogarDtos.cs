using System.ComponentModel.DataAnnotations;

namespace AhorrahogarApi.DTOs
{
    public class CrearHogarDto
    {
        [Required, StringLength(100, MinimumLength = 2)]
        public string nombre { get; set; } = string.Empty;
    }

    public class UnirseDto
    {
        [Required, StringLength(8, MinimumLength = 4)]
        public string codigo { get; set; } = string.Empty;
    }

    public class MiembroDto
    {
        public int id { get; set; }
        public string nombre { get; set; } = string.Empty;
        public string email { get; set; } = string.Empty;
        public string rol { get; set; } = "miembro";
    }

    public class HogarDto
    {
        public int id { get; set; }
        public string nombre { get; set; } = string.Empty;

        // Solo lo ve el administrador; para los demás va vacío
        public string? codigo_invitacion { get; set; }

        public List<MiembroDto> miembros { get; set; } = new();
    }

    public class SolicitudDto
    {
        public int id { get; set; }
        public int integrante_id { get; set; }
        public string nombre { get; set; } = string.Empty;
        public string email { get; set; } = string.Empty;
        public DateTime creado_en { get; set; }
    }

    public class CodigoDto
    {
        public string codigo_invitacion { get; set; } = string.Empty;
    }
}
