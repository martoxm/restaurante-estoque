-- =============================================================
-- Etapa 42 — Aqui o RestauranteId passa a ser obrigatório (NOT NULL).
--
-- Lembra que, desde o script 08 (Etapa 28), a coluna RestauranteId de
-- Produtos, Categorias, Fornecedores e MovimentacoesEstoque era NULL de
-- propósito? Era só pra dar conta da migração pro multi-tenant. Hoje todo
-- dado já tem restaurante (o cadastro sempre cria um, e o script 10 ligou os
-- dados antigos), então o banco passa a garantir isso também — do mesmo
-- jeito que o código (Domain) já garante.
--
-- Pra rodar no SQL Server Management Studio:
--   1. Roda DEPOIS do 11 (porque ele usa os nomes em português) e do 12.
--   2. Com a API parada, cola este arquivo e aperta Executar (F5).
--
-- Se ainda existir alguma linha sem restaurante, o script PARA e não muda
-- nada — aí você roda antes o 10-vincular-dados-orfaos-restaurante-bk.sql.
-- E pode rodar de novo sem medo: coluna que já é NOT NULL é pulada.
--
-- Por que derrubar e recriar a FK? Porque o SQL Server não deixa alterar
-- (ALTER COLUMN) uma coluna que faz parte de uma chave estrangeira — ele dá
-- o erro 5074. Então, pra cada tabela, a gente tira a FK, troca pra NOT NULL
-- e coloca a FK de volta, com o mesmo nome. E tudo numa transação: se algo
-- falhar no meio, volta tudo como estava.
-- =============================================================

USE RestauranteEstoqueDb;
GO

SET XACT_ABORT ON;   -- assim, qualquer erro desfaz a transação inteira
GO

-- 1) Antes de tudo, a gente confere se nenhuma linha está sem restaurante
DECLARE @semRestaurante INT =
      (SELECT COUNT(*) FROM Produtos             WHERE RestauranteId IS NULL)
    + (SELECT COUNT(*) FROM Categorias           WHERE RestauranteId IS NULL)
    + (SELECT COUNT(*) FROM Fornecedores         WHERE RestauranteId IS NULL)
    + (SELECT COUNT(*) FROM MovimentacoesEstoque WHERE RestauranteId IS NULL);

IF @semRestaurante > 0
BEGIN
    RAISERROR('Existem %d linha(s) sem RestauranteId. Rode o script 10 antes deste. Nada foi alterado.', 16, 1, @semRestaurante);
    SET NOEXEC ON;   -- daqui pra baixo, nada mais deste arquivo roda
END
GO

BEGIN TRANSACTION;

-- 2) Começando pela Produtos
IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Produtos') AND name = 'RestauranteId' AND is_nullable = 1)
BEGIN
    ALTER TABLE Produtos DROP CONSTRAINT FK_Produtos_Restaurantes;
    ALTER TABLE Produtos ALTER COLUMN RestauranteId UNIQUEIDENTIFIER NOT NULL;
    ALTER TABLE Produtos ADD CONSTRAINT FK_Produtos_Restaurantes
        FOREIGN KEY (RestauranteId) REFERENCES Restaurantes (Id);
END

-- 3) Agora a Categorias
IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Categorias') AND name = 'RestauranteId' AND is_nullable = 1)
BEGIN
    ALTER TABLE Categorias DROP CONSTRAINT FK_Categorias_Restaurantes;
    ALTER TABLE Categorias ALTER COLUMN RestauranteId UNIQUEIDENTIFIER NOT NULL;
    ALTER TABLE Categorias ADD CONSTRAINT FK_Categorias_Restaurantes
        FOREIGN KEY (RestauranteId) REFERENCES Restaurantes (Id);
END

-- 4) Depois a Fornecedores
IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Fornecedores') AND name = 'RestauranteId' AND is_nullable = 1)
BEGIN
    ALTER TABLE Fornecedores DROP CONSTRAINT FK_Fornecedores_Restaurantes;
    ALTER TABLE Fornecedores ALTER COLUMN RestauranteId UNIQUEIDENTIFIER NOT NULL;
    ALTER TABLE Fornecedores ADD CONSTRAINT FK_Fornecedores_Restaurantes
        FOREIGN KEY (RestauranteId) REFERENCES Restaurantes (Id);
END

-- 5) E por fim a MovimentacoesEstoque
IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('MovimentacoesEstoque') AND name = 'RestauranteId' AND is_nullable = 1)
BEGIN
    ALTER TABLE MovimentacoesEstoque DROP CONSTRAINT FK_MovimentacoesEstoque_Restaurantes;
    ALTER TABLE MovimentacoesEstoque ALTER COLUMN RestauranteId UNIQUEIDENTIFIER NOT NULL;
    ALTER TABLE MovimentacoesEstoque ADD CONSTRAINT FK_MovimentacoesEstoque_Restaurantes
        FOREIGN KEY (RestauranteId) REFERENCES Restaurantes (Id);
END

COMMIT TRANSACTION;
GO

SET NOEXEC OFF;
GO

-- 6) Agora a gente confere: o is_nullable tem que dar 0 nas 4 tabelas
SELECT t.name AS Tabela, c.name AS Coluna, c.is_nullable
FROM sys.columns c
JOIN sys.tables t ON t.object_id = c.object_id
WHERE c.name = 'RestauranteId'
  AND t.name IN ('Produtos', 'Categorias', 'Fornecedores', 'MovimentacoesEstoque');
GO
