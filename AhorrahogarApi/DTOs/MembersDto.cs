using System.ComponentModel.DataAnnotations;

namespace AhorrahogarApi.DTOs
{
    public class MemberDto
    {
      
        public int Hogar_Id { get; set; }

        public string nombre { get; set; } = string.Empty;

        public DateTime Creado_En { get; set; }
    }


    public class CreateMemberDto
    {
       
        public int Hogar_Id { get; set; }

        public string nombre { get; set; } = string.Empty;

        
    }


    public class UpdateMemberDto : CreateMemberDto { }

}
