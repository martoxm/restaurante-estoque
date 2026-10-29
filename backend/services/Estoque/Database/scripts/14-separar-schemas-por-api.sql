-- =============================================================
-- Etapa M3b — Aqui cada API passa a ter o seu schema no banco.
--
-- O banco continua sendo UM só (decisão do roadmap de microserviços), mas
-- agora as tabelas ficam separadas por dono:
--   identidade.  -> AspNet* (usuários, roles) e Restaurantes  (API de Identidade)
--   estoque.     -> Produtos, Categorias, Fornecedores,
--                   MovimentacoesEstoque e OutboxEmails        (API de Estoque)
--
-- E repara numa regra nova: não existe mais FK de uma API pra outra. O Estoque
-- continua guardando o RestauranteId em cada linha, mas o banco não confere se
-- o restaurante existe — quem garante isso é o token (só a Identidade gera
-- token, e o RestauranteId vem dentro dele). É o preço de cada API ser dona dos
-- seus dados: se amanhã a Identidade for pra outro banco, o Estoque nem percebe.
-- A FK de AspNetUsers -> Restaurantes CONTINUA, porque as duas são da Identidade.
--
-- Pra rodar no SQL Server Management Studio (ou no Query editor do Azure):
--   1. Roda DEPOIS do 13.
--   2. Com as DUAS APIs paradas, cola este arquivo e aperta Executar (F5).
--   3. Depois disso, só suba as APIs com o código da Etapa M3b em diante
--      (o código antigo procura as tabelas no dbo e não acha mais nada).
--
-- Pode rodar de novo sem medo: tabela que já mudou de schema é pulada. E vai
-- tudo numa transação: se algo falhar no meio, volta tudo como estava.
--
-- O ALTER SCHEMA ... TRANSFER só muda a tabela de "pasta": os dados, os
-- índices e as FKs que ficam vão junto, sem copiar nada.
-- =============================================================

SET XACT_ABORT ON;   -- assim, qualquer erro desfaz a transação inteira
GO

BEGIN TRANSACTION;

-- 1) Os dois schemas novos. CREATE SCHEMA tem que ser o único comando do lote,
--    por isso ele vai dentro de um EXEC.
IF SCHEMA_ID('identidade') IS NULL EXEC('CREATE SCHEMA identidade');
IF SCHEMA_ID('estoque')    IS NULL EXEC('CREATE SCHEMA estoque');

-- 2) Tira as FKs de RestauranteId das tabelas do Estoque (as que apontam pra
--    Restaurantes). A gente procura pelo catálogo do banco em vez de escrever o
--    nome de cada FK, porque os nomes mudaram ao longo dos scripts 08, 11, 12 e 13.
DECLARE @sql NVARCHAR(MAX) = N'';

SELECT @sql += N'ALTER TABLE ' + QUOTENAME(SCHEMA_NAME(t.schema_id)) + N'.' + QUOTENAME(t.name)
             + N' DROP CONSTRAINT ' + QUOTENAME(fk.name) + N';' + CHAR(10)
FROM sys.foreign_keys fk
JOIN sys.tables t ON t.object_id = fk.parent_object_id
WHERE fk.referenced_object_id IN (OBJECT_ID('dbo.Restaurantes'), OBJECT_ID('identidade.Restaurantes'))
  AND t.name IN ('Produtos', 'Categorias', 'Fornecedores', 'MovimentacoesEstoque', 'OutboxEmails');

PRINT @sql;
EXEC sp_executesql @sql;

-- 3) O que é da Identidade. A __EFMigrationsHistory vai junto porque a
--    Identidade é a única API com migration (a das tabelas AspNet*).
IF OBJECT_ID('dbo.AspNetUsers')           IS NOT NULL ALTER SCHEMA identidade TRANSFER dbo.AspNetUsers;
IF OBJECT_ID('dbo.AspNetRoles')           IS NOT NULL ALTER SCHEMA identidade TRANSFER dbo.AspNetRoles;
IF OBJECT_ID('dbo.AspNetUserRoles')       IS NOT NULL ALTER SCHEMA identidade TRANSFER dbo.AspNetUserRoles;
IF OBJECT_ID('dbo.AspNetUserClaims')      IS NOT NULL ALTER SCHEMA identidade TRANSFER dbo.AspNetUserClaims;
IF OBJECT_ID('dbo.AspNetUserLogins')      IS NOT NULL ALTER SCHEMA identidade TRANSFER dbo.AspNetUserLogins;
IF OBJECT_ID('dbo.AspNetUserTokens')      IS NOT NULL ALTER SCHEMA identidade TRANSFER dbo.AspNetUserTokens;
IF OBJECT_ID('dbo.AspNetRoleClaims')      IS NOT NULL ALTER SCHEMA identidade TRANSFER dbo.AspNetRoleClaims;
IF OBJECT_ID('dbo.Restaurantes')          IS NOT NULL ALTER SCHEMA identidade TRANSFER dbo.Restaurantes;
IF OBJECT_ID('dbo.__EFMigrationsHistory') IS NOT NULL AND OBJECT_ID('identidade.__EFMigrationsHistory') IS NULL
    ALTER SCHEMA identidade TRANSFER dbo.__EFMigrationsHistory;

-- 4) O que é do Estoque.
IF OBJECT_ID('dbo.Produtos')             IS NOT NULL ALTER SCHEMA estoque TRANSFER dbo.Produtos;
IF OBJECT_ID('dbo.Categorias')           IS NOT NULL ALTER SCHEMA estoque TRANSFER dbo.Categorias;
IF OBJECT_ID('dbo.Fornecedores')         IS NOT NULL ALTER SCHEMA estoque TRANSFER dbo.Fornecedores;
IF OBJECT_ID('dbo.MovimentacoesEstoque') IS NOT NULL ALTER SCHEMA estoque TRANSFER dbo.MovimentacoesEstoque;
IF OBJECT_ID('dbo.OutboxEmails')         IS NOT NULL ALTER SCHEMA estoque TRANSFER dbo.OutboxEmails;

COMMIT TRANSACTION;
GO

-- 5) Conferência: tem que aparecer cada tabela no schema certo, e nada no dbo.
SELECT s.name AS SchemaDono, t.name AS Tabela
FROM sys.tables t
JOIN sys.schemas s ON s.schema_id = t.schema_id
ORDER BY s.name, t.name;
GO
