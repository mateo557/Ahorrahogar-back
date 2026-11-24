using System.ComponentModel.DataAnnotations;

namespace AhorrahogarApi.Models
{
    public class Balance
    {
        [Key]
        public int id { get; set; }
        public int hogar_id { get; set; }
        public DateOnly periodo { get; set; }
        public decimal balance { get; set; }
        public decimal balance_ingreso { get; set; }
        public decimal balance_gasto { get; set; }
    }
}
