namespace OrderFlow.Domain.Entities;

public interface IBaseEntity<T>
{
    public T Id { get; set; }
    public bool IsActive { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? CreateDate { get; set; }
    public DateTime? ModifiedDate { get; set; }
    public Guid? ModifiedById { get; set; }
    public Guid? CreateById { get; set; }
}

public interface IBaseLookupEntity<T>
{
    public T Id { get; set; }
    public string Name { get; set; }
    public string FarsiName { get; set; }
    public bool IsActive { get; set; }
};
