-- =============================================================
-- Etapa 8 — Inserir um usuário de teste para conseguir fazer Login
--
-- Por quê isso é manual, num INSERT? Porque o roadmap deste projeto ainda
-- não tem uma etapa de "cadastro de usuário" pela API (isso pode virar uma
-- etapa futura, se você quiser). Por enquanto, o único jeito de existir um
-- usuário no banco é inserir na mão — do mesmo jeito que as tabelas foram
-- criadas na mão no script 01.
--
-- O hash abaixo já foi gerado com BCrypt para a senha "senha123" (a mesma
-- biblioteca que a Api usa no Login, então bate certinho). Se quiser outra
-- senha, me avise: eu gero um novo hash e te passo o INSERT atualizado —
-- não dá para simplesmente digitar a senha em texto puro na coluna
-- PasswordHash, porque o BCrypt.Verify não teria como comparar.
--
-- Como rodar: mesma ideia do script 01 — conecte no seu SQL Server, abra
-- uma "Nova Consulta" e rode este arquivo (F5).
-- =============================================================

USE RestauranteEstoqueDb;
GO

IF NOT EXISTS (SELECT 1 FROM Users WHERE Email = 'teste@restaurante.com')
BEGIN
    INSERT INTO Users (Id, Name, Email, PasswordHash)
    VALUES (
        NEWID(),
        'Usuário de Teste',
        'teste@restaurante.com',
        '$2a$11$Xb39pRGw.ghdUN3HhgYiEOhhCBXeBrrfuKC5pSQcGjXZnYT9nbpIO' -- senha: senha123
    );
END
GO

-- Conferir o resultado
SELECT Id, Name, Email FROM Users;
GO
