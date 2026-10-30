-- Etapa M8d: o catálogo local de produtos da Vendas. A Vendas não pergunta mais
-- pro Estoque (por HTTP) o nome e o preço na hora da venda: ela lê daqui. Quem
-- mantém essa tabela em dia é o ProdutoCatalogoConsumidor, com os avisos que o
-- Estoque manda na catalogo-produtos-queue.
--
-- Repara que não tem FK pra estoque.Produtos: cada API cuida só do próprio
-- schema, e a ligação é pela mensagem.
--
-- A parte 2 é a CARGA INICIAL: os produtos que já existiam antes da M8d nunca
-- geraram aviso, então a gente copia eles uma vez daqui mesmo. Isso só dá porque
-- as duas APIs usam o mesmo banco; com bancos separados, o jeito seria o Estoque
-- republicar todos os produtos na fila.
--
-- Sem USE no topo (igual o 15 a 20). Pode rodar mais de uma vez (idempotente).

SET QUOTED_IDENTIFIER ON;

IF DB_NAME() = 'master'
BEGIN
    RAISERROR('Você está no banco master. Selecione o RestauranteEstoqueDb na janela e rode de novo.', 16, 1);
    SET NOEXEC ON;
END;
GO

IF OBJECT_ID('vendas.ProdutosCatalogo', 'U') IS NULL
BEGIN
    CREATE TABLE vendas.ProdutosCatalogo
    (
        Id            UNIQUEIDENTIFIER NOT NULL CONSTRAINT PK_ProdutosCatalogo PRIMARY KEY,
        RestauranteId UNIQUEIDENTIFIER NOT NULL,
        Nome          NVARCHAR(150)    NOT NULL,
        Preco         DECIMAL(18,2)    NOT NULL,
        Excluido      BIT              NOT NULL,
        AtualizadoEm  DATETIME2        NOT NULL
    );

    CREATE INDEX IX_ProdutosCatalogo_RestauranteId ON vendas.ProdutosCatalogo (RestauranteId);
END;
GO

-- Carga inicial: só entra quem ainda não está no catálogo (por isso dá pra rodar
-- de novo sem duplicar). O AtualizadoEm vai com a hora de agora: qualquer aviso
-- que o Estoque mandar daqui pra frente é mais novo e passa.
INSERT INTO vendas.ProdutosCatalogo (Id, RestauranteId, Nome, Preco, Excluido, AtualizadoEm)
SELECT p.Id, p.RestauranteId, p.Nome, p.Preco, 0, SYSUTCDATETIME()
FROM estoque.Produtos AS p
WHERE NOT EXISTS (SELECT 1 FROM vendas.ProdutosCatalogo AS c WHERE c.Id = p.Id);
GO

SET NOEXEC OFF;
