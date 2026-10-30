-- Etapa M7a: o schema da API de Vendas. Mora aqui junto com os outros scripts
-- pra numeração continuar uma só (o 15 também é de outra API, a Identidade).
--
-- Repara que não tem FK pra Restaurantes nem pra Produtos: os dois são de
-- outras APIs (identidade e estoque), e entre APIs não tem FK nem JOIN. A única
-- FK é a de dentro do próprio schema: item -> venda, com cascade.
--
-- Sem USE no topo (igual o 15 e o 16): abre a janela já conectado no banco
-- certo, local ou Azure. Pode rodar mais de uma vez (idempotente).

SET QUOTED_IDENTIFIER ON;

-- Trava: sem USE, o script roda no banco selecionado na janela, e o padrão do
-- SSMS é o master. Aí as tabelas iam parar no lugar errado sem erro nenhum.
-- O SET NOEXEC ON faz o resto do script ser ignorado, até nos lotes depois do GO.
IF DB_NAME() = 'master'
BEGIN
    RAISERROR('Você está no banco master. Selecione o RestauranteEstoqueDb na janela e rode de novo.', 16, 1);
    SET NOEXEC ON;
END;
GO

IF SCHEMA_ID('vendas') IS NULL
    EXEC('CREATE SCHEMA vendas');
GO

BEGIN TRANSACTION;

IF OBJECT_ID('vendas.Vendas', 'U') IS NULL
BEGIN
    CREATE TABLE vendas.Vendas
    (
        Id            UNIQUEIDENTIFIER NOT NULL CONSTRAINT PK_Vendas PRIMARY KEY,
        RestauranteId UNIQUEIDENTIFIER NOT NULL,
        -- O status vai como texto ("Pendente", "Confirmada", "Cancelada").
        Status        NVARCHAR(20)     NOT NULL,
        CriadoEm      DATETIME2        NOT NULL
    );

    -- A listagem sempre filtra pelo restaurante (Global Query Filter) e ordena
    -- pela data, então esse índice cobre exatamente essa consulta.
    CREATE INDEX IX_Vendas_RestauranteId_CriadoEm
        ON vendas.Vendas (RestauranteId, CriadoEm);
END;

IF OBJECT_ID('vendas.ItensVenda', 'U') IS NULL
BEGIN
    CREATE TABLE vendas.ItensVenda
    (
        Id            UNIQUEIDENTIFIER NOT NULL CONSTRAINT PK_ItensVenda PRIMARY KEY,
        VendaId       UNIQUEIDENTIFIER NOT NULL
            CONSTRAINT FK_ItensVenda_Vendas REFERENCES vendas.Vendas (Id) ON DELETE CASCADE,
        ProdutoId     UNIQUEIDENTIFIER NOT NULL,
        -- Cópia do nome e do preço do produto no momento da venda.
        NomeProduto   NVARCHAR(150)    NOT NULL,
        PrecoUnitario DECIMAL(10, 2)   NOT NULL,
        Quantidade    INT              NOT NULL
    );

    CREATE INDEX IX_ItensVenda_VendaId ON vendas.ItensVenda (VendaId);
END;

COMMIT TRANSACTION;
