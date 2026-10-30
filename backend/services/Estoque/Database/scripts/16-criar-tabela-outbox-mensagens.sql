-- Etapa M6: o Outbox virou genérico. A tabela estoque.OutboxEmails (Etapa 41),
-- que só servia pra e-mail, dá lugar à estoque.OutboxMensagens, que guarda
-- qualquer mensagem que vai pro RabbitMQ: a fila, o tipo (nome do contrato) e o
-- conteúdo em JSON.
--
-- Repara que não tem mais RestauranteId nem FK: o restaurante vai DENTRO do JSON
-- da mensagem. E sem USE no topo de propósito (igual o 15), pra rodar tanto no
-- banco local quanto no Azure — é só abrir a janela já conectado no banco certo.
--
-- Os avisos de e-mail que ainda estavam pendentes na OutboxEmails são perdidos
-- quando ela é apagada. Como é tudo dado de teste, tudo bem; se não fosse, a
-- gente copiaria as linhas pendentes antes do DROP.
--
-- Pode rodar mais de uma vez sem quebrar nada (idempotente).

-- O índice filtrado lá embaixo só pode ser criado com QUOTED_IDENTIFIER ligado.
-- O SSMS já liga sozinho, mas o sqlcmd vem com ele desligado — então a gente
-- garante aqui.
SET QUOTED_IDENTIFIER ON;

BEGIN TRANSACTION;

IF OBJECT_ID('estoque.OutboxMensagens', 'U') IS NULL
BEGIN
    CREATE TABLE estoque.OutboxMensagens
    (
        Id                 UNIQUEIDENTIFIER NOT NULL CONSTRAINT PK_OutboxMensagens PRIMARY KEY,
        Fila               NVARCHAR(100)    NOT NULL,
        Tipo               NVARCHAR(200)    NOT NULL,
        Conteudo           NVARCHAR(MAX)    NOT NULL,
        Tentativas         INT              NOT NULL,
        CriadoEm           DATETIME2        NOT NULL,
        ProximaTentativaEm DATETIME2        NOT NULL,
        ProcessadoEm       DATETIME2        NULL,
        UltimoErro         NVARCHAR(2000)   NULL
    );

    -- O worker sempre pergunta "quais ainda não foram publicadas e já podem sair?".
    -- O índice filtrado só guarda as pendentes, então fica pequeno e rápido.
    CREATE INDEX IX_OutboxMensagens_Pendentes
        ON estoque.OutboxMensagens (ProximaTentativaEm)
        WHERE ProcessadoEm IS NULL;
END;

IF OBJECT_ID('estoque.OutboxEmails', 'U') IS NOT NULL
    DROP TABLE estoque.OutboxEmails;

COMMIT TRANSACTION;
