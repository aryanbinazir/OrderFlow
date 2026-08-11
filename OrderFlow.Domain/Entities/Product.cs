using OrderFlow.Domain.Exceptions;

namespace OrderFlow.Domain.Entities
{
    public class Product : BaseEntity<Guid>
    {
        public string Name { get; private set; } = string.Empty;
        public string? Description { get; private set; }
        public string SKU { get; private set; } = string.Empty;
        public decimal Price { get; private set; }
        public int Stock { get; private set; }

        public Guid? CategoryId { get; private set; }
        public Category? Category { get; private set; }
        public ICollection<OrderItem>? orderItems { get; set; }

        public static Product Create(string name, decimal price, string sku, Guid? categoryId, int stock = 0, string? description = null, Guid? createdBy = null)
        {
            ValidateName(name);
            ValidatePrice(price);
            ValidateSku(sku);
            if (stock < 0) throw new DomainValidationException("Stock must be non-negative.");

            var product = new Product
            {
                Id = Guid.NewGuid(),
                Name = name.Trim(),
                Price = price,
                SKU = sku.Trim(),
                CategoryId = categoryId,
                Stock = stock,
                Description = description
            };
            product.CreateRecord(createdBy);

            return product;
        }

        public void Update(
    string? name = null,
    decimal? price = null,
    string? sku = null,
    Guid? categoryId = null,
    string? description = null,
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

            if (price is not null)
            {
                if (Price != price)
                {
                    Price = price.Value;
                    changed = true;
                }
            }

            if (sku is not null)
            {
                var newSku = sku.Trim();

                if (!string.Equals(SKU, newSku, StringComparison.Ordinal))
                {
                    SKU = newSku;
                    changed = true;
                }
            }

            if (categoryId is not null)
            {
                if (CategoryId != categoryId)
                {
                    CategoryId = categoryId.Value;
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

            if (changed)
                TouchRecord(modifiedBy);
        }
        public void IncreaseStock(int amount, Guid? modifiedBy = null)
        {
            if (amount <= 0) throw new DomainValidationException("Amount must be greater than zero.");
            Stock += amount;
            TouchRecord(modifiedBy);
        }

        public void DecreaseStock(int amount, Guid? modifiedBy = null)
        {
            if (amount <= 0) throw new DomainValidationException("Amount must be greater than zero.");
            if (amount > Stock) throw new DomainValidationException("Insufficient stock.");
            Stock -= amount;
            TouchRecord(modifiedBy);
        }

        // Validation helpers
        private static void ValidateName(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new DomainValidationException("Product name must not be empty.");
            if (name.Length > 250) throw new DomainValidationException("Product name is too long.");
        }

        private static void ValidateSku(string sku)
        {
            if (string.IsNullOrWhiteSpace(sku)) throw new DomainValidationException("SKU must not be empty.");
            if (sku.Length > 100) throw new DomainValidationException("SKU is too long.");
        }

        private static void ValidatePrice(decimal price)
        {
            if (price == 0) throw new DomainValidationException("Price must not be zero.");
            if (price < 0) throw new DomainValidationException("Price must be non-negative.");
        }
    }
}
