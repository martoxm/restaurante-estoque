using RestauranteEstoque.Core.Domain;
using RestauranteEstoque.Core.Domain.ValueObjects;
using RestauranteEstoque.Pagamentos.Domain.Entities;
using RestauranteEstoque.Pagamentos.Domain.Enums;
using RestauranteEstoque.Pagamentos.Tests.Common;

namespace RestauranteEstoque.Pagamentos.Tests.Domain;

public class PagamentoTests
{
    [Fact]
    public void Criar_NasceComoPendente()
    {
        var pagamento = TestePagamentos.NovoPagamento();

        Assert.Equal(StatusPagamento.Pendente, pagamento.Status);
        Assert.Null(pagamento.ProcessadoEm);
    }

    [Fact]
    public void Criar_ComValorZero_LancaDomainException()
    {
        Assert.Throws<DomainException>(() =>
            Pagamento.Criar(TestePagamentos.RestauranteId, Guid.NewGuid(), Dinheiro.Criar(0m), "tok_aprovado", "chave", TestePagamentos.Agora));
    }

    [Fact]
    public void Criar_SemChaveIdempotencia_LancaDomainException()
    {
        Assert.Throws<DomainException>(() =>
            Pagamento.Criar(TestePagamentos.RestauranteId, Guid.NewGuid(), Dinheiro.Criar(10m), "tok_aprovado", " ", TestePagamentos.Agora));
    }

    [Fact]
    public void Aprovar_GuardaTransacaoEData()
    {
        var pagamento = TestePagamentos.NovoPagamento();

        pagamento.MarcarComoProcessando("fake_tr_1");
        pagamento.Aprovar(TestePagamentos.Agora);

        Assert.Equal(StatusPagamento.Aprovado, pagamento.Status);
        Assert.Equal("fake_tr_1", pagamento.IdTransacaoGateway);
        Assert.Equal(TestePagamentos.Agora, pagamento.ProcessadoEm);
    }

    [Fact]
    public void Recusar_DepoisDeAprovado_LancaDomainException()
    {
        var pagamento = TestePagamentos.NovoPagamento();
        pagamento.MarcarComoProcessando("fake_tr_1");
        pagamento.Aprovar(TestePagamentos.Agora);

        Assert.Throws<DomainException>(() => pagamento.Recusar("Saldo insuficiente.", TestePagamentos.Agora));
    }

    [Fact]
    public void EhMesmoPedido_ComparaVendaValorEToken()
    {
        var pagamento = TestePagamentos.NovoPagamento();

        Assert.True(pagamento.EhMesmoPedido(pagamento.VendaId, 50m, "tok_aprovado"));
        Assert.False(pagamento.EhMesmoPedido(pagamento.VendaId, 60m, "tok_aprovado"));
        Assert.False(pagamento.EhMesmoPedido(Guid.NewGuid(), 50m, "tok_aprovado"));
    }
}
