namespace AhorrahogarApi.Models
{
    public class Goals
    {
        public int Id { get; set; }
        public int Hogar_Id { get; set; }
        public int Categoria_Id { get; set; }
        public string Nombre { get; set; }

        public decimal Monto { get; set; }

        public DateOnly Fecha { get; set; }


        public DateTime Creado_En { get; set; }
    }
}
