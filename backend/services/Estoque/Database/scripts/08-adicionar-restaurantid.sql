-- =============================================================
-- Etapa 28 — Aqui a gente adiciona a coluna RestaurantId nas tabelas. É a
-- base do multi-tenant.
--
-- Pra rodar no SQL Server Management Studio:
--   1. Roda primeiro o 07-criar-tabela-restaurants.sql (a tabela Restaurants
--      tem que existir antes das FKs lá embaixo apontarem pra ela).
--   2. Conecta no banco RestauranteEstoqueDb, cola este arquivo inteiro e
--      aperta Executar (F5), uma vez só.
--
-- Repara que o RestaurantId entra NULLABLE de propósito em todas as tabelas.
-- Por quê? Porque ainda não existe nenhum restaurante cadastrado, nem nenhum
-- código preenchendo esse valor (isso só começa a partir da Etapa 29/30). Se
-- a coluna já entrasse NOT NULL, as linhas que já existem (Products,
-- Categories, etc. criados antes desta etapa) quebrariam o ALTER TABLE, já
-- que não têm valor nenhum pra colocar ali. Mais pra frente, uma etapa pode
-- "fechar" a coluna como NOT NULL, quando todo mundo tiver um restaurante de verdade.
-- =============================================================

USE RestauranteEstoqueDb;
GO

-- 1) Começando pela Products
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Products') AND name = 'RestaurantId')
BEGIN
    ALTER TABLE Products ADD RestaurantId UNIQUEIDENTIFIER NULL;

    ALTER TABLE Products ADD CONSTRAINT FK_Products_Restaurants
        FOREIGN KEY (RestaurantId) REFERENCES Restaurants (Id);
END
GO

-- 2) Agora a Categories
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Categories') AND name = 'RestaurantId')
BEGIN
    ALTER TABLE Categories ADD RestaurantId UNIQUEIDENTIFIER NULL;

    ALTER TABLE Categories ADD CONSTRAINT FK_Categories_Restaurants
        FOREIGN KEY (RestaurantId) REFERENCES Restaurants (Id);
END
GO

-- 3) Depois a Suppliers
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Suppliers') AND name = 'RestaurantId')
BEGIN
    ALTER TABLE Suppliers ADD RestaurantId UNIQUEIDENTIFIER NULL;

    ALTER TABLE Suppliers ADD CONSTRAINT FK_Suppliers_Restaurants
        FOREIGN KEY (RestaurantId) REFERENCES Restaurants (Id);
END
GO

-- 4) Em seguida a StockMovements
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('StockMovements') AND name = 'RestaurantId')
BEGIN
    ALTER TABLE StockMovements ADD RestaurantId UNIQUEIDENTIFIER NULL;

    ALTER TABLE StockMovements ADD CONSTRAINT FK_StockMovements_Restaurants
        FOREIGN KEY (RestaurantId) REFERENCES Restaurants (Id);
END
GO

-- 5) E por fim a AspNetUsers. Ela é tabela do ASP.NET Core Identity, criada
-- por Migration na Etapa 23 — mas repara que a coluna nova em si é coisa do
-- negócio (business/multi-tenant), não faz parte do schema padrão do
-- Identity. Por isso ela continua sendo ALTER TABLE manual, igual às outras
-- 4 tabelas aí de cima, e não uma Migration nova do EF Core.
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('AspNetUsers') AND name = 'RestaurantId')
BEGIN
    ALTER TABLE AspNetUsers ADD RestaurantId UNIQUEIDENTIFIER NULL;

    ALTER TABLE AspNetUsers ADD CONSTRAINT FK_AspNetUsers_Restaurants
        FOREIGN KEY (RestaurantId) REFERENCES Restaurants (Id);
END
GO

-- Agora a gente confere: têm que aparecer 5 tabelas, cada uma com a coluna RestaurantId nova
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
