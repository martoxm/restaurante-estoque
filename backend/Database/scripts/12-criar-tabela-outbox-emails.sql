-- =============================================================
-- Etapa 41 — Aqui a gente cria na mão a tabela OutboxEmails.
--
-- Pensa nela como a "caixa de saída" dos e-mails de aviso (ex.: estoque
-- baixo). A linha é gravada na mesma transação da movimentação de estoque,
-- e quem manda o e-mail de verdade é o EnvioEmailOutboxWorker (na Api),
-- depois, em segundo plano.
--
-- Pra rodar no SQL Server Management Studio:
--   1. Conecta no seu servidor local, no banco RestauranteEstoqueDb.
--   2. Roda DEPOIS do 11-traduzir-banco-pt-br.sql, porque a FK aponta pra
--      tabela Restaurantes, com o nome já traduzido.
--   3. Com a API parada, cola este arquivo e aperta Executar (F5).
--
-- Se rodar de novo, não tem problema: ele só cria o que ainda não existe.
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
        ProcessadoEm       DATETIME2        NULL,      -- se tá NULL, é porque ainda não foi enviado
        UltimoErro         NVARCHAR(2000)   NULL,

        CONSTRAINT FK_OutboxEmails_Restaurantes
            FOREIGN KEY (RestauranteId) REFERENCES Restaurantes (Id)
    );
END
GO

-- A cada 15 segundos, o worker procura "o que ainda não foi enviado e já
-- passou da hora de tentar". Esse índice existe pra essa busca continuar
-- rápida mesmo quando a tabela tiver um monte de linha antiga já enviada.
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_OutboxEmails_Pendentes')
BEGIN
    CREATE INDEX IX_OutboxEmails_Pendentes
        ON OutboxEmails (ProcessadoEm, ProximaTentativaEm);
END
GO

-- E aqui a gente confere se a tabela apareceu mesmo
SELECT name AS TabelaCriada FROM sys.tables WHERE name = 'OutboxEmails';
GO
