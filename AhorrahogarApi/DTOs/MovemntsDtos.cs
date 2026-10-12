using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AhorrahogarApi.DTOs;


public class MovemntsDto
{
   

    public int categoria_id { get; set; }
   
    public decimal monto { get; set; }

    public DateOnly Fecha { get; set; }

    [StringLength(200)]
    public string? Nota { get; set; }

    public DateTime creado_en { get; set; }
}


public class CreateMovemntsDto
{
    public int HogarId { get; set; }
    public int CategoriaId { get; set; }

    public decimal monto { get; set; }

    public DateOnly Fecha { get; set; }

    [StringLength(200)]
    public string? Nota { get; set; }

 }


public class UpdateMovemntsDto : CreateMovemntsDto { }
