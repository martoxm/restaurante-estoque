-- =============================================================
-- Etapa 32 — Adiciona a coluna CreatedAt em Products, Categories,
-- Suppliers e Restaurants
--
-- Como rodar no SQL Server Management Studio:
--   Conecte no banco RestauranteEstoqueDb, cole este arquivo inteiro e
--   aperte Executar (F5), uma vez só.
--
-- Diferente do RestaurantId (Etapa 28), aqui a coluna já entra NOT NULL:
-- o DEFAULT GETUTCDATE() abaixo resolve o problema das linhas que já
-- existem (cada uma recebe a hora atual do servidor no momento do ALTER),
-- então não existe linha antiga sem valor pra quebrar o NOT NULL. Daqui
-- pra frente, todo Product/Category/Supplier/Restaurant novo é criado já
-- com CreatedAt de verdade (passado pelo Handler via IClock — nunca o
-- DEFAULT do banco, que só serve pra essas linhas antigas).
-- =============================================================

USE RestauranteEstoqueDb;
GO

-- 1) Products
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Products') AND name = 'CreatedAt')
BEGIN
    ALTER TABLE Products ADD CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE();
END
GO

-- 2) Categories
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Categories') AND name = 'CreatedAt')
BEGIN
    ALTER TABLE Categories ADD CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE();
END
GO

-- 3) Suppliers
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Suppliers') AND name = 'CreatedAt')
BEGIN
    ALTER TABLE Suppliers ADD CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE();
END
GO

-- 4) Restaurants
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Restaurants') AND name = 'CreatedAt')
BEGIN
    ALTER TABLE Restaurants ADD CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE();
END
GO

-- Fora de escopo de propósito (ver backend/docs/etapa-32-createdat.md):
-- StockMovements já tem MovementDate, que cumpre o mesmo papel; e
-- AspNetUsers é tabela do framework (Identity), fora da regra de script
-- manual deste projeto.

-- Conferir o resultado (4 tabelas, cada uma com a coluna CreatedAt nova)
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
