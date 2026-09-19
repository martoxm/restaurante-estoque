-- =============================================================
-- Etapa 25 — Remover a tabela "Users" antiga (substituída por AspNetUsers)
--
-- Desde a Etapa 23/24, a autenticação passou a usar o ASP.NET Core Identity
-- (tabelas AspNetUsers/AspNetRoles/etc., criadas via EF Core Migration).
-- A tabela "Users" original (criada na mão pelo script 01) não é mais lida
-- nem escrita por ninguém no código — pode ser apagada com segurança.
--
-- Se você já rodou os scripts 02/04 (usuários de teste inseridos na mão
-- via INSERT com hash BCrypt), esses usuários somem junto com a tabela —
-- eles já não serviam mais para logar de qualquer forma, porque o Identity
-- usa um algoritmo de hash diferente (PBKDF2, não BCrypt).
--
-- Como rodar: conecte no seu SQL Server, abra uma "Nova Consulta" no banco
-- RestauranteEstoqueDb e rode este arquivo (F5).
-- =============================================================

USE RestauranteEstoqueDb;
GO

IF EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Users')
BEGIN
    DROP TABLE Users;
END
GO

-- Conferir o resultado: "Users" não deve mais aparecer na lista,
-- mas "AspNetUsers" (criada pela migration da Etapa 23) sim.
SELECT name AS Tabela FROM sys.tables ORDER BY name;
GO
