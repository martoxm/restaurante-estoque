-- Etapa M13b: o pagamento entra na saga da venda. Três mudanças, em duas APIs:
--
-- 1. vendas.Vendas ganha TokenCartao: o token do gateway que o cliente manda ao
--    criar a venda (nunca o número do cartão). Nulo nas vendas antigas.
-- 2. pagamentos.Pagamentos ganha Tentativas: quantas vezes o gateway não
--    respondeu. Na 5ª a API desiste e recusa, pra a venda não esperar pra sempre.
--    O DEFAULT 0 já preenche as linhas que existem.
-- 3. pagamentos.OutboxMensagens: a Pagamentos agora responde pela fila, então
--    ganhou o Outbox (mesma tabela da Vendas no script 18).
--
-- Sem USE no topo (igual o 15 a 23). Pode rodar mais de uma vez (idempotente).

SET QUOTED_IDENTIFIER ON;

IF DB_NAME() = 'master'
BEGIN
    RAISERROR('Você está no banco master. Selecione o RestauranteEstoqueDb na janela e rode de novo.', 16, 1);
    SET NOEXEC ON;
END;
GO

IF COL_LENGTH('vendas.Vendas', 'TokenCartao') IS NULL
    ALTER TABLE vendas.Vendas ADD TokenCartao NVARCHAR(100) NULL;
GO

IF COL_LENGTH('pagamentos.Pagamentos', 'Tentativas') IS NULL
    ALTER TABLE pagamentos.Pagamentos ADD Tentativas INT NOT NULL
        CONSTRAINT DF_Pagamentos_Tentativas DEFAULT 0;
GO

IF OBJECT_ID('pagamentos.OutboxMensagens') IS NULL
BEGIN
    CREATE TABLE pagamentos.OutboxMensagens
    (
        Id                 UNIQUEIDENTIFIER NOT NULL CONSTRAINT PK_OutboxMensagens_Pagamentos PRIMARY KEY,
        Fila               NVARCHAR(100)    NOT NULL,
        Tipo               NVARCHAR(200)    NOT NULL,
        Conteudo           NVARCHAR(MAX)    NOT NULL,
        Tentativas         INT              NOT NULL,
        CriadoEm           DATETIME2        NOT NULL,
        ProximaTentativaEm DATETIME2        NOT NULL,
        ProcessadoEm       DATETIME2        NULL,
        UltimoErro         NVARCHAR(2000)   NULL
    );

    CREATE INDEX IX_OutboxMensagens_Pendentes
        ON pagamentos.OutboxMensagens (ProximaTentativaEm)
        WHERE ProcessadoEm IS NULL;
END;
GO
