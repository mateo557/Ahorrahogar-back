namespace AhorrahogarApi.DTOs
{
    public class CategoryDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Tipo { get; set; }
    }

    public class CreateCategoryDTO
    {
        public string Nombre { get; set; }
        public string Tipo { get; set; }
    }
    public class UpdateCategoryDto : CreateCategoryDTO { }
}
