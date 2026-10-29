# Restaurante Estoque

Sistema de controle de estoque para restaurantes — API em .NET com CQRS/DDD e front-end em Next.js. Vários restaurantes usam a mesma API e o mesmo banco, e cada um só enxerga o próprio estoque (multi-tenant). Projeto de estudo full stack, construído do zero em etapas.

## Stack

**Backend** — C# / .NET 10, Minimal API, CQRS com MediatR (Commands/Queries/Handlers), DDD com entidades ricas (regras de negócio na própria entidade, não em serviços externos), Entity Framework Core sobre SQL Server, FluentValidation, autenticação JWT com ASP.NET Core Identity, autorização por roles (`Admin`/`Funcionario`), Repository Pattern, API versionada (`/api/v1`), testes com xUnit.

**Frontend** — Next.js (App Router) + TypeScript, Tailwind CSS, shadcn/ui, Axios, TanStack Query, Recharts, tema claro/escuro.

## Funcionalidades

- Cadastro e login (JWT). Cada cadastro cria um restaurante novo, e o usuário só vê os dados dele
- CRUD completo de Produtos, Categorias e Fornecedores
- Movimentação de estoque (entrada/saída) com validação de saldo
- Aviso por e-mail aos administradores do restaurante quando um produto fica com estoque baixo
- Permissões por role: editar/excluir Categoria e Fornecedor e excluir Produto são exclusivos de `Admin`; todo cadastro novo nasce `Funcionario`
- Listagens paginadas, com filtro e ordenação
- Dashboard com totais, produtos com estoque baixo, nome do restaurante e gráficos (produtos por categoria e estoque por categoria)
- Data de criação (`CreatedAt`) em produtos, categorias, fornecedores e restaurantes
- Layout responsivo com menu mobile e modo escuro

## Arquitetura

O repositório está se preparando para virar microserviços:

- `backend/services/Identidade` — usuários, roles e restaurantes; é a única API que gera o token JWT
- `backend/services/Estoque` — produtos, categorias, fornecedores e movimentações; só valida o token
- `backend/shared/Core` — código comum a todas as APIs: classe base `Entity`, Value Objects, validação, tratamento de erros, validação do JWT e CorrelationId
- `frontend` — o app Next.js

Cada API segue Clean Architecture, com as dependências apontando para dentro:

- `Domain` — entidades e regras de negócio, sem depender de nenhum framework
- `Application` — casos de uso (Commands, Queries e Handlers do MediatR), validators do FluentValidation e as interfaces que a Infrastructure implementa. Não depende do EF Core
- `Infrastructure` — EF Core, repositórios e relógio (`IRelogio`); na Identidade, também o ASP.NET Core Identity e a geração do JWT
- `Api` — endpoints Minimal API, um módulo por entidade, middleware de erros e Swagger
- `Contracts` — DTOs de entrada e saída, compartilhados entre as camadas

Alguns detalhes:

- **Validação** roda num pipeline behavior do MediatR, antes de qualquer Handler. Erros viram respostas `ProblemDetails` (400, 401, 404) num middleware único.
- **Multi-tenant**: o `RestaurantId` vem de uma claim do JWT e é aplicado como Global Query Filter do EF Core em toda leitura. Testes de isolamento garantem que um restaurante não lê os dados de outro.
- **Eventos de domínio + Outbox**: o `Produto` registra um `EstoqueFicouBaixoEvent`; um interceptor do EF publica o evento e o handler grava o aviso na tabela `OutboxEmails` na mesma transação; um `BackgroundService` envia o e-mail (SMTP com MailKit) em segundo plano, com nova tentativa se falhar.
- **Banco**: um banco só, com um schema por API (`identidade` e `estoque`) e nenhuma FK de uma API para a outra. As tabelas de negócio são criadas por script SQL manual. Só as tabelas do Identity (`AspNet*`) usam EF Core Migration, no projeto da Identidade.
- **Token compartilhado**: a Identidade assina o JWT (HS256) e o Estoque valida com a mesma chave, sem chamar a Identidade a cada requisição.

O frontend fica em `frontend/src`, organizado por funcionalidade (`features/produtos`, `features/categorias`, `features/auth`, ...).

## Como rodar localmente

**Pré-requisitos**: .NET 10 SDK, Node.js, SQL Server local e a ferramenta do EF Core (`dotnet tool install --global dotnet-ef`).

### 1. Banco de dados

Rode os scripts de `backend/services/Estoque/Database/scripts/` no SQL Server, **nesta ordem**:

1. `01-criar-tabelas.sql` — cria o banco `RestauranteEstoqueDb` e as tabelas principais
2. `03-criar-tabela-fornecedores.sql`
3. `05-remover-tabela-users.sql` — apaga a tabela de usuários antiga (os usuários agora vivem nas tabelas do Identity)

