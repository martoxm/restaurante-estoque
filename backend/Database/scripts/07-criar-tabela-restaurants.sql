-- =============================================================
-- Etapa 28 — Criação manual da tabela Restaurants (fundação multi-tenant)
--
-- Como rodar no SQL Server Management Studio:
--   1. Conecte no seu servidor local, dentro do banco RestauranteEstoqueDb
--      (o mesmo criado no script 01-criar-tabelas.sql).
--   2. Clique em "Nova Consulta" já com esse banco selecionado.
--   3. Cole este arquivo inteiro e aperte Executar (F5), uma vez só.
--
-- Rode este script ANTES do 08-adicionar-restaurantid.sql — a tabela
-- Restaurants precisa existir antes das outras tabelas ganharem uma FK
-- apontando pra ela.
-- =============================================================

USE RestauranteEstoqueDb;
GO

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Restaurants')
BEGIN
    CREATE TABLE Restaurants
    (
        Id   UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
        Name NVARCHAR(150)    NOT NULL
    );
END
GO

-- Conferir o resultado
SELECT name AS TabelaCriada FROM sys.tables ORDER BY name;
GO
