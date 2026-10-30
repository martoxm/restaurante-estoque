-- Revisão de produção: índices que faltavam nas tabelas de negócio do Estoque.
--
-- Por que precisa: desde a Etapa 34, o Global Query Filter coloca um
-- WHERE RestauranteId = @x em TODA consulta. Sem índice nessa coluna, o SQL
-- Server lê a tabela inteira pra achar as linhas de um restaurante só. Com um
-- restaurante ninguém percebe, mas com cem a listagem vai ficando lenta.
--
-- Repara também que o SQL Server NÃO cria índice sozinho pra FK (diferente do
-- MySQL). Então o ExisteParaCategoriaAsync / o COUNT da listagem de categorias
-- (Produtos.CategoriaId) e o ExisteParaProdutoAsync (MovimentacoesEstoque.ProdutoId)
-- também liam a tabela toda.
--
-- Os de (RestauranteId, Nome) servem pro filtro E pra ordenação padrão por Nome
-- das listagens, ou seja, o banco já entrega na ordem, sem ordenar na hora.
--
-- Sem USE no topo (igual o 15 a 21). Pode rodar mais de uma vez (idempotente).

SET QUOTED_IDENTIFIER ON;

IF DB_NAME() = 'master'
BEGIN
    RAISERROR('Você está no banco master. Selecione o RestauranteEstoqueDb na janela e rode de novo.', 16, 1);
    SET NOEXEC ON;
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Produtos_RestauranteId_Nome' AND object_id = OBJECT_ID('estoque.Produtos'))
    CREATE INDEX IX_Produtos_RestauranteId_Nome ON estoque.Produtos (RestauranteId, Nome);

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Produtos_CategoriaId' AND object_id = OBJECT_ID('estoque.Produtos'))
    CREATE INDEX IX_Produtos_CategoriaId ON estoque.Produtos (CategoriaId);

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Categorias_RestauranteId_Nome' AND object_id = OBJECT_ID('estoque.Categorias'))
    CREATE INDEX IX_Categorias_RestauranteId_Nome ON estoque.Categorias (RestauranteId, Nome);

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Fornecedores_RestauranteId_Nome' AND object_id = OBJECT_ID('estoque.Fornecedores'))
    CREATE INDEX IX_Fornecedores_RestauranteId_Nome ON estoque.Fornecedores (RestauranteId, Nome);

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_MovimentacoesEstoque_RestauranteId' AND object_id = OBJECT_ID('estoque.MovimentacoesEstoque'))
    CREATE INDEX IX_MovimentacoesEstoque_RestauranteId ON estoque.MovimentacoesEstoque (RestauranteId);

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_MovimentacoesEstoque_ProdutoId' AND object_id = OBJECT_ID('estoque.MovimentacoesEstoque'))
    CREATE INDEX IX_MovimentacoesEstoque_ProdutoId ON estoque.MovimentacoesEstoque (ProdutoId);
GO

-- Conferência: tem que listar os 6 índices acima.
SELECT OBJECT_SCHEMA_NAME(i.object_id) + '.' + OBJECT_NAME(i.object_id) AS Tabela, i.name AS Indice
FROM sys.indexes i
WHERE i.name LIKE 'IX[_]%'
  AND OBJECT_SCHEMA_NAME(i.object_id) = 'estoque'
  AND OBJECT_NAME(i.object_id) IN ('Produtos', 'Categorias', 'Fornecedores', 'MovimentacoesEstoque')
ORDER BY Tabela, Indice;
