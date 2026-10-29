using RestauranteEstoque.Core.Domain;
using RestauranteEstoque.Domain.Entities;
using RestauranteEstoque.Core.Domain.ValueObjects;

namespace RestauranteEstoque.Tests.Domain;

public class FornecedorTests
{
    private static readonly DateTime CriadoEm = new(2026, 1, 1, 12, 0, 0, DateTimeKind.Utc);

    [Fact]
    public void Criar_ComDadosValidos_DeveCriarFornecedor()
    {
        var fornecedor = Fornecedor.Criar(DadosTeste.RestauranteId, "Distribuidora ABC", CriadoEm, "(11) 99999-0000", Email.Criar("contato@abc.com"));

        Assert.Equal("Distribuidora ABC", fornecedor.Nome);
        Assert.Equal("(11) 99999-0000", fornecedor.Telefone);
        Assert.Equal("contato@abc.com", fornecedor.Email?.Valor);
        Assert.Equal(CriadoEm, fornecedor.CriadoEm);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Criar_ComNomeInvalido_DeveLancarDomainException(string? nomeInvalido)
    {
        Assert.Throws<DomainException>(() => Fornecedor.Criar(DadosTeste.RestauranteId, nomeInvalido!, CriadoEm));
    }

    [Fact]
    public void Atualizar_ComDadosValidos_DeveAtualizarTodosOsCampos()
    {
        var fornecedor = Fornecedor.Criar(DadosTeste.RestauranteId, "Distribuidora ABC", CriadoEm);

        fornecedor.Atualizar("Distribuidora XYZ", "(11) 98888-1111", Email.Criar("novo@xyz.com"));

        Assert.Equal("Distribuidora XYZ", fornecedor.Nome);
        Assert.Equal("(11) 98888-1111", fornecedor.Telefone);
        Assert.Equal("novo@xyz.com", fornecedor.Email?.Valor);
    }

    [Fact]
    public void Criar_SemRestaurante_DeveLancarDomainException()
    {
        Assert.Throws<DomainException>(() => Fornecedor.Criar(Guid.Empty, "Distribuidora", CriadoEm));
    }
}
