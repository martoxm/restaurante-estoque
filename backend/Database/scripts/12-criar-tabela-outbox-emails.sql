-- =============================================================
-- Etapa 41 — Criação manual da tabela OutboxEmails
--
-- "Caixa de saída" dos e-mails de aviso (ex.: estoque baixo). A linha é
-- gravada na mesma transação da movimentação de estoque, e o
-- EnvioEmailOutboxWorker (Api) manda o e-mail depois, em segundo plano.
--
-- Como rodar no SQL Server Management Studio:
--   1. Conecte no seu servidor local, banco RestauranteEstoqueDb.
--   2. Rode DEPOIS do 11-traduzir-banco-pt-br.sql (a FK aponta para a
--      tabela Restaurantes, com o nome já traduzido).
--   3. Com a API parada, cole este arquivo e aperte Executar (F5).
--
-- Pode rodar de novo sem problema: só cria o que ainda não existe.
-- =============================================================

USE RestauranteEstoqueDb;
GO

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'OutboxEmails')
BEGIN
    CREATE TABLE OutboxEmails
    (
        Id                 UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
        RestauranteId      UNIQUEIDENTIFIER NOT NULL,
        Assunto            NVARCHAR(200)    NOT NULL,
        CorpoHtml          NVARCHAR(MAX)    NOT NULL,
        Tentativas         INT              NOT NULL DEFAULT 0,
        CriadoEm           DATETIME2        NOT NULL,
        ProximaTentativaEm DATETIME2        NOT NULL,
        ProcessadoEm       DATETIME2        NULL,      -- NULL = ainda não enviado
        UltimoErro         NVARCHAR(2000)   NULL,

        CONSTRAINT FK_OutboxEmails_Restaurantes
            FOREIGN KEY (RestauranteId) REFERENCES Restaurantes (Id)
    );
END
GO

-- O worker busca, a cada 15 segundos, "o que ainda não foi enviado e já
-- passou da hora de tentar". Este índice deixa essa busca rápida mesmo com
-- muitas linhas antigas já enviadas na tabela.
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_OutboxEmails_Pendentes')
BEGIN
    CREATE INDEX IX_OutboxEmails_Pendentes
        ON OutboxEmails (ProcessadoEm, ProximaTentativaEm);
END
GO

-- Conferir o resultado
SELECT name AS TabelaCriada FROM sys.tables WHERE name = 'OutboxEmails';
GO
