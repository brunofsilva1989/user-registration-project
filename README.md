# UserRegistration API — Guia de Execução e Testes

Projeto **.NET 8 Web API** com **CQRS + MediatR + EF Core** e **JWT Bearer**. Permite **cadastro**, **listagem** e **autenticação** de usuários.

> Endpoints principais:
> - `POST /api/auth/register` – cadastra usuário (e-mail único)
> - `POST /api/auth/login` – autentica e retorna **JWT**
> - `GET /api/users` – lista (requer **Bearer token**)
> - `GET /api/users/{id}` – detalhe (requer **Bearer token**)

---

## 1) Tecnologias usadas
- **.NET 8** / **ASP.NET Core Web API**
- **EF Core 8** (SQL Server) + **Migrations**
- **CQRS + MediatR**
- **JWT Bearer Auth**
- **xUnit + Moq + FluentAssertions** (testes unitários)
- **Logging** (ILogger)

---

## 2) Pré‑requisitos
- **.NET SDK 8.0+** (`dotnet --version`)
- **SQL Server** (LocalDB/Express/instância local ou container)
- (Opcional) **Docker**

> Dica: no Windows, o **LocalDB** já resolve bem: `Server=(localdb)\\MSSQLLocalDB`.

---

## 3) Configuração

### 3.1 `appsettings.Development.json`
Defina **ConnectionString** e **JWT** (padrão sensato para DEV):

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\MSSQLLocalDB;Database=UserRegistrationDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
  },
  "Jwt": {
    "Issuer": "UserRegistration",
    "Audience": "UserRegistration",
    "Key": "CHAVE-SUPER-SECRETA-DEV" 
  },
  "AllowedHosts": "*"
}
```

> **Produção**: use **variáveis de ambiente** (`ConnectionStrings__DefaultConnection`, `Jwt__Key`, etc.).

### 3.2 Criar/atualizar o banco (Migrations)
Na raiz do repositório:
```bash
dotnet tool install --global dotnet-ef --version 8.*  # se ainda não tiver
dotnet restore
dotnet ef database update --project UserRegistration.Infrastructure --startup-project UserRegistration.API
```

> As migrations ficam em `UserRegistration.Infrastructure/Migrations`.

---

## 4) Executar a API

```bash
dotnet run --project UserRegistration.API
```
Por padrão: `http://localhost:5000` e `https://localhost:5001`.

Habilite CORS para o front (ex.: `http://localhost:4200`) já está configurado via política `AllowSpa` no `Program.cs`.

---

## 5) Testando com curl / HTTPie / Postman

### 5.1 Registrar usuário
```bash
curl -X POST http://localhost:5000/api/auth/register   -H "Content-Type: application/json"   -d '{"login":"jsilva","firstName":"João","lastName":"Silva","email":"joao@exemplo.com","password":"P@ssw0rd"}'
```

### 5.2 Login e pegar token
```bash
TOKEN=$(curl -s -X POST http://localhost:5000/api/auth/login   -H "Content-Type: application/json"   -d '{"login":"jsilva","password":"P@ssw0rd"}' | jq -r .token)
echo $TOKEN
```

### 5.3 Listar usuários (rota protegida)
```bash
curl http://localhost:5000/api/users -H "Authorization: Bearer $TOKEN"
```

---

## 6) Testes automatizados (xUnit)

### 6.1 Pacotes (caso precise adicionar)
```bash
dotnet add UserRegistration.UnitTests package Moq
dotnet add UserRegistration.UnitTests package FluentAssertions
```

### 6.2 Executar testes
```bash
dotnet test
```

> Exemplo de teste incluso: **RegisterUserCommandHandlerTests** (verifica criação com e-mail novo e exceção com e-mail existente).  
> Atenção: o `IPasswordHasher.Hash` retorna **byte[]**; o mock usa `Encoding.UTF8.GetBytes(...)`.

---

## 7) Docker (opcional)

### 7.1 Build
```bash
docker build -t userregistration.api ./UserRegistration.API
```

### 7.2 Run
```bash
docker run -p 5000:8080   -e ConnectionStrings__DefaultConnection="Server=host.docker.internal;Database=UserRegistrationDb;User Id=sa;Password=SuaSenha123;TrustServerCertificate=True"   -e Jwt__Key="CHAVE-FORTE-PROD"   userregistration.api
```

---

## 8) Problemas comuns (Troubleshooting)
- **401/403**: faltou `Authorization: Bearer <token>` ou `Issuer/Audience/Key` inconsistentes.
- **Falha na migration**: confira ConnectionString/permissões; rode `dotnet ef database update` com os projetos corretos (`--project` e `--startup-project`).
- **CORS** no front: garanta origem `http://localhost:4200` na política de CORS.


#############################################################################################################################################################
# UserRegistration Web (Angular 19) — Guia de Execução

Frontend em **Angular 19** com **Standalone Components**, **Interceptor de JWT** e **Guard** para rotas protegidas.

---

## 1) Tecnologias usadas
- **Angular 19.x**
- **TypeScript 5.x**
- **RxJS 7.8+**
- **Angular Router**
- **HTTP Client com Interceptor (JWT)**

---

## 2) Pré‑requisitos (patch/versões mínimas)
- **Node.js **20 LTS** - Verifique com `node -v`.
- **npm 9+**. Verifique com `npm -v`.
- **Angular CLI 19.0.x** (instale com):
  ```bash
  npm i -g @angular/cli@19
  ```

> Dica: se o projeto já tem `package-lock.json`, prefira **`npm ci`** para instalar exatamente as versões travadas.

---

## 3) Configurar a URL da API
Edite `src/environments/environment.ts`:

```ts
export const environment = {
  production: false,
  apiBaseUrl: 'http://localhost:5000/api' // API da Web
};
```

Para build de produção (`environment.prod.ts`), ajuste conforme o host do backend.

---

## 4) Instalar dependências e subir

Na pasta do frontend:

```bash
# instala dependências
npm ci          # ou: npm install

# sobe em modo dev
npm start       # alias para: ng serve --open
# ou explicitamente:
ng serve --host 0.0.0.0 --port 4200
```

Acesse: **http://localhost:4200**.

## 5) Fluxo rápido para testar

1. **Cadastre-se** via `POST /api/auth/register` na API ou pela tela de cadastro (se existir).  
2. **Login** → salva o token (LocalStorage).  
3. A **listagem de usuários** deve carregar com **Bearer** sendo enviado automaticamente pelo **interceptor**.

## 6) Build de produção
```bash
ng build
# artefatos em: dist/<nome-do-projeto>/
```

## 7) Problemas comuns (Troubleshooting)
- **401/403**: token ausente/expirado; verifique o interceptor e a origem do CORS no backend.
- **CORS**: garanta que a API permite a origem `http://localhost:4200`.
- **Versões incompatíveis**: confira `node -v`, `npm -v` e `ng version`. Reinstale CLI `@angular/cli@19` se necessário.
