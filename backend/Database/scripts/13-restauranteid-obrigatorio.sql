-- =============================================================
-- Etapa 42 — RestauranteId passa a ser obrigatório (NOT NULL)
--
-- Desde o script 08 (Etapa 28), a coluna RestauranteId de Produtos,
-- Categorias, Fornecedores e MovimentacoesEstoque era NULL de propósito,
-- só para a migração para multi-tenant. Hoje todo dado tem restaurante
-- (o cadastro sempre cria um, e o script 10 vinculou os dados antigos),
-- então o banco passa a garantir isso também — igual o código (Domain).
--
-- Como rodar no SQL Server Management Studio:
--   1. Rode DEPOIS do 11 (usa os nomes em português) e do 12.
--   2. Com a API parada, cole este arquivo e aperte Executar (F5).
--
-- Se ainda existir alguma linha sem restaurante, o script PARA e não muda
-- nada — rode antes o 10-vincular-dados-orfaos-restaurante-bk.sql.
-- Pode rodar de novo sem problema: coluna que já é NOT NULL é pulada.
--
-- Por que derrubar e recriar a FK? O SQL Server não deixa alterar
-- (ALTER COLUMN) uma coluna que faz parte de uma chave estrangeira — dá o
-- erro 5074. Então, para cada tabela: tira a FK, troca para NOT NULL e
-- coloca a FK de volta, com o mesmo nome. Tudo numa transação: se algo
-- falhar no meio, volta tudo como estava.
-- =============================================================

USE RestauranteEstoqueDb;
GO

SET XACT_ABORT ON;   -- qualquer erro desfaz a transação inteira
GO

-- 1) Conferência: nenhuma linha pode estar sem restaurante
DECLARE @semRestaurante INT =
      (SELECT COUNT(*) FROM Produtos             WHERE RestauranteId IS NULL)
    + (SELECT COUNT(*) FROM Categorias           WHERE RestauranteId IS NULL)
    + (SELECT COUNT(*) FROM Fornecedores         WHERE RestauranteId IS NULL)
    + (SELECT COUNT(*) FROM MovimentacoesEstoque WHERE RestauranteId IS NULL);

IF @semRestaurante > 0
BEGIN
    RAISERROR('Existem %d linha(s) sem RestauranteId. Rode o script 10 antes deste. Nada foi alterado.', 16, 1, @semRestaurante);
    SET NOEXEC ON;   -- não executa mais nada deste arquivo
END
GO

BEGIN TRANSACTION;

-- 2) Produtos
IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Produtos') AND name = 'RestauranteId' AND is_nullable = 1)
BEGIN
    ALTER TABLE Produtos DROP CONSTRAINT FK_Produtos_Restaurantes;
    ALTER TABLE Produtos ALTER COLUMN RestauranteId UNIQUEIDENTIFIER NOT NULL;
    ALTER TABLE Produtos ADD CONSTRAINT FK_Produtos_Restaurantes
        FOREIGN KEY (RestauranteId) REFERENCES Restaurantes (Id);
END

-- 3) Categorias
IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Categorias') AND name = 'RestauranteId' AND is_nullable = 1)
BEGIN
    ALTER TABLE Categorias DROP CONSTRAINT FK_Categorias_Restaurantes;
    ALTER TABLE Categorias ALTER COLUMN RestauranteId UNIQUEIDENTIFIER NOT NULL;
    ALTER TABLE Categorias ADD CONSTRAINT FK_Categorias_Restaurantes
        FOREIGN KEY (RestauranteId) REFERENCES Restaurantes (Id);
END

-- 4) Fornecedores
IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Fornecedores') AND name = 'RestauranteId' AND is_nullable = 1)
BEGIN
    ALTER TABLE Fornecedores DROP CONSTRAINT FK_Fornecedores_Restaurantes;
    ALTER TABLE Fornecedores ALTER COLUMN RestauranteId UNIQUEIDENTIFIER NOT NULL;
    ALTER TABLE Fornecedores ADD CONSTRAINT FK_Fornecedores_Restaurantes
        FOREIGN KEY (RestauranteId) REFERENCES Restaurantes (Id);
END

-- 5) MovimentacoesEstoque
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

-- 6) Conferir o resultado: is_nullable deve ser 0 nas 4 tabelas
SELECT t.name AS Tabela, c.name AS Coluna, c.is_nullable
FROM sys.columns c
JOIN sys.tables t ON t.object_id = c.object_id
WHERE c.name = 'RestauranteId'
  AND t.name IN ('Produtos', 'Categorias', 'Fornecedores', 'MovimentacoesEstoque');
GO
