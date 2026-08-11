using OrderFlow.Domain.Exceptions;

namespace OrderFlow.Domain.Entities;

public abstract class BaseEntity<T> : IBaseEntity<T>
{
    public T Id { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsDeleted { get; set; } = false;
    public DateTime? CreateDate { get; set; } = DateTime.Now;
    public DateTime? ModifiedDate { get; set; }
    public Guid? ModifiedById { get; set; }
    public Guid? CreateById { get; set; }

    public virtual void SoftDelete(Guid? modifiedById = null)
    {
        if (IsDeleted) return;
        IsDeleted = true;
        IsActive = false;
        TouchRecord(modifiedById);
    }

    public virtual void Activate(Guid? modifiedById = null)
    {
        if (IsActive)
            throw new DomainValidationException("این مورد از قبل فعال است.");
        IsActive = true;
        TouchRecord(modifiedById);
    }

    public virtual void Inactivate(Guid? modifiedById = null)
    {
        if (!IsActive)
            throw new DomainValidationException("این مورد از قبل غیر فعال است.");
        IsActive = false;
        TouchRecord(modifiedById);
    }

    public virtual void TouchRecord(Guid? modifiedById = null)
    {
        ModifiedById = modifiedById;
        ModifiedDate = DateTime.Now;
    }

    public virtual void CreateRecord(Guid? createdById = null)
    {
        CreateById = createdById;
        CreateDate = DateTime.Now;
    }
}

public abstract class BaseEntity : BaseEntity<long>, IBaseEntity<long>
{
}

public abstract class BaseLookupEntity<T> : IBaseLookupEntity<T>
{
    public T Id { get; set; }
    public string Name { get; set; }
    public string FarsiName { get; set; }
    public bool IsActive { get; set; } = true;
}