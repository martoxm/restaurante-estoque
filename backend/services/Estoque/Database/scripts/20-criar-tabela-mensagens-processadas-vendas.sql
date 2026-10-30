-- Etapa M8c: a tabela da idempotência da Vendas. Cada resposta que o
-- consumidor da resultado-baixa-estoque-queue processa deixa o MessageId aqui,
-- na mesma transação da confirmação. Se a mesma mensagem chegar de novo, o Id já existe e ela é
-- ignorada; e a PK barra até duas entregas ao mesmo tempo.
--
-- Sem USE no topo (igual o 15 a 19). Pode rodar mais de uma vez (idempotente).

SET QUOTED_IDENTIFIER ON;

IF DB_NAME() = 'master'
BEGIN
    RAISERROR('Você está no banco master. Selecione o RestauranteEstoqueDb na janela e rode de novo.', 16, 1);
    SET NOEXEC ON;
END;
GO

IF OBJECT_ID('vendas.MensagensProcessadas', 'U') IS NULL
BEGIN
    CREATE TABLE vendas.MensagensProcessadas
    (
        Id           UNIQUEIDENTIFIER NOT NULL CONSTRAINT PK_MensagensProcessadas_Vendas PRIMARY KEY,
        Fila         NVARCHAR(100)    NOT NULL,
        ProcessadoEm DATETIME2        NOT NULL
    );
END;
GO

SET NOEXEC OFF;
