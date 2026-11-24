namespace AhorrahogarApi.DTOs
{
    public class BalanceDTO
    {
        public int id { get; set; }
        public int hogar_id { get; set; }
        public DateOnly periodo { get; set; }
        public decimal balance { get; set; }
        public decimal balance_ingreso { get; set; }
        public decimal balance_gasto { get; set; }
        
    
    }
    public class CreateBalanceDTO
    {
        
        public int hogar_id { get; set; }
        public DateOnly periodo { get; set; }
        public decimal balance { get; set; }
        public decimal balance_ingreso { get; set; }
        public decimal balance_gasto { get; set; }
    }

    public class UpdateBalanceDTO : CreateBalanceDTO { }

    public class CategoriaTotalDto
    {
        public int categoria_id { get; set; }
        public string nombre { get; set; } = string.Empty;
        public decimal total { get; set; }
        public decimal porcentaje { get; set; }
    }
}
