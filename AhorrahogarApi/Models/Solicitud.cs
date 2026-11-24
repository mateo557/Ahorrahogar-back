namespace AhorrahogarApi.Models
{
    // Una persona pidió unirse a un hogar con el código de invitación
    public class Solicitud
    {
        public int Id { get; set; }
        public int Hogar_Id { get; set; }
        public int Integrante_Id { get; set; }

        // pendiente | aceptada | rechazada | cancelada
        public string Estado { get; set; } = "pendiente";

        public DateTime Creado_En { get; set; }
        public DateTime? Resuelto_En { get; set; }
    }
}
