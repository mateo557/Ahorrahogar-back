using System.ComponentModel.DataAnnotations;

namespace AhorrahogarApi.Models;

public class Movemnts
{
    public int Id { get; set; }
    public int Hogar_Id { get; set; }
    public int Categoria_Id { get; set; }
    public string Tipo { get; set; } = string.Empty;

    public decimal Monto { get; set; }

    public DateOnly Fecha { get; set; }

    public string? Nota { get; set; }

    public DateTime Creado_En { get; set; }
}
