namespace AhorrahogarApi.Models
{
    public class Members
    {
        public int Id { get; set; }
        public int Hogar_Id { get; set; }
       
        public string nombre { get; set; } = string.Empty;

        public DateTime Creado_En { get; set; }
    }
}
