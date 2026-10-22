-- =============================================================
-- Etapa 28 — Aqui a gente cria na mão a tabela Restaurants. Ela é a base
-- do multi-tenant.
--
-- Pra rodar no SQL Server Management Studio:
--   1. Conecta no seu servidor local, dentro do banco RestauranteEstoqueDb
--      (o mesmo que a gente criou no script 01-criar-tabelas.sql).
--   2. Clica em "Nova Consulta" já com esse banco selecionado.
--   3. Cola este arquivo inteiro e aperta Executar (F5), uma vez só.
--
-- Atenção à ordem: roda este script ANTES do 08-adicionar-restaurantid.sql.
-- O motivo é que a tabela Restaurants precisa existir antes das outras
-- tabelas ganharem uma FK apontando pra ela.
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

-- E aqui a gente confere se a tabela apareceu mesmo
SELECT name AS TabelaCriada FROM sys.tables ORDER BY name;
GO
