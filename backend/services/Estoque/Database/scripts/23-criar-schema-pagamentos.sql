-- Etapa M13a: o schema da API de Pagamentos, com a tabela de pagamentos.
--
-- Mesma regra das outras APIs: schema próprio ("pagamentos"), sem FK pra
-- Vendas nem pra Restaurantes (eles moram em outras APIs).
--
-- Repara no índice ÚNICO de (RestauranteId, ChaveIdempotencia): é a trava final
-- da idempotência. Se dois pedidos com o mesmo Idempotency-Key chegarem no
-- mesmo instante, os dois passam pela consulta do Handler, mas o segundo INSERT
-- bate nesse índice e falha — e o cliente nunca é cobrado duas vezes.
--
-- E repara no que NÃO tem: coluna de número de cartão. Só o token do gateway.
--
-- Sem USE no topo (igual o 15 a 22). Pode rodar mais de uma vez (idempotente).

SET QUOTED_IDENTIFIER ON;

IF DB_NAME() = 'master'
BEGIN
    RAISERROR('Você está no banco master. Selecione o RestauranteEstoqueDb na janela e rode de novo.', 16, 1);
    SET NOEXEC ON;
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.schemas WHERE name = 'pagamentos')
    EXEC('CREATE SCHEMA pagamentos');
GO

IF OBJECT_ID('pagamentos.Pagamentos') IS NULL
BEGIN
    CREATE TABLE pagamentos.Pagamentos (
        Id                 UNIQUEIDENTIFIER NOT NULL CONSTRAINT PK_Pagamentos PRIMARY KEY,
        RestauranteId      UNIQUEIDENTIFIER NOT NULL,
        VendaId            UNIQUEIDENTIFIER NOT NULL,
        Valor              DECIMAL(18, 2)   NOT NULL,
        TokenCartao        NVARCHAR(100)    NOT NULL,
        ChaveIdempotencia  NVARCHAR(255)    NOT NULL,
        Status             NVARCHAR(20)     NOT NULL,
        CriadoEm           DATETIME2        NOT NULL,
        ProcessadoEm       DATETIME2        NULL,
        IdTransacaoGateway NVARCHAR(100)    NULL,
        MotivoRecusa       NVARCHAR(300)    NULL
    );
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'UX_Pagamentos_RestauranteId_ChaveIdempotencia' AND object_id = OBJECT_ID('pagamentos.Pagamentos'))
    CREATE UNIQUE INDEX UX_Pagamentos_RestauranteId_ChaveIdempotencia
        ON pagamentos.Pagamentos (RestauranteId, ChaveIdempotencia);

-- O worker procura os Pendentes a cada 5 s. O filtro (WHERE) deixa o índice
-- pequenininho: só guarda as linhas que ainda estão esperando o gateway.
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Pagamentos_Pendentes' AND object_id = OBJECT_ID('pagamentos.Pagamentos'))
    CREATE INDEX IX_Pagamentos_Pendentes
        ON pagamentos.Pagamentos (CriadoEm)
        WHERE Status = 'Pendente';

-- Pra achar o pagamento de uma venda (a saga da M13b vai precisar).
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Pagamentos_VendaId' AND object_id = OBJECT_ID('pagamentos.Pagamentos'))
    CREATE INDEX IX_Pagamentos_VendaId ON pagamentos.Pagamentos (VendaId);
GO
