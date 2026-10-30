using RestauranteEstoque.Core.Domain;
using RestauranteEstoque.Core.Domain.ValueObjects;
using RestauranteEstoque.Vendas.Domain.Entities;
using RestauranteEstoque.Vendas.Domain.Enums;
using RestauranteEstoque.Vendas.Tests.Common;

namespace RestauranteEstoque.Vendas.Tests.Domain;

public class VendaTests
{
    private static NovoItemVenda Item(string nome = "Coca-Cola", decimal preco = 5m, int quantidade = 1) =>
        new(Guid.NewGuid(), nome, Dinheiro.Criar(preco), Quantidade.Criar(quantidade));

    [Fact]
    public void Criar_ComItensValidos_NascePendenteComTotalSomado()
    {
        var venda = Venda.Criar(TesteVendas.RestauranteId, TesteVendas.Agora,
            [Item("Coca-Cola", 5m, 2), Item("Pão de queijo", 3.5m, 4)], TesteVendas.Token);

        Assert.Equal(StatusVenda.Pendente, venda.Status);
        Assert.Equal(2, venda.Itens.Count);
        Assert.Equal(24m, venda.Total.Valor);
        Assert.All(venda.Itens, item => Assert.Equal(venda.Id, item.VendaId));
    }

    [Fact]
    public void Criar_SemItens_LancaDomainException()
    {
        Assert.Throws<DomainException>(() => Venda.Criar(TesteVendas.RestauranteId, TesteVendas.Agora, [], TesteVendas.Token));
    }

    [Fact]
    public void Criar_SemRestaurante_LancaDomainException()
    {
        Assert.Throws<DomainException>(() => Venda.Criar(Guid.Empty, TesteVendas.Agora, [Item()], TesteVendas.Token));
    }

    [Fact]
    public void Criar_ComProdutoRepetido_LancaDomainException()
    {
        var item = Item();

        Assert.Throws<DomainException>(() => Venda.Criar(TesteVendas.RestauranteId, TesteVendas.Agora, [item, item], TesteVendas.Token));
    }

    [Fact]
    public void Criar_ComQuantidadeZero_LancaDomainException()
    {
        Assert.Throws<DomainException>(() => Venda.Criar(TesteVendas.RestauranteId, TesteVendas.Agora, [Item(quantidade: 0)], TesteVendas.Token));
    }

    [Fact]
    public void AguardarPagamentoEConfirmar_SegueOCaminhoFeliz()
    {
        var venda = Venda.Criar(TesteVendas.RestauranteId, TesteVendas.Agora, [Item()], TesteVendas.Token);

        venda.AguardarPagamento();
        Assert.Equal(StatusVenda.AguardandoPagamento, venda.Status);

        venda.Confirmar();
        Assert.Equal(StatusVenda.Confirmada, venda.Status);
    }

    [Fact]
    public void Confirmar_VendaPendente_LancaDomainException()
    {
        var venda = Venda.Criar(TesteVendas.RestauranteId, TesteVendas.Agora, [Item()], TesteVendas.Token);

        Assert.Throws<DomainException>(venda.Confirmar);
    }

    [Fact]
    public void Cancelar_AguardandoPagamento_Cancela()
    {
        var venda = Venda.Criar(TesteVendas.RestauranteId, TesteVendas.Agora, [Item()], TesteVendas.Token);
        venda.AguardarPagamento();

        venda.Cancelar();

        Assert.Equal(StatusVenda.Cancelada, venda.Status);
    }

    [Fact]
    public void Cancelar_VendaJaConfirmada_LancaDomainException()
    {
        var venda = Venda.Criar(TesteVendas.RestauranteId, TesteVendas.Agora, [Item()], TesteVendas.Token);
        venda.AguardarPagamento();
        venda.Confirmar();

        Assert.Throws<DomainException>(venda.Cancelar);
    }

    [Fact]
    public void Criar_SemTokenCartao_LancaDomainException()
    {
        Assert.Throws<DomainException>(() => Venda.Criar(TesteVendas.RestauranteId, TesteVendas.Agora, [Item()], " "));
    }
}
