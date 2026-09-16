-- =============================================================
-- Etapa 10b — Criação manual da tabela Suppliers (Fornecedores)
--
-- Como rodar no SQL Server Management Studio:
--   1. Conecte no seu servidor local, dentro do banco RestauranteEstoqueDb
--      (o mesmo criado no script 01-criar-tabelas.sql).
--   2. Clique em "Nova Consulta" já com esse banco selecionado.
--   3. Cole este arquivo inteiro e aperte Executar (F5), uma vez só.
-- =============================================================

USE RestauranteEstoqueDb;
GO

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Suppliers')
BEGIN
    CREATE TABLE Suppliers
    (
        Id    UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
        Name  NVARCHAR(150)    NOT NULL,
        Phone NVARCHAR(20)     NULL,
        Email NVARCHAR(200)    NULL
    );
END
GO

-- Conferir o resultado
SELECT name AS TabelaCriada FROM sys.tables ORDER BY name;
GO
