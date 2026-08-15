namespace OrderFlow.Application.DTOs.Folder.Category
{
    public class GetAllCategoryResDto
    {
        public List<GetAllCategoryResDto_Category> Categories { get; set; } = [];
    }

    public class GetAllCategoryResDto_Category
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public Guid? ParentId { get; set; }
        public List<GetAllCategoryResDto_Category_Products>? Products { get; set; }

    }

    public class GetAllCategoryResDto_Category_Products
    {
        public string Name { get; set; }
    }
}
