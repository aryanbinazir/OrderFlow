namespace OrderFlow.Application.DTOs.Folder.Category
{
    public class GetByIdCategoryResDto
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public Guid? ParentId { get; set; }
        public List<GetByIdCategoryResDto_Products>? Products { get; set; }
        public List<GetByIdCategoryResDto_Children>? Children { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
    }

    public class GetByIdCategoryResDto_Products
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }

    public class GetByIdCategoryResDto_Children
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}
