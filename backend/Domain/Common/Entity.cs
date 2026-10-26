namespace RestauranteEstoque.Domain.Common;

public abstract class Entity
{
    public Guid Id { get; protected set; }

    private readonly List<IDomainEvent> _eventos = [];
    public IReadOnlyCollection<IDomainEvent> Eventos => _eventos.AsReadOnly();

    protected Entity()
    {
        Id = Guid.NewGuid();
    }

    protected void RegistrarEvento(IDomainEvent evento) => _eventos.Add(evento);

    public void LimparEventos() => _eventos.Clear();
}
