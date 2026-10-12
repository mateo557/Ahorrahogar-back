using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AhorrahogarApi.Models;


public class Movemnts
{
    public int Id { get; set; }
    public int hogar_id { get; set; }
    public int categoria_id { get; set; }

    public decimal monto { get; set; }

    public DateOnly Fecha { get; set; }

    public string? Nota { get; set; }

    public DateTime Creado_En { get; set; }
}
