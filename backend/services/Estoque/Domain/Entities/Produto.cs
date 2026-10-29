using RestauranteEstoque.Core.Domain;
using RestauranteEstoque.Domain.Events;
using RestauranteEstoque.Core.Domain.ValueObjects;

namespace RestauranteEstoque.Domain.Entities;

public class Produto : Entity
{
    public static readonly Quantidade LimiteEstoqueBaixo = Quantidade.Criar(10);

    public string Nome { get; private set; } = string.Empty;
    public string? Descricao { get; private set; }
    public Dinheiro Preco { get; private set; } = Dinheiro.Zero;
    public Quantidade QuantidadeEmEstoque { get; private set; } = Quantidade.Zero;
    public Guid CategoriaId { get; private set; }
    public Categoria? Categoria { get; private set; }

    public Guid RestauranteId { get; private set; }

    public DateTime CriadoEm { get; private set; }

    private Produto()
    {
    }

    public static Produto Criar(Guid restauranteId, string nome, string? descricao, Dinheiro preco, Categoria categoria, DateTime criadoEm, Quantidade? quantidadeInicial = null)
    {
        ValidarRestauranteId(restauranteId);
        ValidarNome(nome);
        ValidarCategoria(categoria, restauranteId);

        return new Produto
        {
            Nome = nome.Trim(),
            Descricao = descricao?.Trim(),
            Preco = preco,
            CategoriaId = categoria.Id,
            QuantidadeEmEstoque = quantidadeInicial ?? Quantidade.Zero,
            RestauranteId = restauranteId,
            CriadoEm = criadoEm
        };
    }

    public void Atualizar(string nome, string? descricao, Dinheiro preco, Categoria categoria)
    {
        ValidarNome(nome);
        ValidarCategoria(categoria, RestauranteId);

        Nome = nome.Trim();
        Descricao = descricao?.Trim();
        Preco = preco;
        CategoriaId = categoria.Id;
    }

    public void AumentarEstoque(Quantidade quantidade)
    {
        ValidarQuantidadeMovimentada(quantidade);

        QuantidadeEmEstoque = QuantidadeEmEstoque.Somar(quantidade);
    }

    public void DiminuirEstoque(Quantidade quantidade)
    {
        ValidarQuantidadeMovimentada(quantidade);

        if (quantidade > QuantidadeEmEstoque)
            throw new DomainException($"Estoque insuficiente: disponível {QuantidadeEmEstoque}, solicitado {quantidade}.");

        var estavaAcimaDoLimite = QuantidadeEmEstoque >= LimiteEstoqueBaixo;

        QuantidadeEmEstoque = QuantidadeEmEstoque.Subtrair(quantidade);

        if (estavaAcimaDoLimite && QuantidadeEmEstoque < LimiteEstoqueBaixo)
            RegistrarEvento(new EstoqueFicouBaixoEvent(Id, Nome, QuantidadeEmEstoque.Valor, RestauranteId));
    }

    public void GarantirQuePodeSerExcluido(bool possuiMovimentacoes)
    {
        if (possuiMovimentacoes)
            throw new DomainException("Não é possível excluir um produto que possui movimentações de estoque vinculadas.");
    }

    private static void ValidarRestauranteId(Guid restauranteId)
    {
        if (restauranteId == Guid.Empty)
            throw new DomainException("O produto precisa pertencer a um restaurante.");
    }

    private static void ValidarQuantidadeMovimentada(Quantidade quantidade)
    {
        if (quantidade.EhZero)
            throw new DomainException("A quantidade movimentada deve ser maior que zero.");
    }

    private static void ValidarNome(string nome)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new DomainException("O nome do produto é obrigatório.");

        if (nome.Trim().Length > 150)
            throw new DomainException("O nome do produto deve ter no máximo 150 caracteres.");
    }

    private static void ValidarCategoria(Categoria categoria, Guid restauranteId)
    {
        if (categoria is null)
            throw new DomainException("O produto precisa estar vinculado a uma categoria.");

        if (categoria.RestauranteId != restauranteId)
            throw new DomainException("A categoria precisa ser do mesmo restaurante do produto.");
    }
}
