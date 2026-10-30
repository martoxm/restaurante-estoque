using RestauranteEstoque.Core.Domain;

namespace RestauranteEstoque.Core.Mensageria;

public class MensagemProcessada : Entity
{
    public string Fila { get; private set; } = string.Empty;
    public DateTime ProcessadoEm { get; private set; }

    private MensagemProcessada()
    {
    }

    public static MensagemProcessada Criar(Guid mensagemId, string fila, DateTime processadoEm)
    {
        if (mensagemId == Guid.Empty)
            throw new DomainException("O Id da mensagem é obrigatório.");

        return new MensagemProcessada { Id = mensagemId, Fila = fila, ProcessadoEm = processadoEm };
    }
}
