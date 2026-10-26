using RestauranteEstoque.Domain.Common;
using RestauranteEstoque.Domain.Entities;

namespace RestauranteEstoque.Tests.Domain;

public class CategoriaTests
{
    private static readonly DateTime CriadoEm = new(2026, 1, 1, 12, 0, 0, DateTimeKind.Utc);

    [Fact]
    public void Criar_ComNomeValido_DeveCriarCategoria()
    {
        var categoria = Categoria.Criar(DadosTeste.RestauranteId, "Bebidas", CriadoEm, "Bebidas em geral");

        Assert.Equal("Bebidas", categoria.Nome);
        Assert.Equal("Bebidas em geral", categoria.Descricao);
        Assert.Equal(CriadoEm, categoria.CriadoEm);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Criar_ComNomeInvalido_DeveLancarDomainException(string? nomeInvalido)
    {
        Assert.Throws<DomainException>(() => Categoria.Criar(DadosTeste.RestauranteId, nomeInvalido!, CriadoEm));
    }

    [Fact]
    public void Criar_ComNomeMaiorQue100Caracteres_DeveLancarDomainException()
    {
        var nomeMuitoLongo = new string('A', 101);

        Assert.Throws<DomainException>(() => Categoria.Criar(DadosTeste.RestauranteId, nomeMuitoLongo, CriadoEm));
    }

    [Fact]
    public void Atualizar_ComDadosValidos_DeveAtualizarNomeEDescricao()
    {
        var categoria = Categoria.Criar(DadosTeste.RestauranteId, "Bebidas", CriadoEm);

        categoria.Atualizar("Bebidas Geladas", "Cervejas e refrigerantes");

        Assert.Equal("Bebidas Geladas", categoria.Nome);
        Assert.Equal("Cervejas e refrigerantes", categoria.Descricao);
    }

    [Fact]
    public void GarantirQuePodeSerExcluida_SemProdutos_NaoDeveLancarExcecao()
    {
        var categoria = Categoria.Criar(DadosTeste.RestauranteId, "Bebidas", CriadoEm);

        var excecao = Record.Exception(() => categoria.GarantirQuePodeSerExcluida(possuiProdutos: false));

        Assert.Null(excecao);
    }

    [Fact]
    public void GarantirQuePodeSerExcluida_ComProdutos_DeveLancarDomainException()
    {
        var categoria = Categoria.Criar(DadosTeste.RestauranteId, "Bebidas", CriadoEm);

        var excecao = Assert.Throws<DomainException>(() => categoria.GarantirQuePodeSerExcluida(possuiProdutos: true));

        Assert.Equal("Não é possível excluir uma categoria que possui produtos vinculados.", excecao.Message);
    }

    [Fact]
    public void Criar_SemRestaurante_DeveLancarDomainException()
    {
        Assert.Throws<DomainException>(() => Categoria.Criar(Guid.Empty, "Bebidas", CriadoEm));
    }
}
