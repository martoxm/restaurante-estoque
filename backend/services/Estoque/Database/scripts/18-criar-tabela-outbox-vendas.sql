-- Etapa M8a: o Outbox da API de Vendas. Mesma tabela do estoque.OutboxMensagens
-- (script 16), só que no schema "vendas": cada API tem o seu Outbox. É aqui que
-- a venda grava o pedido de "baixar estoque", na mesma transação da venda.
--
-- Sem USE no topo (igual o 15, 16 e 17): abre a janela já conectado no banco
-- certo. Pode rodar mais de uma vez (idempotente).

SET QUOTED_IDENTIFIER ON;

-- Mesma trava do script 17: sem USE, o padrão do SSMS é o master.
IF DB_NAME() = 'master'
BEGIN
    RAISERROR('Você está no banco master. Selecione o RestauranteEstoqueDb na janela e rode de novo.', 16, 1);
    SET NOEXEC ON;
END;
GO

BEGIN TRANSACTION;

IF OBJECT_ID('vendas.OutboxMensagens', 'U') IS NULL
BEGIN
    CREATE TABLE vendas.OutboxMensagens
    (
        Id                 UNIQUEIDENTIFIER NOT NULL CONSTRAINT PK_OutboxMensagens_Vendas PRIMARY KEY,
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
        ON vendas.OutboxMensagens (ProximaTentativaEm)
        WHERE ProcessadoEm IS NULL;
END;

COMMIT TRANSACTION;
GO

SET NOEXEC OFF;
