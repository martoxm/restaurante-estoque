-- =============================================================
-- Etapa 3 — Criação manual do banco e das tabelas (sem EF Core Migrations)
--
-- Como rodar no SQL Server Management Studio:
--   1. Conecte no seu servidor local (o da sua print: "localhost (SQL
--      Server ... DESKTOP-ASSF0I8\usuario)").
--   2. Clique em "Nova Consulta" com esse servidor selecionado
--      (importante: NÃO precisa estar dentro de nenhum banco específico
--      para rodar o PASSO 1 — ele mesmo vai criar o banco).
--   3. Cole este arquivo inteiro e aperte Executar (F5), uma vez só.
--   4. Atualize (F5 / botão direito > Atualizar) a pasta "Bancos de
--      Dados" no Object Explorer: vai aparecer "RestauranteEstoqueDb"
--      do lado de "ProverContatos".
-- =============================================================


-- =============================================================
-- PASSO 1 — Criar o banco de dados novo
-- =============================================================
-- CREATE DATABASE cria dois arquivos físicos no disco (na pasta padrão
-- de dados do seu SQL Server, ex: C:\Program Files\Microsoft SQL
-- Server\MSSQL...\MSSQL\DATA\):
--   - RestauranteEstoqueDb.mdf -> arquivo de DADOS (onde as tabelas
--     e as linhas realmente ficam salvas)
--   - RestauranteEstoqueDb_log.ldf -> arquivo de LOG de transações
--     (histórico de alterações, usado para o SQL Server conseguir
--     desfazer/recuperar em caso de falha)
-- Não precisamos escolher esses caminhos na mão: sem especificar nada,
-- o SQL Server usa a pasta padrão configurada na instância.
IF NOT EXISTS (SELECT 1 FROM sys.databases WHERE name = 'RestauranteEstoqueDb')
BEGIN
    CREATE DATABASE RestauranteEstoqueDb;
END
GO


-- =============================================================
-- PASSO 2 — "Entrar" no banco novo
-- =============================================================
-- Tudo que vier depois deste USE roda dentro do RestauranteEstoqueDb
-- (é assim que "salvamos as tabelas nele", e não em outro banco que
-- porventura já exista na instância, como o ProverContatos).
USE RestauranteEstoqueDb;
GO


-- =============================================================
-- PASSO 3 — Criar as tabelas dentro do banco novo
-- =============================================================

-- 3.1) Categories
-- Precisa existir antes de Products, porque Products tem uma FK para ela.
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

-- 3.2) Products
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

-- 3.3) StockMovements
-- Depende de Products (cada movimentação é sempre de um produto específico).
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'StockMovements')
BEGIN
    CREATE TABLE StockMovements
    (
        Id           UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
        ProductId    UNIQUEIDENTIFIER NOT NULL,
        Type         INT              NOT NULL, -- 1 = Entrada, 2 = Saida (enum StockMovementType)
        Quantity     INT              NOT NULL,
        MovementDate DATETIME2        NOT NULL,
        Notes        NVARCHAR(500)    NULL,

        CONSTRAINT FK_StockMovements_Products
            FOREIGN KEY (ProductId) REFERENCES Products (Id)
    );
END
GO

-- 3.4) Users
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
-- PASSO 4 (opcional) — Conferir o resultado
-- =============================================================
-- Rode só este SELECT depois para ver as 4 tabelas recém-criadas,
-- sem precisar ficar clicando no Object Explorer.
SELECT name AS TabelaCriada FROM sys.tables ORDER BY name;
GO
