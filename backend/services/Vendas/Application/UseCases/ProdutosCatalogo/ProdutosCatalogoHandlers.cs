using MediatR;
using RestauranteEstoque.Core.Application.Abstractions;
using RestauranteEstoque.Core.Domain.ValueObjects;
using RestauranteEstoque.Vendas.Application.Abstractions.Repositories;
using RestauranteEstoque.Vendas.Application.Abstractions.Services;
using RestauranteEstoque.Vendas.Domain.Entities;

namespace RestauranteEstoque.Vendas.Application.UseCases.ProdutosCatalogo;

public class SincronizarProdutoCatalogoHandler : IRequestHandler<SincronizarProdutoCatalogoCommand>
{
    private readonly IProdutoCatalogoRepository _catalogo;
    private readonly IDefinidorRestauranteAtual _definidorRestaurante;
    private readonly IApplicationDbContext _context;

    public SincronizarProdutoCatalogoHandler(
        IProdutoCatalogoRepository catalogo,
        IDefinidorRestauranteAtual definidorRestaurante,
        IApplicationDbContext context)
    {
        _catalogo = catalogo;
        _definidorRestaurante = definidorRestaurante;
        _context = context;
    }

    public async Task Handle(SincronizarProdutoCatalogoCommand request, CancellationToken cancellationToken)
    {
        var mensagem = request.Produto;

        _definidorRestaurante.Definir(mensagem.RestauranteId);

        var preco = Dinheiro.Criar(mensagem.Preco);
        var produto = await _catalogo.ObterPorIdAsync(mensagem.ProdutoId, cancellationToken);

        if (produto is null)
        {
            _catalogo.Adicionar(ProdutoCatalogo.Criar(mensagem.ProdutoId, mensagem.RestauranteId, mensagem.Nome, preco, mensagem.Excluido, mensagem.OcorridoEm));
        }
        else if (produto.AplicarSeForMaisNovo(mensagem.Nome, preco, mensagem.Excluido, mensagem.OcorridoEm))
        {
            _catalogo.Atualizar(produto);
        }
        else
        {
            return;
        }

        await _context.SaveChangesAsync(cancellationToken);
    }
}
