using Microsoft.EntityFrameworkCore;
using RestauranteEstoque.Application.Abstractions.Repositories;
using RestauranteEstoque.Core.Mensageria;

namespace RestauranteEstoque.Tests.Common;

public class FakeMensagemProcessadaRepository : IMensagemProcessadaRepository
{
    private readonly TestDbContext _context;

    public FakeMensagemProcessadaRepository(TestDbContext context)
    {
        _context = context;
    }

    public Task<bool> ExisteAsync(Guid mensagemId, CancellationToken cancellationToken) =>
        _context.MensagensProcessadas.AnyAsync(mensagem => mensagem.Id == mensagemId, cancellationToken);

    public void Adicionar(MensagemProcessada mensagem) => _context.MensagensProcessadas.Add(mensagem);
}
