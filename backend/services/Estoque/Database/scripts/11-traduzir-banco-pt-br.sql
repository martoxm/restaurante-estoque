-- =============================================================
-- Etapa 36 — Aqui a gente traduz as tabelas, as colunas e as FKs pro
-- português. É a linguagem ubíqua: o código e o banco falando a mesma
-- língua do negócio.
--
-- Pra rodar no SQL Server Management Studio:
--   Conecta no banco RestauranteEstoqueDb, cola este arquivo inteiro e
--   aperta Executar (F5), uma vez só. E PARA a API antes de rodar.
--
-- Por que sp_rename e não DROP/CREATE? Porque o sp_rename só troca o NOME —
-- os dados, as chaves e os índices continuam exatamente onde estavam. Já um
-- DROP TABLE apagaria tudo.
--
-- Repara que cada bloco é protegido com um IF (OBJECT_ID/COL_LENGTH). Ou
-- seja, rodar duas vezes não quebra nada: na segunda vez, nada mais "tem
-- nome em inglês" e todos os IFs dão falso. E tudo roda dentro de uma
-- transação: se qualquer passo falhar, nada fica pela metade.
--
-- Os scripts 01-10 ficam do jeito que estão, porque eles são o histórico —
-- quem clonar o projeto roda do 01 ao 10 e depois este 11, igual uma
-- sequência de migrations.
--
-- Algumas coisas ficaram de fora de propósito. As tabelas AspNet* são do
-- Identity, e o framework espera esses nomes — por isso só as DUAS colunas
-- que nós mesmos colocamos em AspNetUsers (Name, RestaurantId) são
-- traduzidas. Os nomes automáticos de PK/DEFAULT (ex.:
-- PK__Products__3214EC07...) também ficam: quem gerou foi o SQL Server, e
-- ninguém usa eles pelo nome.
-- =============================================================

USE RestauranteEstoqueDb;
GO

SET XACT_ABORT ON; -- assim, qualquer erro desfaz a transação inteira
BEGIN TRANSACTION;

-- 1) Primeiro, o nome das tabelas
IF OBJECT_ID('Products') IS NOT NULL       EXEC sp_rename 'Products', 'Produtos';
IF OBJECT_ID('Categories') IS NOT NULL     EXEC sp_rename 'Categories', 'Categorias';
IF OBJECT_ID('Suppliers') IS NOT NULL      EXEC sp_rename 'Suppliers', 'Fornecedores';
IF OBJECT_ID('StockMovements') IS NOT NULL EXEC sp_rename 'StockMovements', 'MovimentacoesEstoque';
IF OBJECT_ID('Restaurants') IS NOT NULL    EXEC sp_rename 'Restaurants', 'Restaurantes';

-- 2) Agora as colunas de Produtos
IF COL_LENGTH('Produtos', 'Name') IS NOT NULL            EXEC sp_rename 'Produtos.Name', 'Nome', 'COLUMN';
IF COL_LENGTH('Produtos', 'Description') IS NOT NULL     EXEC sp_rename 'Produtos.Description', 'Descricao', 'COLUMN';
IF COL_LENGTH('Produtos', 'Price') IS NOT NULL           EXEC sp_rename 'Produtos.Price', 'Preco', 'COLUMN';
IF COL_LENGTH('Produtos', 'QuantityInStock') IS NOT NULL EXEC sp_rename 'Produtos.QuantityInStock', 'QuantidadeEmEstoque', 'COLUMN';
IF COL_LENGTH('Produtos', 'CategoryId') IS NOT NULL      EXEC sp_rename 'Produtos.CategoryId', 'CategoriaId', 'COLUMN';
IF COL_LENGTH('Produtos', 'RestaurantId') IS NOT NULL    EXEC sp_rename 'Produtos.RestaurantId', 'RestauranteId', 'COLUMN';
IF COL_LENGTH('Produtos', 'CreatedAt') IS NOT NULL       EXEC sp_rename 'Produtos.CreatedAt', 'CriadoEm', 'COLUMN';

-- 3) Depois as colunas de Categorias
IF COL_LENGTH('Categorias', 'Name') IS NOT NULL         EXEC sp_rename 'Categorias.Name', 'Nome', 'COLUMN';
IF COL_LENGTH('Categorias', 'Description') IS NOT NULL  EXEC sp_rename 'Categorias.Description', 'Descricao', 'COLUMN';
IF COL_LENGTH('Categorias', 'RestaurantId') IS NOT NULL EXEC sp_rename 'Categorias.RestaurantId', 'RestauranteId', 'COLUMN';
IF COL_LENGTH('Categorias', 'CreatedAt') IS NOT NULL    EXEC sp_rename 'Categorias.CreatedAt', 'CriadoEm', 'COLUMN';

