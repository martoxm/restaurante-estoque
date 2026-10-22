-- =============================================================
-- Etapa 10b — Aqui a gente cria na mão a tabela Suppliers (os Fornecedores).
--
-- Pra rodar no SQL Server Management Studio:
--   1. Conecta no seu servidor local, dentro do banco RestauranteEstoqueDb
--      (é o mesmo que a gente criou no script 01-criar-tabelas.sql).
--   2. Clica em "Nova Consulta" já com esse banco selecionado.
--   3. Cola este arquivo inteiro e aperta Executar (F5), uma vez só.
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

-- E aqui a gente confere se a tabela apareceu mesmo
SELECT name AS TabelaCriada FROM sys.tables ORDER BY name;
GO
