-- =============================================================
-- Etapa M4a — Aqui a gente cria na mão a tabela identidade.RefreshTokens.
--
-- Cada login (e cada renovação) grava uma linha aqui. Repara que a tabela não
-- guarda o refresh token em si, só o HASH SHA-256 dele (64 caracteres em
-- hexadecimal): se alguém copiar o banco, não consegue usar nenhuma sessão.
--
-- É da API de Identidade, então fica no schema "identidade", e a FK pra
-- AspNetUsers pode existir (as duas tabelas são da mesma API). O ON DELETE
-- CASCADE faz as sessões sumirem junto quando um usuário é apagado.
--
-- Pra rodar no SSMS (ou no Query editor do Azure):
--   1. Roda DEPOIS do 14 (o schema "identidade" nasce lá).
--   2. Pode rodar com as APIs ligadas: é só uma tabela nova.
--
-- Se rodar de novo, não tem problema: ele só cria o que ainda não existe.
-- (Sem "USE": no Azure SQL cada banco é isolado e o USE nem funciona —
-- conecta direto no banco certo antes de rodar.)
-- =============================================================

IF NOT EXISTS (
    SELECT 1 FROM sys.tables t
    JOIN sys.schemas s ON s.schema_id = t.schema_id
    WHERE s.name = 'identidade' AND t.name = 'RefreshTokens')
BEGIN
    CREATE TABLE identidade.RefreshTokens
    (
        Id          UNIQUEIDENTIFIER NOT NULL CONSTRAINT PK_RefreshTokens PRIMARY KEY,
        UsuarioId   UNIQUEIDENTIFIER NOT NULL,
        TokenHash   NVARCHAR(64)     NOT NULL,
        CriadoEm    DATETIME2        NOT NULL,
        ExpiraEm    DATETIME2        NOT NULL,
        RevogadoEm  DATETIME2        NULL,

        CONSTRAINT FK_RefreshTokens_AspNetUsers_UsuarioId
            FOREIGN KEY (UsuarioId) REFERENCES identidade.AspNetUsers (Id) ON DELETE CASCADE
    );

    -- A busca do /auth/renovar é sempre pelo hash; o UNIQUE também garante que
    -- dois tokens iguais nunca existam.
    CREATE UNIQUE INDEX IX_RefreshTokens_TokenHash ON identidade.RefreshTokens (TokenHash);

    -- Pra derrubar todas as sessões de um usuário (detecção de roubo).
    CREATE INDEX IX_RefreshTokens_UsuarioId ON identidade.RefreshTokens (UsuarioId);

    PRINT 'Tabela identidade.RefreshTokens criada.';
END
ELSE
    PRINT 'Tabela identidade.RefreshTokens já existe, nada a fazer.';
GO
