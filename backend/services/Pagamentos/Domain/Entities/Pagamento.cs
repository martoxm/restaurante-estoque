using RestauranteEstoque.Core.Domain;
using RestauranteEstoque.Core.Domain.ValueObjects;
using RestauranteEstoque.Pagamentos.Domain.Enums;

namespace RestauranteEstoque.Pagamentos.Domain.Entities;

public class Pagamento : Entity
{
    public Guid RestauranteId { get; private set; }
    public Guid VendaId { get; private set; }
    public Dinheiro Valor { get; private set; } = null!;
    public string TokenCartao { get; private set; } = null!;

    public string ChaveIdempotencia { get; private set; } = null!;

    public StatusPagamento Status { get; private set; }
    public DateTime CriadoEm { get; private set; }
    public DateTime? ProcessadoEm { get; private set; }

    public string? IdTransacaoGateway { get; private set; }
    public string? MotivoRecusa { get; private set; }

    public int Tentativas { get; private set; }
    public const int MaximoTentativas = 5;

    private Pagamento()
    {
    }

    public static Pagamento Criar(Guid restauranteId, Guid vendaId, Dinheiro valor, string tokenCartao, string chaveIdempotencia, DateTime criadoEm)
    {
        if (restauranteId == Guid.Empty)
            throw new DomainException("O pagamento precisa pertencer a um restaurante.");

        if (vendaId == Guid.Empty)
            throw new DomainException("O pagamento precisa ser de uma venda.");

        if (valor.Valor <= 0)
            throw new DomainException("O valor do pagamento precisa ser maior que zero.");

        if (string.IsNullOrWhiteSpace(tokenCartao))
            throw new DomainException("Informe o token do cartão.");

        if (string.IsNullOrWhiteSpace(chaveIdempotencia))
            throw new DomainException("Informe a chave de idempotência.");

        return new Pagamento
        {
            RestauranteId = restauranteId,
            VendaId = vendaId,
            Valor = valor,
            TokenCartao = tokenCartao.Trim(),
            ChaveIdempotencia = chaveIdempotencia.Trim(),
            Status = StatusPagamento.Pendente,
            CriadoEm = criadoEm
        };
    }

    public bool EhMesmoPedido(Guid vendaId, decimal valor, string tokenCartao) =>
        VendaId == vendaId && Valor.Valor == valor && TokenCartao == tokenCartao.Trim();

    public void MarcarComoProcessando(string idTransacaoGateway)
    {
        GarantirStatus(StatusPagamento.Pendente);
        IdTransacaoGateway = idTransacaoGateway;
        Status = StatusPagamento.Processando;
    }

    public void Aprovar(DateTime processadoEm)
    {
        GarantirStatus(StatusPagamento.Processando);
        Status = StatusPagamento.Aprovado;
        ProcessadoEm = processadoEm;
    }

    public void Recusar(string motivo, DateTime processadoEm)
    {
        GarantirStatus(StatusPagamento.Pendente, StatusPagamento.Processando);
        MotivoRecusa = motivo;
        Status = StatusPagamento.Recusado;
        ProcessadoEm = processadoEm;
    }

    public bool RegistrarFalhaGateway(DateTime agora)
    {
        GarantirStatus(StatusPagamento.Pendente);
        Tentativas++;

        if (Tentativas < MaximoTentativas)
            return false;

        Recusar($"Gateway indisponível depois de {MaximoTentativas} tentativas.", agora);
        return true;
    }

    private void GarantirStatus(params StatusPagamento[] permitidos)
    {
        if (!permitidos.Contains(Status))
            throw new DomainException($"O pagamento não pode mudar de status agora (está {Status}).");
    }
}
