-- =============================================================
-- Etapa 25 — Aqui a gente apaga a tabela "Users" antiga (quem ocupou o
-- lugar dela foi a AspNetUsers).
--
-- O que aconteceu foi o seguinte: desde a Etapa 23/24, a autenticação
-- passou a usar o ASP.NET Core Identity (as tabelas AspNetUsers/AspNetRoles/
-- etc., que foram criadas via EF Core Migration). Então a tabela "Users"
-- original (aquela que a gente criou na mão no script 01) não é mais lida
-- nem escrita por ninguém no código — dá pra apagar com segurança.
--
-- Repara numa coisa: se você já rodou os scripts 02/04 (os usuários de teste
-- inseridos na mão via INSERT com hash BCrypt), esses usuários somem junto
-- com a tabela. Mas eles já não serviam pra logar de qualquer jeito, porque
-- o Identity usa outro algoritmo de hash (PBKDF2, não BCrypt).
--
-- Pra rodar: conecta no seu SQL Server, abre uma "Nova Consulta" no banco
-- RestauranteEstoqueDb e roda este arquivo (F5).
-- =============================================================

USE RestauranteEstoqueDb;
GO

IF EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Users')
BEGIN
    DROP TABLE Users;
END
GO

-- Agora a gente confere o resultado: a "Users" não pode mais aparecer na
-- lista, mas a "AspNetUsers" (que a migration da Etapa 23 criou) tem que estar lá.
SELECT name AS Tabela FROM sys.tables ORDER BY name;
GO
