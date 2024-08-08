namespace FaccToolkit.Domain.Rich
{
    public interface IDomainEvent<out TId> : IDomainEvent
    {
        TId Id { get; }
    }

    public interface IDomainEvent { }
}
