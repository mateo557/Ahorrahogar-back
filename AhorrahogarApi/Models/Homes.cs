using System.ComponentModel.DataAnnotations.Schema;

namespace AhorrahogarApi.Models
{
    public class Homes
    {
        public int Id { get; set; }

        public string Nombre { get; set; } = string.Empty;

        public DateTime Creado_En { get; set; }

        [Column("codigo_invitacion")]
        public string Codigo_Invitacion { get; set; } = string.Empty;
    }
}
