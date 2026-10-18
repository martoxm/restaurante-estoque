using MediatR;
using RestauranteEstoque.Application.Abstractions.Repositories;
using RestauranteEstoque.Application.Abstractions.Services;
using RestauranteEstoque.Application.Common.Exceptions;
using RestauranteEstoque.Domain.Services;
using RestauranteEstoque.Domain.ValueObjects;

namespace RestauranteEstoque.Application.UseCases.MovimentacoesEstoque;

public class CriarMovimentacaoEstoqueHandler : IRequestHandler<CriarMovimentacaoEstoqueCommand, Guid>
{
    private readonly IMovimentacaoEstoqueRepository _movimentacaoEstoqueRepository;
    private readonly IProdutoRepository _produtoRepository;
    private readonly IApplicationDbContext _context;
    private readonly IRelogio _relogio;

    public CriarMovimentacaoEstoqueHandler(
        IMovimentacaoEstoqueRepository movimentacaoEstoqueRepository,
        IProdutoRepository produtoRepository,
        IApplicationDbContext context,
        IRelogio relogio)
    {
        _movimentacaoEstoqueRepository = movimentacaoEstoqueRepository;
        _produtoRepository = produtoRepository;
        _context = context;
        _relogio = relogio;
    }

    public async Task<Guid> Handle(CriarMovimentacaoEstoqueCommand request, CancellationToken cancellationToken)
    {
        var produto = await _produtoRepository.ObterPorIdAsync(request.ProdutoId, cancellationToken);

        if (produto is null)
            throw new NaoEncontradoException($"Produto com Id '{request.ProdutoId}' não foi encontrado.");

        var quantidade = Quantidade.Criar(request.Quantidade);

        var movimentacao = MovimentadorEstoque.Movimentar(
            produto,
            request.Tipo,
            quantidade,
            _relogio.AgoraUtc,
            request.Observacoes);

        _produtoRepository.Atualizar(produto);
        _movimentacaoEstoqueRepository.Adicionar(movimentacao);
        await _context.SaveChangesAsync(cancellationToken);

        return movimentacao.Id;
    }
}
