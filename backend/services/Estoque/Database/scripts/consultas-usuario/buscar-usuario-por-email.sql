-- =============================================================
-- Consulta — Aqui a gente busca um usuário específico (pelo e-mail) e as
-- roles dele.
--
-- Repara que é só leitura (SELECT): não altera nada no banco, diferente dos
-- scripts numerados da pasta de cima (que criam/alteram tabela ou dado).
-- Serve pra checar rapidinho se um usuário existe e qual role ele tem, sem
-- precisar decorar o nome das 3 tabelas do Identity que entram nessa história
-- (AspNetUsers, AspNetUserRoles, AspNetRoles).
--
-- Pra rodar: troca o e-mail na variável abaixo e executa (F5).
-- =============================================================

USE RestauranteEstoqueDb;
GO

DECLARE @Email NVARCHAR(256) = 'seuemail@aqui.com'; -- é aqui que você troca o e-mail

SELECT
    u.Id,
    u.Email,
    u.Nome,
    r.Name AS Role
FROM identidade.AspNetUsers u
LEFT JOIN identidade.AspNetUserRoles ur ON ur.UserId = u.Id
LEFT JOIN identidade.AspNetRoles r ON r.Id = ur.RoleId
WHERE u.Email = @Email;
-- Por que LEFT JOIN e não INNER JOIN? Porque, se o usuário ainda não tiver
-- NENHUMA role, ele continua aparecendo no resultado (com Role = NULL), em
-- vez de simplesmente não vir nada. Assim fica bem mais fácil perceber que o
-- problema é "está sem role" e não "o usuário não existe".
GO
