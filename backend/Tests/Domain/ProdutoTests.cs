using RestauranteEstoque.Domain.Common;
using RestauranteEstoque.Domain.Entities;
using RestauranteEstoque.Domain.Events;
using RestauranteEstoque.Domain.ValueObjects;

namespace RestauranteEstoque.Tests.Domain;

public class ProdutoTests
{
    private static readonly DateTime CriadoEm = new(2026, 1, 1, 12, 0, 0, DateTimeKind.Utc);

    [Fact]
    public void Criar_ComDadosValidos_DeveCriarProdutoComOsValoresInformados()
    {
        var categoria = DadosTeste.CriarCategoria();

        var produto = Produto.Criar(DadosTeste.RestauranteId, "Arroz", "Arroz branco tipo 1", Dinheiro.Criar(25.90m), categoria, CriadoEm, quantidadeInicial: Quantidade.Criar(10));

        Assert.Equal("Arroz", produto.Nome);
        Assert.Equal("Arroz branco tipo 1", produto.Descricao);
        Assert.Equal(25.90m, produto.Preco.Valor);
        Assert.Equal(categoria.Id, produto.CategoriaId);
        Assert.Equal(10, produto.QuantidadeEmEstoque.Valor);
        Assert.Equal(CriadoEm, produto.CriadoEm);
    }

