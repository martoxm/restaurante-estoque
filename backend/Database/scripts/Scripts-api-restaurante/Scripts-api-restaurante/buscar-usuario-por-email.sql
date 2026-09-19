-- =============================================================
-- Consulta — Buscar um usuário específico (por e-mail) e suas roles
--
-- Só leitura (SELECT) — não altera nada no banco, diferente dos scripts
-- numerados na pasta acima (que criam/alteram tabela ou dado). Útil pra
-- checar rápido se um usuário existe e o que ele tem de role, sem precisar
-- decorar os nomes das 3 tabelas do Identity envolvidas (AspNetUsers,
-- AspNetUserRoles, AspNetRoles).
--
-- Como rodar: troque o e-mail na variável abaixo e execute (F5).
-- =============================================================

USE RestauranteEstoqueDb;
GO

DECLARE @Email NVARCHAR(256) = 'seuemail@aqui.com'; -- troque aqui

SELECT
    u.Id,
    u.Email,
    u.Name,
    r.Name AS Role
FROM AspNetUsers u
LEFT JOIN AspNetUserRoles ur ON ur.UserId = u.Id
LEFT JOIN AspNetRoles r ON r.Id = ur.RoleId
WHERE u.Email = @Email;
-- LEFT JOIN (não INNER JOIN): se o usuário não tiver NENHUMA role ainda,
-- ele continua aparecendo no resultado (com Role = NULL), em vez de
-- simplesmente não aparecer nada — mais fácil de perceber que o problema
-- é "sem role" e não "usuário não existe".
GO
