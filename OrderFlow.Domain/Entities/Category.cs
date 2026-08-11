using OrderFlow.Domain.Exceptions;

namespace OrderFlow.Domain.Entities
{
    public class Category : BaseEntity<Guid>
    {
        public string Name { get; private set; }
        public string? Description { get; private set; }

        public Guid? ParentId { get; private set; }
        public ICollection<Category>? Children { get; private set; }
        public ICollection<Product>? Products { get; set; }

        public static Category Create(string name, string? description = null, Guid? parentCategoryId = null, Guid? createdBy = null)
        {
            ValidateName(name);
            var category = new Category
            {
                Id = Guid.NewGuid(),
                Name = name.Trim(),
                Description = description,
                ParentId = parentCategoryId,
            };
            category.CreateRecord(createdBy);

            return category;
        }

        public void Update(
     string? name = null,
     string? description = null,
     Guid? parentId = null,
     Guid? modifiedBy = null)
        {
            var changed = false;

            if (name is not null)
            {
                var newName = name.Trim();

                if (!string.Equals(Name, newName, StringComparison.Ordinal))
                {
                    Name = newName;
                    changed = true;
                }
            }

            if (description is not null)
            {
                var newDescription = description.Trim();

                if (!string.Equals(Description, newDescription, StringComparison.Ordinal))
                {
                    Description = newDescription;
                    changed = true;
                }
            }

            if (parentId is not null)
            {
                ValidateParentId(parentId, Id);

                if (ParentId != parentId)
                {
                    ParentId = parentId;
                    changed = true;
                }
            }

            if (changed)
                TouchRecord(modifiedBy);
        }

        private static void ValidateParentId(Guid? parentId, Guid currentCategoryId)
        {
            if (parentId.HasValue && parentId.Value == currentCategoryId)
            {
                throw new DomainValidationException("A category cannot be its own parent.");
            }
        }

        private static void ValidateName(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new DomainValidationException("Category name must not be empty.");
            if (name.Length > 250) throw new DomainValidationException("Category name is too long.");
        }
    }
}
