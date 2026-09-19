-- =============================================================
-- Fora do roadmap — Vincula contas e dados antigos (sem restaurante)
-- ao restaurante BK
--
-- Contexto: contas criadas ANTES da Etapa 31 (registro ganhar a escolha
-- de restaurante) nasceram com AspNetUsers.RestaurantId = NULL — porque
-- na época o multi-tenant nem existia ainda. O mesmo vale pra Products,
-- Categories, Suppliers e StockMovements criados antes da Etapa 28
-- (fundação multi-tenant): todos nasceram com RestaurantId = NULL.
--
-- Depois da Etapa 30 (filtro de RestaurantId em todo Repository), um
-- usuário faz login e só enxerga dado com o MESMO RestaurantId que o
-- dele. Se a gente só atualizasse o usuário para o BK e deixasse os
-- produtos/categorias/fornecedores antigos com RestaurantId = NULL,
-- eles sumiriam da visão desse usuário (NULL != Id do BK).
--
-- Por isso este script faz as DUAS coisas juntas, nesta ordem:
--   1) Vincula ao BK todo usuário (AspNetUsers) com RestaurantId NULL.
--   2) Vincula ao BK todo Product/Category/Supplier/StockMovement que
--      também está com RestaurantId NULL (o dado "órfão" que esses
--      usuários já tinham antes do multi-tenant existir).
--
-- Repare que o passo 2 vincula TODO dado órfão ao BK, não só o dado
-- "de" um usuário específico — antes do multi-tenant não existia essa
-- amarração dono→dado, então não dá pra saber de quem era cada linha.
-- Se você tiver mais de uma conta antiga E elas deveriam pertencer a
-- restaurantes diferentes, não rode este script direto — ele junta
-- tudo num restaurante só (o BK).
--
-- Como rodar no SQL Server Management Studio:
--   1. Conecte no banco RestauranteEstoqueDb.
--   2. Confirme que o Id do BK abaixo (@RestaurantId) é o mesmo que
--      aparece em SELECT * FROM Restaurants — troque se for diferente.
--   3. Cole este arquivo inteiro e aperte Executar (F5), uma vez só.
--   4. Todo usuário afetado precisa logar de novo (POST /auth/login)
--      pra pegar um token novo já com a claim "restaurantId" certa —
--      o token antigo foi montado sem essa claim (ou com o valor
--      antigo) e continua assim até expirar.
-- =============================================================

USE RestauranteEstoqueDb;
GO

DECLARE @RestaurantId UNIQUEIDENTIFIER = 'C0DABCE3-358C-41C8-8F04-931491344CB4'; -- BK

IF NOT EXISTS (SELECT 1 FROM Restaurants WHERE Id = @RestaurantId)
BEGIN
    RAISERROR('Nenhum restaurante encontrado com esse Id — confira o Id do BK antes de rodar.', 16, 1);
    RETURN;
END

-- 1) Contas sem restaurante -> BK
UPDATE AspNetUsers
SET RestaurantId = @RestaurantId
WHERE RestaurantId IS NULL;

-- 2) Dados órfãos (criados antes do multi-tenant) -> BK
UPDATE Products
SET RestaurantId = @RestaurantId
WHERE RestaurantId IS NULL;

UPDATE Categories
SET RestaurantId = @RestaurantId
WHERE RestaurantId IS NULL;

UPDATE Suppliers
SET RestaurantId = @RestaurantId
WHERE RestaurantId IS NULL;

UPDATE StockMovements
SET RestaurantId = @RestaurantId
WHERE RestaurantId IS NULL;
GO

-- Conferir o resultado: não deve sobrar nenhuma linha com RestaurantId
-- nulo nas 5 tabelas.
SELECT 'AspNetUsers' AS Tabela, COUNT(*) AS LinhasSemRestaurante FROM AspNetUsers WHERE RestaurantId IS NULL
UNION ALL
SELECT 'Products', COUNT(*) FROM Products WHERE RestaurantId IS NULL
UNION ALL
SELECT 'Categories', COUNT(*) FROM Categories WHERE RestaurantId IS NULL
UNION ALL
SELECT 'Suppliers', COUNT(*) FROM Suppliers WHERE RestaurantId IS NULL
UNION ALL
SELECT 'StockMovements', COUNT(*) FROM StockMovements WHERE RestaurantId IS NULL;
GO

-- Conferir quem ficou vinculado ao BK
SELECT Email, RestaurantId
FROM AspNetUsers
WHERE RestaurantId = 'C0DABCE3-358C-41C8-8F04-931491344CB4';
GO
