using System.ComponentModel.DataAnnotations;

namespace AhorrahogarApi.DTOs;

/// <summary>Lo que devuelve la API.</summary>
public class MovemntsDto
{
    public int Id { get; init; }
    public int HogarId { get; init; }
    public int CategoriaId { get; init; }

    [Required]
    [StringLength(10)]
    public string Tipo { get; init; } = string.Empty;

    public decimal Monto { get; init; }

    public DateOnly Fecha { get; init; }

    [StringLength(200)]
    public string? Nota { get; init; }

    public DateTime CreadoEn { get; init; }
}

/// <summary>Lo que recibe la API al crear.</summary>
public class CreateMovemntsDto
{
    public int Id { get; init; }
    public int HogarId { get; init; }
    public int CategoriaId { get; init; }

    [Required]
    [StringLength(10)]
    public string Tipo { get; init; } = string.Empty;

    public decimal Monto { get; init; }

    public DateOnly Fecha { get; init; }

    [StringLength(200)]
    public string? Nota { get; init; }

    public DateTime CreadoEn { get; init; }
}

/// <summary>Lo que recibe la API al actualizar.</summary>
public class UpdateMovemntsDto : CreateMovemntsDto { }
