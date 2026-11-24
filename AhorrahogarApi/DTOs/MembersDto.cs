using System.ComponentModel.DataAnnotations;

namespace AhorrahogarApi.DTOs
{
    public class MemberDto
    {
        public int Id { get; set; }
        public int? Hogar_Id { get; set; }

        public string nombre { get; set; } = string.Empty;

        public DateTime Creado_En { get; set; }

        public string? Email { get; set; }
    }


    public class CreateMemberDto
    {
        public int? Hogar_Id { get; set; }

        public string nombre { get; set; } = string.Empty;

        public string? Email { get; set; }
    }


    public class UpdateMemberDto : CreateMemberDto { }

}
