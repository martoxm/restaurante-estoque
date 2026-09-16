# Segurança / configuração

Este repositório não versiona nenhum segredo real. Antes de rodar em qualquer ambiente que não seja a sua máquina local de desenvolvimento:

- **`backend/Api/appsettings.json` → `Jwt:Secret`**: o valor commitado é só um placeholder de exemplo. Defina o valor real via variável de ambiente (`Jwt__Secret`) ou `dotnet user-secrets set "Jwt:Secret" "<valor>"` — nunca edite `appsettings.json` com o segredo real.
- **`backend/Api/appsettings.json` → `ConnectionStrings:DefaultConnection`**: por padrão usa autenticação integrada do Windows (`Trusted_Connection=True`), sem usuário/senha. Se o seu ambiente exigir usuário/senha do SQL Server, defina a connection string também via variável de ambiente (`ConnectionStrings__DefaultConnection`), nunca direto no arquivo.
- **`frontend/.env.local`**: não é versionado (veja `.gitignore`). Crie o seu localmente com `NEXT_PUBLIC_API_URL` apontando para a API.
- Os scripts em `backend/Database/scripts/` criam um usuário de teste (`teste@restaurante.com`) com senha e hash de exemplo — não é uma conta real, só serve para testar o login localmente após rodar os scripts.
