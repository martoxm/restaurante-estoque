using RestauranteEstoque.Core.Domain;
using RestauranteEstoque.Core.Domain.ValueObjects;
using RestauranteEstoque.Vendas.Domain.Enums;

namespace RestauranteEstoque.Vendas.Domain.Entities;

public class Venda : Entity
{
    private readonly List<ItemVenda> _itens = [];

    public Guid RestauranteId { get; private set; }
    public StatusVenda Status { get; private set; }
    public DateTime CriadoEm { get; private set; }

    public IReadOnlyCollection<ItemVenda> Itens => _itens.AsReadOnly();

    public Dinheiro Total => Dinheiro.Criar(_itens.Sum(item => item.Subtotal.Valor));

    private Venda()
    {
    }

    public static Venda Criar(Guid restauranteId, DateTime criadoEm, IReadOnlyCollection<NovoItemVenda> itens)
    {
        if (restauranteId == Guid.Empty)
            throw new DomainException("A venda precisa pertencer a um restaurante.");

        if (itens.Count == 0)
            throw new DomainException("A venda precisa ter pelo menos um item.");

        if (itens.Select(item => item.ProdutoId).Distinct().Count() != itens.Count)
            throw new DomainException("Um produto não pode aparecer mais de uma vez na mesma venda.");

        var venda = new Venda
        {
            RestauranteId = restauranteId,
            Status = StatusVenda.Pendente,
            CriadoEm = criadoEm
        };

        foreach (var item in itens)
            venda._itens.Add(ItemVenda.Criar(venda.Id, item.ProdutoId, item.NomeProduto, item.PrecoUnitario, item.Quantidade));

        return venda;
    }

    public void Confirmar()
    {
        GarantirQueEstaPendente();
        Status = StatusVenda.Confirmada;
    }

    public void Cancelar()
    {
        GarantirQueEstaPendente();
        Status = StatusVenda.Cancelada;
    }

    private void GarantirQueEstaPendente()
    {
        if (Status != StatusVenda.Pendente)
            throw new DomainException($"Só uma venda pendente pode mudar de status (esta está {Status}).");
    }
}

public record NovoItemVenda(Guid ProdutoId, string NomeProduto, Dinheiro PrecoUnitario, Quantidade Quantidade);
