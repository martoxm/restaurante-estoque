using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RestauranteEstoque.Application.UseCases.MovimentacoesEstoque;
using RestauranteEstoque.Core.Domain;
using RestauranteEstoque.Domain.Entities;
using RestauranteEstoque.Domain.Enums;
using RestauranteEstoque.Core.Domain.ValueObjects;
using RestauranteEstoque.Tests.Common;
using RestauranteEstoque.Tests.Integration.Fixtures;
using System.Text.Json;
using RestauranteEstoque.Core.Mensageria.Contratos;

namespace RestauranteEstoque.Tests.Integration.Fluxos;

[Collection(nameof(SqlServerCollection))]
public sealed class OutboxEstoqueBaixoTests(SqlServerFixture fixture)
{
    private static readonly DateTime CriadoEm = new(2026, 1, 1, 12, 0, 0, DateTimeKind.Utc);

    [DockerFact]
    public async Task SaidaQueDeixaOEstoqueBaixo_DeveGravarOAvisoNoOutboxJuntoComAMovimentacao()
    {
        if (!fixture.EstaDisponivel)
            return;

        var (restauranteId, produto) = await CriarProdutoComEstoqueAsync(12);

        await EnviarAsync(new CriarMovimentacaoEstoqueCommand(produto.Id, TipoMovimentacaoEstoque.Saida, 5), restauranteId);

        await using var contexto = fixture.CriarContexto(UsuarioDo(restauranteId));
        var produtoAtualizado = await contexto.Produtos.SingleAsync(p => p.Id == produto.Id);
        var avisos = await contexto.OutboxMensagens.Where(o => o.Conteudo.Contains(restauranteId.ToString())).ToListAsync();

        Assert.Equal(Quantidade.Criar(7), produtoAtualizado.QuantidadeEmEstoque);
        Assert.Equal(1, await contexto.MovimentacoesEstoque.CountAsync(m => m.ProdutoId == produto.Id));
        var aviso = Assert.Single(avisos);
        Assert.Contains(produto.Nome, JsonSerializer.Deserialize<EnviarEmailMensagem>(aviso.Conteudo)!.Assunto);
        Assert.Null(aviso.ProcessadoEm);
    }

    [DockerFact]
    public async Task SaidaQueNaoDeixaOEstoqueBaixo_NaoDeveGravarNadaNoOutbox()
    {
        if (!fixture.EstaDisponivel)
            return;

        var (restauranteId, produto) = await CriarProdutoComEstoqueAsync(30);

        await EnviarAsync(new CriarMovimentacaoEstoqueCommand(produto.Id, TipoMovimentacaoEstoque.Saida, 5), restauranteId);

        await using var contexto = fixture.CriarContexto(UsuarioDo(restauranteId));
        Assert.False(await contexto.OutboxMensagens.AnyAsync(o => o.Conteudo.Contains(restauranteId.ToString())));
    }

    [DockerFact]
    public async Task SaidaMaiorQueOEstoque_NaoDeveGravarNada()
    {
        if (!fixture.EstaDisponivel)
            return;

        var (restauranteId, produto) = await CriarProdutoComEstoqueAsync(3);

        await Assert.ThrowsAsync<DomainException>(() =>
            EnviarAsync(new CriarMovimentacaoEstoqueCommand(produto.Id, TipoMovimentacaoEstoque.Saida, 5), restauranteId));

        await using var contexto = fixture.CriarContexto(UsuarioDo(restauranteId));
        var produtoNoBanco = await contexto.Produtos.SingleAsync(p => p.Id == produto.Id);
        Assert.Equal(Quantidade.Criar(3), produtoNoBanco.QuantidadeEmEstoque);
        Assert.False(await contexto.MovimentacoesEstoque.AnyAsync(m => m.ProdutoId == produto.Id));
        Assert.False(await contexto.OutboxMensagens.AnyAsync(o => o.Conteudo.Contains(restauranteId.ToString())));
    }

    private async Task<T> EnviarAsync<T>(IRequest<T> comando, Guid? restauranteId = null)
    {
        await using var servicos = fixture.CriarServicos(restauranteId is null ? null : UsuarioDo(restauranteId.Value));
        using var escopo = servicos.CreateScope();
        var sender = escopo.ServiceProvider.GetRequiredService<ISender>();

        return await sender.Send(comando, CancellationToken.None);
    }

    private async Task<(Guid RestauranteId, Produto Produto)> CriarProdutoComEstoqueAsync(int quantidade)
    {
        var restauranteId = Guid.NewGuid();
        var categoria = DadosTeste.CriarCategoria(restauranteId);
        var produto = Produto.Criar(restauranteId, "Arroz", null, Dinheiro.Criar(10m), categoria, CriadoEm, Quantidade.Criar(quantidade));

        await using var contexto = fixture.CriarContexto(UsuarioDo(restauranteId));
        contexto.Categorias.Add(categoria);
        contexto.Produtos.Add(produto);
        await contexto.SaveChangesAsync();

        return (restauranteId, produto);
    }

    private static FakeUsuarioAtualService UsuarioDo(Guid restauranteId) =>
        new() { RestauranteId = restauranteId };
}