-- 4) As colunas de Fornecedores (a Email fica, porque já é igual em português)
IF COL_LENGTH('Fornecedores', 'Name') IS NOT NULL         EXEC sp_rename 'Fornecedores.Name', 'Nome', 'COLUMN';
IF COL_LENGTH('Fornecedores', 'Phone') IS NOT NULL        EXEC sp_rename 'Fornecedores.Phone', 'Telefone', 'COLUMN';
IF COL_LENGTH('Fornecedores', 'RestaurantId') IS NOT NULL EXEC sp_rename 'Fornecedores.RestaurantId', 'RestauranteId', 'COLUMN';
IF COL_LENGTH('Fornecedores', 'CreatedAt') IS NOT NULL    EXEC sp_rename 'Fornecedores.CreatedAt', 'CriadoEm', 'COLUMN';

-- 5) As colunas de MovimentacoesEstoque
IF COL_LENGTH('MovimentacoesEstoque', 'ProductId') IS NOT NULL    EXEC sp_rename 'MovimentacoesEstoque.ProductId', 'ProdutoId', 'COLUMN';
IF COL_LENGTH('MovimentacoesEstoque', 'Type') IS NOT NULL         EXEC sp_rename 'MovimentacoesEstoque.Type', 'Tipo', 'COLUMN';
IF COL_LENGTH('MovimentacoesEstoque', 'Quantity') IS NOT NULL     EXEC sp_rename 'MovimentacoesEstoque.Quantity', 'Quantidade', 'COLUMN';
IF COL_LENGTH('MovimentacoesEstoque', 'MovementDate') IS NOT NULL EXEC sp_rename 'MovimentacoesEstoque.MovementDate', 'DataMovimentacao', 'COLUMN';
IF COL_LENGTH('MovimentacoesEstoque', 'Notes') IS NOT NULL        EXEC sp_rename 'MovimentacoesEstoque.Notes', 'Observacoes', 'COLUMN';
IF COL_LENGTH('MovimentacoesEstoque', 'RestaurantId') IS NOT NULL EXEC sp_rename 'MovimentacoesEstoque.RestaurantId', 'RestauranteId', 'COLUMN';

-- 6) As colunas de Restaurantes
IF COL_LENGTH('Restaurantes', 'Name') IS NOT NULL      EXEC sp_rename 'Restaurantes.Name', 'Nome', 'COLUMN';
IF COL_LENGTH('Restaurantes', 'CreatedAt') IS NOT NULL EXEC sp_rename 'Restaurantes.CreatedAt', 'CriadoEm', 'COLUMN';

-- 7) Aqui só as duas colunas que são NOSSAS em AspNetUsers (ApplicationUser.Nome/RestauranteId)
IF COL_LENGTH('AspNetUsers', 'Name') IS NOT NULL         EXEC sp_rename 'AspNetUsers.Name', 'Nome', 'COLUMN';
IF COL_LENGTH('AspNetUsers', 'RestaurantId') IS NOT NULL EXEC sp_rename 'AspNetUsers.RestaurantId', 'RestauranteId', 'COLUMN';

-- 8) E as foreign keys, porque o nome delas cita as tabelas antigas
IF OBJECT_ID('FK_Products_Categories') IS NOT NULL        EXEC sp_rename 'FK_Products_Categories', 'FK_Produtos_Categorias', 'OBJECT';
IF OBJECT_ID('FK_StockMovements_Products') IS NOT NULL    EXEC sp_rename 'FK_StockMovements_Products', 'FK_MovimentacoesEstoque_Produtos', 'OBJECT';
IF OBJECT_ID('FK_Products_Restaurants') IS NOT NULL       EXEC sp_rename 'FK_Products_Restaurants', 'FK_Produtos_Restaurantes', 'OBJECT';
IF OBJECT_ID('FK_Categories_Restaurants') IS NOT NULL     EXEC sp_rename 'FK_Categories_Restaurants', 'FK_Categorias_Restaurantes', 'OBJECT';
IF OBJECT_ID('FK_Suppliers_Restaurants') IS NOT NULL      EXEC sp_rename 'FK_Suppliers_Restaurants', 'FK_Fornecedores_Restaurantes', 'OBJECT';
IF OBJECT_ID('FK_StockMovements_Restaurants') IS NOT NULL EXEC sp_rename 'FK_StockMovements_Restaurants', 'FK_MovimentacoesEstoque_Restaurantes', 'OBJECT';
IF OBJECT_ID('FK_AspNetUsers_Restaurants') IS NOT NULL    EXEC sp_rename 'FK_AspNetUsers_Restaurants', 'FK_AspNetUsers_Restaurantes', 'OBJECT';

COMMIT TRANSACTION;
GO

-- Agora a gente confere: nenhuma linha aqui deveria ter nome em inglês
SELECT t.name AS Tabela, c.name AS Coluna
FROM sys.columns c
JOIN sys.tables t ON t.object_id = c.object_id
WHERE t.name IN ('Produtos', 'Categorias', 'Fornecedores', 'MovimentacoesEstoque', 'Restaurantes')
   OR (t.name = 'AspNetUsers' AND c.name IN ('Nome', 'RestauranteId'))
ORDER BY t.name, c.column_id;

SELECT name AS ForeignKey FROM sys.foreign_keys WHERE name LIKE 'FK[_]%' ORDER BY name;
GO
