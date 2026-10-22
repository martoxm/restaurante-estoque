-- =============================================================
-- Fora do roadmap — Aqui a gente pega as contas e os dados antigos (que
-- ficaram sem restaurante) e liga tudo ao restaurante BK.
--
-- Pra entender o contexto: as contas criadas ANTES da Etapa 31 (quando o
-- registro ganhou a escolha de restaurante) nasceram com
-- AspNetUsers.RestaurantId = NULL — afinal, na época o multi-tenant nem
-- existia. A mesma coisa vale pra Products, Categories, Suppliers e
-- StockMovements criados antes da Etapa 28 (a base do multi-tenant):
-- todos nasceram com RestaurantId = NULL.
--
-- O problema é que, depois da Etapa 30 (o filtro de RestaurantId em todo
-- Repository), o usuário faz login e só enxerga dado com o MESMO
-- RestaurantId que o dele. Então, se a gente só passasse o usuário pro BK e
-- deixasse os produtos/categorias/fornecedores antigos com RestaurantId =
-- NULL, eles sumiriam da tela desse usuário (NULL != Id do BK).
--
-- Por isso este script faz as DUAS coisas juntas, nesta ordem:
--   1) Liga ao BK todo usuário (AspNetUsers) que está com RestaurantId NULL.
--   2) Liga ao BK todo Product/Category/Supplier/StockMovement que também
--      está com RestaurantId NULL (é o dado "órfão" que esses usuários já
--      tinham antes do multi-tenant existir).
--
-- Repara que o passo 2 liga TODO dado órfão ao BK, e não só o dado "de" um
-- usuário específico. É que antes do multi-tenant não existia essa amarração
-- dono→dado, então não tem como saber de quem era cada linha. Se você tiver
-- mais de uma conta antiga E elas deveriam ser de restaurantes diferentes,
-- não roda este script direto — ele junta tudo num restaurante só (o BK).
--
-- Pra rodar no SQL Server Management Studio:
--   1. Conecta no banco RestauranteEstoqueDb.
--   2. Confere se o Id do BK lá embaixo (@RestaurantId) é o mesmo que
--      aparece em SELECT * FROM Restaurants — se for diferente, troca.
--   3. Cola este arquivo inteiro e aperta Executar (F5), uma vez só.
--   4. Todo usuário afetado tem que logar de novo (POST /auth/login) pra
--      pegar um token novo, já com a claim "restaurantId" certa. Isso porque
--      o token antigo foi montado sem essa claim (ou com o valor antigo) e
--      continua assim até expirar.
-- =============================================================

USE RestauranteEstoqueDb;
GO

DECLARE @RestaurantId UNIQUEIDENTIFIER = 'C0DABCE3-358C-41C8-8F04-931491344CB4'; -- esse é o Id do BK

IF NOT EXISTS (SELECT 1 FROM Restaurants WHERE Id = @RestaurantId)
BEGIN
    RAISERROR('Nenhum restaurante encontrado com esse Id — confira o Id do BK antes de rodar.', 16, 1);
    RETURN;
END

-- 1) Primeiro, as contas sem restaurante vão pro BK
UPDATE AspNetUsers
SET RestaurantId = @RestaurantId
WHERE RestaurantId IS NULL;

-- 2) Depois, os dados órfãos (criados antes do multi-tenant) também vão pro BK
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

-- Agora a gente confere: não pode sobrar nenhuma linha com RestaurantId
-- nulo em nenhuma das 5 tabelas.
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

-- E aqui dá pra ver quem ficou ligado ao BK
SELECT Email, RestaurantId
FROM AspNetUsers
WHERE RestaurantId = 'C0DABCE3-358C-41C8-8F04-931491344CB4';
GO
