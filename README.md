# Restaurante Estoque

Sistema de controle de estoque para restaurante — API em .NET com CQRS/DDD e front-end em Next.js. Projeto de estudo full stack, construído do zero em etapas (domínio, CQRS, autenticação, regras de estoque, CRUD completo de categorias/fornecedores/produtos e movimentações).

## Stack

**Backend** — C# / .NET 10, Minimal API, CQRS com MediatR (Commands/Queries/Handlers), DDD com entidades ricas (regras de negócio na própria entidade, não em serviços externos), Entity Framework Core sobre SQL Server, FluentValidation, autenticação JWT com hash de senha via BCrypt, Repository Pattern, API versionada (`/api/v1`).

**Frontend** — Next.js (App Router) + TypeScript, Tailwind CSS, shadcn/ui, Axios, TanStack Query.

## Funcionalidades

- Cadastro e login de usuário (JWT)
- CRUD completo de Produtos, Categorias e Fornecedores
- Movimentação de estoque (entrada/saída) com validação de saldo
- Listagens paginadas e ordenáveis
- Dashboard com indicadores simples (produtos com estoque baixo, totais)

## Arquitetura

O backend segue Clean Architecture: `Domain` (entidades e regras de negócio) → `Application` (casos de uso via CQRS) → `Infrastructure` (EF Core, JWT, hash de senha) → `Api` (endpoints Minimal API, um módulo por entidade) → `Contracts` (DTOs compartilhados). As tabelas do banco são criadas por script SQL manual (sem EF Core Migrations).

## Como rodar localmente

**Pré-requisitos**: .NET 10 SDK, Node.js, SQL Server local.

1. Rode os scripts em `backend/Database/scripts/` (na ordem) no seu SQL Server para criar o banco e as tabelas.
2. Configure `backend/Api/appsettings.json` (ou variáveis de ambiente `Jwt__Secret` / `ConnectionStrings__DefaultConnection`) — veja [SECURITY.md](SECURITY.md) antes de rodar em qualquer ambiente que não seja a sua máquina.
3. Backend: `dotnet run --project backend/Api` (sobe em `https://localhost:7008`).
4. Frontend: crie `frontend/.env.local` com `NEXT_PUBLIC_API_URL=https://localhost:7008`, depois `npm install && npm run dev --prefix frontend` (sobe em `http://localhost:3000`).
5. Crie uma conta pela tela de cadastro (`/registrar`) ou use o INSERT de usuário de teste em `backend/Database/scripts/02-inserir-usuario-teste.sql`.