    [Fact]
    public void Criar_DeveRemoverEspacosEmBrancoDoNomeEDaDescricao()
    {
        var produto = Produto.Criar(DadosTeste.RestauranteId, "  Feijão  ", "  Feijão carioca  ", Dinheiro.Criar(8m), DadosTeste.CriarCategoria(), CriadoEm);

        Assert.Equal("Feijão", produto.Nome);
        Assert.Equal("Feijão carioca", produto.Descricao);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Criar_ComNomeInvalido_DeveLancarDomainException(string? nomeInvalido)
    {
        Assert.Throws<DomainException>(() =>
            Produto.Criar(DadosTeste.RestauranteId, nomeInvalido!, null, Dinheiro.Criar(10m), DadosTeste.CriarCategoria(), CriadoEm));
    }

    [Fact]
    public void Criar_ComPrecoNegativo_DeveLancarDomainException()
    {
        Assert.Throws<DomainException>(() =>
            Produto.Criar(DadosTeste.RestauranteId, "Arroz", null, Dinheiro.Criar(-1m), DadosTeste.CriarCategoria(), CriadoEm));
    }

    [Fact]
    public void Criar_SemCategoria_DeveLancarDomainException()
    {
        Assert.Throws<DomainException>(() =>
            Produto.Criar(DadosTeste.RestauranteId, "Arroz", null, Dinheiro.Criar(10m), null!, CriadoEm));
    }

    [Fact]
    public void Criar_ComCategoriaDeOutroRestaurante_DeveLancarDomainException()
    {
        var categoriaDeOutroRestaurante = DadosTeste.CriarCategoria(Guid.NewGuid());

        var excecao = Assert.Throws<DomainException>(() =>
            Produto.Criar(DadosTeste.RestauranteId, "Arroz", null, Dinheiro.Criar(10m), categoriaDeOutroRestaurante, CriadoEm));

        Assert.Equal("A categoria precisa ser do mesmo restaurante do produto.", excecao.Message);
    }

    [Fact]
    public void Atualizar_ComCategoriaDeOutroRestaurante_DeveLancarDomainException()
    {
        var produto = CriarProdutoComEstoque(5);
        var categoriaDeOutroRestaurante = DadosTeste.CriarCategoria(Guid.NewGuid());

        Assert.Throws<DomainException>(() =>
            produto.Atualizar("Arroz", null, Dinheiro.Criar(10m), categoriaDeOutroRestaurante));
    }

    [Fact]
    public void Atualizar_ComCategoriaDoMesmoRestaurante_DeveTrocarACategoria()
    {
        var produto = CriarProdutoComEstoque(5);
        var categoriaNova = DadosTeste.CriarCategoria();

        produto.Atualizar("Arroz", null, Dinheiro.Criar(10m), categoriaNova);

        Assert.Equal(categoriaNova.Id, produto.CategoriaId);
    }

    [Fact]
    public void Criar_ComQuantidadeInicialNegativa_DeveLancarDomainException()
    {
        Assert.Throws<DomainException>(() =>
            Produto.Criar(DadosTeste.RestauranteId, "Arroz", null, Dinheiro.Criar(10m), DadosTeste.CriarCategoria(), CriadoEm, quantidadeInicial: Quantidade.Criar(-1)));
    }

    [Fact]
    public void AumentarEstoque_DeveSomarQuantidadeAoEstoqueAtual()
    {
        var produto = Produto.Criar(DadosTeste.RestauranteId, "Arroz", null, Dinheiro.Criar(10m), DadosTeste.CriarCategoria(), CriadoEm, quantidadeInicial: Quantidade.Criar(5));

        produto.AumentarEstoque(Quantidade.Criar(3));

        Assert.Equal(8, produto.QuantidadeEmEstoque.Valor);
    }

    [Fact]
    public void DiminuirEstoque_ComEstoqueSuficiente_DeveSubtrairQuantidade()
    {
        var produto = Produto.Criar(DadosTeste.RestauranteId, "Arroz", null, Dinheiro.Criar(10m), DadosTeste.CriarCategoria(), CriadoEm, quantidadeInicial: Quantidade.Criar(5));

        produto.DiminuirEstoque(Quantidade.Criar(3));

        Assert.Equal(2, produto.QuantidadeEmEstoque.Valor);
    }

    [Fact]
    public void DiminuirEstoque_ComEstoqueInsuficiente_DeveLancarDomainExceptionENaoAlterarQuantidade()
    {
        var produto = Produto.Criar(DadosTeste.RestauranteId, "Arroz", null, Dinheiro.Criar(10m), DadosTeste.CriarCategoria(), CriadoEm, quantidadeInicial: Quantidade.Criar(5));

        Assert.Throws<DomainException>(() => produto.DiminuirEstoque(Quantidade.Criar(10)));
        Assert.Equal(5, produto.QuantidadeEmEstoque.Valor);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void AumentarEstoque_ComQuantidadeZeroOuNegativa_DeveLancarDomainException(int quantidade)
    {
        var produto = Produto.Criar(DadosTeste.RestauranteId, "Arroz", null, Dinheiro.Criar(10m), DadosTeste.CriarCategoria(), CriadoEm);

        Assert.Throws<DomainException>(() => produto.AumentarEstoque(Quantidade.Criar(quantidade)));
    }


    private static Produto CriarProdutoComEstoque(int quantidade) =>
        Produto.Criar(DadosTeste.RestauranteId, "Arroz", null, Dinheiro.Criar(10m), DadosTeste.CriarCategoria(), CriadoEm, quantidadeInicial: Quantidade.Criar(quantidade));

    [Fact]
    public void DiminuirEstoque_CruzandoOLimite_DeveRegistrarEventoEstoqueFicouBaixo()
    {
        var produto = CriarProdutoComEstoque(12);

        produto.DiminuirEstoque(Quantidade.Criar(5));

        var evento = Assert.IsType<EstoqueFicouBaixoEvent>(Assert.Single(produto.Eventos));
        Assert.Equal(produto.Id, evento.ProdutoId);
        Assert.Equal("Arroz", evento.NomeProduto);
        Assert.Equal(7, evento.QuantidadeAtual);
    }

    [Fact]
    public void DiminuirEstoque_SaindoExatamenteDoLimite_DeveRegistrarEvento()
    {
        var produto = CriarProdutoComEstoque(10);

        produto.DiminuirEstoque(Quantidade.Criar(1));

        Assert.Single(produto.Eventos);
    }

    [Fact]
    public void DiminuirEstoque_ContinuandoAcimaDoLimite_NaoDeveRegistrarEvento()
    {
        var produto = CriarProdutoComEstoque(30);

        produto.DiminuirEstoque(Quantidade.Criar(20));

        Assert.Empty(produto.Eventos);
    }

    [Fact]
    public void DiminuirEstoque_QuandoJaEstavaBaixo_NaoDeveRegistrarEventoRepetido()
    {
        var produto = CriarProdutoComEstoque(8);

        produto.DiminuirEstoque(Quantidade.Criar(2));

        Assert.Empty(produto.Eventos);
    }

    [Fact]
    public void LimparEventos_DeveEsvaziarAListaDeEventos()
    {
        var produto = CriarProdutoComEstoque(12);
        produto.DiminuirEstoque(Quantidade.Criar(5));

        produto.LimparEventos();

        Assert.Empty(produto.Eventos);
    }

    [Fact]
    public void Criar_SemRestaurante_DeveLancarDomainException()
    {
        Assert.Throws<DomainException>(() =>
            Produto.Criar(Guid.Empty, "Arroz", null, Dinheiro.Criar(10m), DadosTeste.CriarCategoria(), CriadoEm));
    }

    [Fact]
    public void GarantirQuePodeSerExcluido_ComMovimentacoes_DeveLancarDomainException()
    {
        var produto = CriarProdutoComEstoque(5);

        var excecao = Assert.Throws<DomainException>(() => produto.GarantirQuePodeSerExcluido(possuiMovimentacoes: true));

        Assert.Equal("Não é possível excluir um produto que possui movimentações de estoque vinculadas.", excecao.Message);
    }

    [Fact]
    public void GarantirQuePodeSerExcluido_SemMovimentacoes_NaoDeveLancarExcecao()
    {
        var produto = CriarProdutoComEstoque(5);

        var excecao = Record.Exception(() => produto.GarantirQuePodeSerExcluido(possuiMovimentacoes: false));

        Assert.Null(excecao);
    }
}
