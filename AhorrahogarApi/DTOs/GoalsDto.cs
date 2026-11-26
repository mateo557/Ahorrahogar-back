using System.ComponentModel.DataAnnotations;

namespace AhorrahogarApi.DTOs
{
    public class GoalsDto
    {
        public int id { get; set; }
        public int Hogar_Id { get; set; }
        public string Nombre { get; set; }

        public decimal montoobjetivo { get; set; }

        public DateOnly fechalimite { get; set; }

        public DateTime CreadoEn { get; set; }

        public decimal montoahorrado { get; set; }
        public decimal porcentaje { get; set; }



    }


    public class CreateGoalsDto
    {
        public int id { get; set; }
        public int Hogar_Id { get; set; }
        public string Nombre { get; set; }
        public decimal montoobjetivo { get; set; }
        public DateOnly fechalimite { get; set; }
    }

    public class UpdateGoalsDto : CreateGoalsDto { }
}

