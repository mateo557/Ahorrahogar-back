using System.ComponentModel.DataAnnotations.Schema;

namespace AhorrahogarApi.Models
{
    public class Goals
    {
        public int Id { get; set; }
        public int Hogar_Id { get; set; }
        public string Nombre { get; set; }
        [Column("montoobjetivo")]
        public decimal montoobjetivo { get; set; }
        [Column("fechalimite")]
        public DateOnly fechalimite { get; set; }

        [Column("creadoen")]
        public DateTime creadoen { get; set; }
    }
}