Depois, crie as tabelas do ASP.NET Core Identity pela migration:

```bash
dotnet ef database update --project backend/services/Identidade/Infrastructure --startup-project backend/services/Identidade/Api
```

E termine com os scripts que dependem delas:

4. `07-criar-tabela-restaurants.sql`
5. `08-adicionar-restaurantid.sql` — liga produtos, categorias, fornecedores, movimentações e usuários a um restaurante
6. `09-adicionar-createdat.sql`
7. `11-traduzir-banco-pt-br.sql` — traduz tabelas e colunas para português (`Produtos`, `Categorias`, `Fornecedores`, `MovimentacoesEstoque`, `Restaurantes`), o mesmo vocabulário do código
8. `12-criar-tabela-outbox-emails.sql` — caixa de saída dos e-mails de aviso de estoque baixo
9. `13-restauranteid-obrigatorio.sql` — torna `RestauranteId` obrigatório (`NOT NULL`) em produtos, categorias, fornecedores e movimentações; para sem alterar nada se ainda houver linha sem restaurante (rode o `10-vincular-dados-orfaos-restaurante-bk.sql` antes, se o banco tiver dados antigos)
10. `14-separar-schemas-por-api.sql` — move as tabelas para os schemas `identidade` e `estoque` e tira as FKs entre as duas APIs

### 2. Configuração

Os `appsettings.json` das duas APIs (`backend/services/Identidade/Api` e `backend/services/Estoque/Api`) já vêm prontos para a máquina local (SQL Server em `localhost` com autenticação do Windows). As duas usam o mesmo *user-secrets*, então o que você guardar lá vale para as duas. Fora da sua máquina, nunca use os valores do arquivo: defina o segredo do JWT e a connection string por variável de ambiente (`Jwt__Secret` e `ConnectionStrings__DefaultConnection`) — o `Jwt` tem que ser igual nas duas APIs.

**E-mail de estoque baixo (opcional).** Quando um produto fica abaixo de 10 unidades, a API manda um e-mail para os `Admin` daquele restaurante. Para ligar isso com uma conta Gmail, crie uma [senha de app](https://myaccount.google.com/apppasswords) (exige verificação em duas etapas) e guarde as credenciais no *user-secrets* da API, nunca no `appsettings.json`:

```bash
dotnet user-secrets set "Email:Usuario" "seu.email@gmail.com" --project backend/services/Estoque/Api
```

```bash
dotnet user-secrets set "Email:Senha" "senha-de-app-de-16-letras" --project backend/services/Estoque/Api
```

Sem isso a API funciona normalmente; o aviso só fica na fila (tabela `OutboxEmails`) e o log avisa que o SMTP não está configurado.

> O envio está pausado por enquanto: os e-mails dos `Admin` passaram a morar na API de Identidade, e o Estoque ainda não sabe perguntar para ela. O aviso continua sendo gravado na fila e será enviado quando essa ligação existir.

### 3. Backend

Na primeira vez, confie no certificado HTTPS de desenvolvimento do .NET:

```bash
dotnet dev-certs https --trust
```

Depois, suba as duas APIs com o perfil `https`, cada uma num terminal:

```bash
dotnet run --project backend/services/Identidade/Api --launch-profile https
```

```bash
dotnet run --project backend/services/Estoque/Api --launch-profile https
```

A Identidade sobe em `https://localhost:7010` e o Estoque em `https://localhost:7008`, cada uma com o Swagger em `/swagger`. Login e cadastro ficam na Identidade; o token que ela devolve serve também no Estoque. Sem o `--launch-profile https`, o `dotnet run` usa o perfil `http` (Identidade em `http://localhost:5264`, Estoque em `http://localhost:5262`).

### 4. Frontend

Crie `frontend/.env.local` com o endereço das duas APIs:

```
NEXT_PUBLIC_API_URL=https://localhost:7008
NEXT_PUBLIC_API_IDENTIDADE_URL=https://localhost:7010
```

O `NEXT_PUBLIC_API_URL` é o Estoque; login, cadastro e o nome do restaurante vão para a Identidade.

Depois:

```bash
npm install --prefix frontend
```

```bash
npm run dev --prefix frontend
```

O app sobe em `http://localhost:3000`.

### 5. Primeiro acesso

Crie uma conta pela tela de cadastro (`/registrar`). O cadastro pede o nome do restaurante, cria o restaurante e já faz login.

Toda conta nova nasce com a role `Funcionario`. Para virar `Admin`, abra `backend/services/Estoque/Database/scripts/06-promover-usuario-admin.sql`, troque o e-mail pelo seu, rode o script e faça login de novo (o token novo já vem com a role atualizada).

## Testes

```bash
dotnet test backend/RestauranteEstoque.slnx
```

Cobrem regras de domínio, Handlers (com banco em memória) e o isolamento de dados entre restaurantes.
