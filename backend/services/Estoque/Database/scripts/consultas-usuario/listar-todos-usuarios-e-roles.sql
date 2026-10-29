-- =============================================================
-- Consulta — Aqui a gente lista TODOS os usuários e as roles de cada um.
--
-- É só leitura (SELECT), não altera nada no banco. Pensa nela como a versão
-- "sem filtro" da buscar-usuario-por-email.sql: serve pra ter uma visão
-- geral de quem é Admin, quem é Funcionario e quem ainda não tem role
-- nenhuma. Esse último caso não deveria acontecer com quem foi criado via
-- /register desde a Etapa 27, mas pode aparecer com usuário antigo, criado
-- antes disso.
--
-- Pra rodar: é só executar direto (F5), sem precisar editar nada.
-- =============================================================

USE RestauranteEstoqueDb;
GO

SELECT
    u.Email,
    u.Nome,
    r.Name AS Role
FROM identidade.AspNetUsers u
LEFT JOIN identidade.AspNetUserRoles ur ON ur.UserId = u.Id
LEFT JOIN identidade.AspNetRoles r ON r.Id = ur.RoleId
ORDER BY u.Email, r.Name;
GO
