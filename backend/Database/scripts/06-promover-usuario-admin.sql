-- =============================================================
-- Etapa 27 — Aqui a gente promove um usuário que já existe para a role "Admin".
--
-- Lembra da decisão da Etapa 27 (RBAC)? Todo usuário criado via
-- POST /auth/register já nasce com a role "Funcionario" sozinho. Pra virar
-- "Admin", é na mão, direto no banco (ainda não existe endpoint pra isso) —
-- e é isso que este script faz. Pensa assim: o Identity guarda "quem tem
-- qual role" na tabela de ligação AspNetUserRoles (um par UserId/RoleId por
-- linha). O INSERT lá embaixo só ADICIONA a role Admin, sem tirar a
-- Funcionario que o usuário já tinha — e ter as duas ao mesmo tempo não é
-- problema, porque os endpoints protegidos só perguntam "tem Admin?".
--
-- IMPORTANTE: depois de rodar este script, o usuário tem que fazer login de
-- novo (POST /auth/login) pra pegar um token novo. Isso porque o token JWT é
-- montado na hora do login, com as roles que o usuário tinha NAQUELE
-- momento; ou seja, o token antigo continua sem a claim de Admin até expirar.
--
-- Pra rodar: troca o e-mail na variável abaixo pelo do usuário que você
-- quer promover, conecta no seu SQL Server, abre uma "Nova Consulta" no
-- banco RestauranteEstoqueDb e roda este arquivo (F5).
-- =============================================================

USE RestauranteEstoqueDb;
GO

DECLARE @Email NVARCHAR(256) = 'seuemail@aqui.com'; -- é aqui que você troca o e-mail
DECLARE @UserId UNIQUEIDENTIFIER = (SELECT Id FROM AspNetUsers WHERE Email = @Email);
DECLARE @AdminRoleId UNIQUEIDENTIFIER = (SELECT Id FROM AspNetRoles WHERE Name = 'Admin');

IF @UserId IS NULL
BEGIN
    RAISERROR('Nenhum usuário encontrado com esse e-mail.', 16, 1);
    RETURN;
END

IF EXISTS (SELECT 1 FROM AspNetUserRoles WHERE UserId = @UserId AND RoleId = @AdminRoleId)
BEGIN
    PRINT 'Esse usuário já tem a role Admin — nada a fazer.';
END
ELSE
BEGIN
    INSERT INTO AspNetUserRoles (UserId, RoleId) VALUES (@UserId, @AdminRoleId);
    PRINT 'Role Admin adicionada com sucesso.';
END
GO

-- Agora a gente confere: tem que aparecer "Admin" (e a "Funcionario", que ele já tinha).
SELECT u.Email, r.Name AS Role
FROM AspNetUsers u
JOIN AspNetUserRoles ur ON ur.UserId = u.Id
JOIN AspNetRoles r ON r.Id = ur.RoleId
WHERE u.Email = 'seuemail@aqui.com'; -- e não esquece de trocar aqui também
GO
