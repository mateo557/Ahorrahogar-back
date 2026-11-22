namespace AhorrahogarApi.DTOs
{
    public class ContributeGoalDto
    {
        
        public int meta_id { get; set; }


        public decimal Monto { get; set; }

        public DateOnly Fecha { get; set; }
    }

    public class CreateContributeGoalDto
    {
        public int meta_id { get; set; }


        public decimal Monto { get; set; }

        public DateOnly Fecha { get; set; }
    }

    public class UpdateContributeGoalDto : CreateContributeGoalDto { }
}
