-- =============================================================
-- Etapa 28 — Adiciona a coluna RestaurantId (fundação multi-tenant)
--
-- Como rodar no SQL Server Management Studio:
--   1. Rode primeiro o 07-criar-tabela-restaurants.sql (a tabela Restaurants
--      precisa existir antes das FKs abaixo apontarem pra ela).
--   2. Conecte no banco RestauranteEstoqueDb, cole este arquivo inteiro e
--      aperte Executar (F5), uma vez só.
--
-- RestaurantId entra NULLABLE de propósito em todas as tabelas: ainda não
-- existe nenhum restaurante cadastrado nem nenhum código preenchendo esse
-- valor (isso só acontece a partir da Etapa 29/30). Se a coluna fosse
-- NOT NULL agora, as linhas que já existem (Products, Categories, etc.
-- criados antes desta etapa) quebrariam o ALTER TABLE, porque não têm
-- valor nenhum pra preencher. Uma etapa futura pode "fechar" a coluna como
-- NOT NULL depois que todo mundo tiver um restaurante de verdade.
-- =============================================================

USE RestauranteEstoqueDb;
GO

-- 1) Products
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Products') AND name = 'RestaurantId')
BEGIN
    ALTER TABLE Products ADD RestaurantId UNIQUEIDENTIFIER NULL;

    ALTER TABLE Products ADD CONSTRAINT FK_Products_Restaurants
        FOREIGN KEY (RestaurantId) REFERENCES Restaurants (Id);
END
GO

-- 2) Categories
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Categories') AND name = 'RestaurantId')
BEGIN
    ALTER TABLE Categories ADD RestaurantId UNIQUEIDENTIFIER NULL;

    ALTER TABLE Categories ADD CONSTRAINT FK_Categories_Restaurants
        FOREIGN KEY (RestaurantId) REFERENCES Restaurants (Id);
END
GO

-- 3) Suppliers
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Suppliers') AND name = 'RestaurantId')
BEGIN
    ALTER TABLE Suppliers ADD RestaurantId UNIQUEIDENTIFIER NULL;

    ALTER TABLE Suppliers ADD CONSTRAINT FK_Suppliers_Restaurants
        FOREIGN KEY (RestaurantId) REFERENCES Restaurants (Id);
END
GO

-- 4) StockMovements
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('StockMovements') AND name = 'RestaurantId')
BEGIN
    ALTER TABLE StockMovements ADD RestaurantId UNIQUEIDENTIFIER NULL;

    ALTER TABLE StockMovements ADD CONSTRAINT FK_StockMovements_Restaurants
        FOREIGN KEY (RestaurantId) REFERENCES Restaurants (Id);
END
GO

-- 5) AspNetUsers (tabela do ASP.NET Core Identity, criada por Migration na
-- Etapa 23 — mas a coluna nova em si é business/multi-tenant, não schema
-- padrão do Identity, então continua sendo ALTER TABLE manual, igual as
-- outras 4 tabelas acima, e não uma nova Migration do EF Core).
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('AspNetUsers') AND name = 'RestaurantId')
BEGIN
    ALTER TABLE AspNetUsers ADD RestaurantId UNIQUEIDENTIFIER NULL;

    ALTER TABLE AspNetUsers ADD CONSTRAINT FK_AspNetUsers_Restaurants
        FOREIGN KEY (RestaurantId) REFERENCES Restaurants (Id);
END
GO

-- Conferir o resultado (5 tabelas, cada uma com a coluna RestaurantId nova)
SELECT
    t.name  AS Tabela,
    c.name  AS Coluna,
    ty.name AS Tipo,
    c.is_nullable AS Nullable
FROM sys.columns c
JOIN sys.tables t ON t.object_id = c.object_id
JOIN sys.types ty ON ty.user_type_id = c.user_type_id
WHERE c.name = 'RestaurantId'
ORDER BY t.name;
GO
