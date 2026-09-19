-- =============================================================
-- Etapa 27 — Promover um usuário existente para a role "Admin"
--
-- Decisão da Etapa 27 (RBAC): todo usuário criado via POST /auth/register
-- já nasce com a role "Funcionario" automaticamente. Virar "Admin" é manual,
-- direto no banco (não existe endpoint pra isso ainda) — é o que este
-- script faz. O Identity guarda "quem tem qual role" na tabela de ligação
-- AspNetUserRoles (um par UserId/RoleId por linha); o INSERT abaixo só
-- ADICIONA a role Admin, sem remover a Funcionario que o usuário já tinha
-- — ter as duas ao mesmo tempo não é problema, os endpoints protegidos só
-- checam "tem Admin?".
--
-- IMPORTANTE: depois de rodar este script, é preciso fazer login de novo
-- (POST /auth/login) para pegar um token novo — o token JWT é montado no
-- momento do login com as roles que o usuário tinha NAQUELA hora; o token
-- antigo continua sem a claim de Admin até expirar.
--
-- Como rodar: troque o e-mail na variável abaixo pelo do usuário que você
-- quer promover, conecte no seu SQL Server, abra uma "Nova Consulta" no
-- banco RestauranteEstoqueDb e rode este arquivo (F5).
-- =============================================================

USE RestauranteEstoqueDb;
GO

DECLARE @Email NVARCHAR(256) = 'seuemail@aqui.com'; -- troque aqui
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

-- Conferir o resultado: deve aparecer "Admin" (e "Funcionario", que já tinha).
SELECT u.Email, r.Name AS Role
FROM AspNetUsers u
JOIN AspNetUserRoles ur ON ur.UserId = u.Id
JOIN AspNetRoles r ON r.Id = ur.RoleId
WHERE u.Email = 'seuemail@aqui.com'; -- troque aqui também
GO
