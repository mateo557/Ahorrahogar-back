using System.ComponentModel.DataAnnotations.Schema;

namespace AhorrahogarApi.Models
{
    public class Categorys
    {
        public int Id { get; set; }
        
        public string Nombre { get; set; }
        public string Tipo { get; set; }
    }
}
