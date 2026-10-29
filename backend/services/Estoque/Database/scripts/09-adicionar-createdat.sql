-- =============================================================
-- Etapa 32 — Aqui a gente adiciona a coluna CreatedAt em Products,
-- Categories, Suppliers e Restaurants.
--
-- Pra rodar no SQL Server Management Studio:
--   Conecta no banco RestauranteEstoqueDb, cola este arquivo inteiro e
--   aperta Executar (F5), uma vez só.
--
-- Repara que, diferente do RestaurantId (Etapa 28), aqui a coluna já entra
-- NOT NULL. Dá pra fazer isso porque o DEFAULT GETUTCDATE() lá embaixo
-- resolve o problema das linhas que já existem: cada uma ganha a hora atual
-- do servidor no momento do ALTER. Ou seja, não sobra linha antiga sem valor
-- pra quebrar o NOT NULL. Daqui pra frente, todo Product/Category/Supplier/
-- Restaurant novo já nasce com o CreatedAt de verdade, que o Handler passa
-- via IClock — nunca pelo DEFAULT do banco, que só existe pra essas linhas antigas.
-- =============================================================

USE RestauranteEstoqueDb;
GO

-- 1) Começando pela Products
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Products') AND name = 'CreatedAt')
BEGIN
    ALTER TABLE Products ADD CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE();
END
GO

-- 2) Agora a Categories
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Categories') AND name = 'CreatedAt')
BEGIN
    ALTER TABLE Categories ADD CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE();
END
GO

-- 3) Depois a Suppliers
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Suppliers') AND name = 'CreatedAt')
BEGIN
    ALTER TABLE Suppliers ADD CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE();
END
GO

-- 4) E por fim a Restaurants
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Restaurants') AND name = 'CreatedAt')
BEGIN
    ALTER TABLE Restaurants ADD CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE();
END
GO

-- Duas tabelas ficaram de fora de propósito (tem mais detalhe em
-- backend/docs/etapa-32-createdat.md): a StockMovements, porque ela já tem
-- o MovementDate, que faz o mesmo papel; e a AspNetUsers, porque é tabela
-- do framework (Identity) e fica fora da regra de script manual deste projeto.

-- Agora a gente confere: têm que aparecer 4 tabelas, cada uma com a coluna CreatedAt nova
SELECT
    t.name  AS Tabela,
    c.name  AS Coluna,
    ty.name AS Tipo,
    c.is_nullable AS Nullable
FROM sys.columns c
JOIN sys.tables t ON t.object_id = c.object_id
JOIN sys.types ty ON ty.user_type_id = c.user_type_id
WHERE c.name = 'CreatedAt'
ORDER BY t.name;
GO
