namespace ToDos.Domain.Interfaces
{
    public interface IBaseEntity<T>
    {
        T Id { get; set; }
    }
}
