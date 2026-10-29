-- =============================================================
-- Etapa 3 — Aqui a gente cria o banco e as tabelas na mão, sem usar as
-- Migrations do EF Core.
--
-- Pra rodar no SQL Server Management Studio, é assim:
--   1. Conecta no seu servidor local (aquele da sua print: "localhost (SQL
--      Server ... DESKTOP-ASSF0I8\usuario)").
--   2. Clica em "Nova Consulta" com esse servidor selecionado. Repara que
--      NÃO precisa estar dentro de nenhum banco específico pra rodar o
--      PASSO 1 — é ele mesmo que vai criar o banco.
--   3. Cola este arquivo inteiro e aperta Executar (F5), uma vez só.
--   4. Depois atualiza (F5 / botão direito > Atualizar) a pasta "Bancos de
--      Dados" no Object Explorer: o "RestauranteEstoqueDb" vai aparecer
--      do lado do "ProverContatos".
-- =============================================================


-- =============================================================
-- PASSO 1 — Primeiro a gente cria o banco de dados novo
-- =============================================================
-- O que acontece quando a gente roda CREATE DATABASE é que o SQL Server
-- cria dois arquivos físicos no disco (na pasta padrão de dados dele,
-- ex: C:\Program Files\Microsoft SQL Server\MSSQL...\MSSQL\DATA\):
--   - RestauranteEstoqueDb.mdf -> esse é o arquivo de DADOS (é onde as
--     tabelas e as linhas ficam salvas de verdade)
--   - RestauranteEstoqueDb_log.ldf -> esse é o arquivo de LOG de
--     transações (o histórico de alterações, que o SQL Server usa pra
--     conseguir desfazer/recuperar se alguma coisa falhar)
-- Repara que a gente não precisa escolher esses caminhos na mão: se não
-- especificar nada, o SQL Server usa a pasta padrão configurada na instância.
IF NOT EXISTS (SELECT 1 FROM sys.databases WHERE name = 'RestauranteEstoqueDb')
BEGIN
    CREATE DATABASE RestauranteEstoqueDb;
END
GO


-- =============================================================
-- PASSO 2 — Agora a gente "entra" no banco novo
-- =============================================================
-- Tudo que vem depois deste USE roda dentro do RestauranteEstoqueDb.
-- Ou seja, é assim que a gente garante que as tabelas vão ser salvas
-- nele, e não em outro banco que por acaso já exista na instância,
-- como o ProverContatos.
USE RestauranteEstoqueDb;
GO


-- =============================================================
-- PASSO 3 — Hora de criar as tabelas dentro do banco novo
-- =============================================================

-- 3.1) Começando pela Categories
-- Ela tem que existir antes da Products, porque a Products tem uma FK
-- apontando pra ela.
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Categories')
BEGIN
    CREATE TABLE Categories
    (
        Id          UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
        Name        NVARCHAR(100)    NOT NULL,
        Description NVARCHAR(500)    NULL
    );
END
GO

-- 3.2) Depois vem a Products
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Products')
BEGIN
    CREATE TABLE Products
    (
        Id              UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
        Name            NVARCHAR(150)    NOT NULL,
        Description     NVARCHAR(500)    NULL,
        Price           DECIMAL(10, 2)   NOT NULL,
        QuantityInStock INT              NOT NULL,
        CategoryId      UNIQUEIDENTIFIER NOT NULL,

        CONSTRAINT FK_Products_Categories
            FOREIGN KEY (CategoryId) REFERENCES Categories (Id)
    );
END
GO

-- 3.3) Em seguida, a StockMovements
-- Ela depende da Products, porque toda movimentação é sempre de um
-- produto específico.
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'StockMovements')
BEGIN
    CREATE TABLE StockMovements
    (
        Id           UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
        ProductId    UNIQUEIDENTIFIER NOT NULL,
        Type         INT              NOT NULL, -- aqui 1 = Entrada e 2 = Saida (é o enum StockMovementType)
        Quantity     INT              NOT NULL,
        MovementDate DATETIME2        NOT NULL,
        Notes        NVARCHAR(500)    NULL,

        CONSTRAINT FK_StockMovements_Products
            FOREIGN KEY (ProductId) REFERENCES Products (Id)
    );
END
GO

-- 3.4) E por último, a Users
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Users')
BEGIN
    CREATE TABLE Users
    (
        Id           UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
        Name         NVARCHAR(150)    NOT NULL,
        Email        NVARCHAR(200)    NOT NULL,
        PasswordHash NVARCHAR(300)    NOT NULL,

        CONSTRAINT UQ_Users_Email UNIQUE (Email)
    );
END
GO


-- =============================================================
-- PASSO 4 (opcional) — Pra fechar, a gente confere o resultado
-- =============================================================
-- Depois é só rodar este SELECT sozinho pra ver as 4 tabelas que acabaram
-- de nascer, sem precisar ficar clicando no Object Explorer.
SELECT name AS TabelaCriada FROM sys.tables ORDER BY name;
GO
