-- =============================================================
-- Consulta — Listar TODOS os usuários e suas roles
--
-- Só leitura (SELECT) — não altera nada no banco. Versão "sem filtro" da
-- buscar-usuario-por-email.sql: útil pra ter uma visão geral de quem é
-- Admin, quem é Funcionario, e quem ainda não tem role nenhuma (não deveria
-- acontecer com usuários criados via /register desde a Etapa 27, mas pode
-- acontecer com usuários antigos, criados antes disso).
--
-- Como rodar: execute direto (F5), sem precisar editar nada.
-- =============================================================

USE RestauranteEstoqueDb;
GO

SELECT
    u.Email,
    u.Name,
    r.Name AS Role
FROM AspNetUsers u
LEFT JOIN AspNetUserRoles ur ON ur.UserId = u.Id
LEFT JOIN AspNetRoles r ON r.Id = ur.RoleId
ORDER BY u.Email, r.Name;
GO
