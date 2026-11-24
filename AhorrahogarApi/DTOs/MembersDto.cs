using System.ComponentModel.DataAnnotations;

namespace AhorrahogarApi.DTOs
{
    public class MemberDto
    {
<<<<<<< Updated upstream
      
        public int Hogar_Id { get; set; }
=======
        public int Id { get; set; }
        public int? Hogar_Id { get; set; }
>>>>>>> Stashed changes

        public string nombre { get; set; } = string.Empty;

        public DateTime Creado_En { get; set; }
<<<<<<< Updated upstream
=======

        public string? Email { get; set; }
>>>>>>> Stashed changes
    }


    public class CreateMemberDto
    {
        public int? Hogar_Id { get; set; }

        public string nombre { get; set; } = string.Empty;

<<<<<<< Updated upstream
        
=======
        public string? Email { get; set; }
>>>>>>> Stashed changes
    }


    public class UpdateMemberDto : CreateMemberDto { }

}
