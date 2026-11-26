# WestmarchBook

Sistema completo de gerenciamento de acervo literário, leitores e empréstimos, desenvolvido com foco em robustez, manutenibilidade e separação de responsabilidades, utilizando **Clean Architecture** e ecossistema **.NET 10**.

---

## Tecnologias e Arquitetura

* **Backend:** ASP.NET Core Web API (.NET 10)
* **Frontend Desktop:** .NET MAUI (`net10.0-windows10.0.19041.0`)
* **Persistência de Dados:** Entity Framework Core com suporte a **MySQL** (produção/Docker) e **SQLite** (modo local com isolamento em `%LocalAppData%\WestmarchBook`)
* **Mensageria:** RabbitMQ (`RabbitMQ.Client`) para processamento assíncrono em background
* **Armazenamento de Mídia:** Azure Blob Storage / Azurite (`Azure.Storage.Blobs`)
* **Envio de E-mails:** Integração com Brevo (`BrevoEmailService`)
* **Validação e Mapeamento:** FluentValidation (incluindo validações compartilhadas como `EmailValidator`, `PasswordValidator` e `ReaderValidatorExtensions`) e Mapster
* **Segurança:** Autenticação JWT (`AccessToken`), **Refresh Token** persistido com rotação, login externo com **Google OpenID Connect** e hashing de senhas com **Argon2id**
* **Testes:** xUnit, Shouldly, Moq, Bogus e **Testcontainers (MySQL)**
* **Empacotamento e Deploy:** `Dockerfile` multi-stage para a API e script Inno Setup (`gerar-instalador.ps1`) para instalador desktop

---

## Funcionalidades Implementadas

### Autenticação, Sessão e Recuperação de Conta (`AuthenticationController`)
* **Login com e-mail e senha** (`POST /authentication/login`) com retorno de `AccessToken` (JWT) e `RefreshToken`.
* **Renovação de sessão via Refresh Token** (`POST /authentication/token/refresh`) com expiração configurada em 15 dias e substituição do token anterior no banco de dados; respostas `401` sinalizam `accessTokenExpired: true` quando o JWT expira.
* **Login externo com Google** (`GET /authentication/external-login/google/start` e `POST /authentication/external-login/exchange`) utilizando código temporário (`VerificationCode`) e redirecionamento configurável para Web ou App Desktop.
* **Recuperação de senha** (`POST /authentication/password-recovery` e `POST /authentication/password-recovery/reset`) com envio de código de verificação por e-mail.

### Usuários e Exclusão Assíncrona de Conta (`UsersController`)
* Cadastro de conta (`POST /users`) com emissão imediata de `AccessToken` e `RefreshToken`.
* Consulta de perfil (`GET /users`), atualização de dados cadastrais (`PATCH /users/profile`) e alteração de senha (`PATCH /users/password`).
* Upload de foto de perfil (`PUT /users/profile-picture`) armazenada no Azure Blob Storage.
* **Exclusão de conta assíncrona** (`DELETE /users`): desativa a conta imediatamente e publica uma mensagem na fila `account-deletion` do RabbitMQ, consumida pelo `DeleteUserAccountConsumer` para remoção definitiva do usuário e dados vinculados.

### Acervo Literário (`BooksController`, `AuthorsController`, `PublishersController`)
* **Livros:** cadastro via `multipart/form-data` com capa e exemplares físicos (`BookCopy` com `RegistrationNumber` único), atualização (`PATCH /books/{id}`), consulta (`GET /books/{id}`), ativação/inativação (`PATCH /books/active/{id}` e `PATCH /books/inactive/{id}`), filtro avançado (`POST /books/filter`), exclusão (`DELETE /books/{id}`) e upload/remoção de capa (`PUT`/`DELETE /books/{id}/illustration`).
* **Autores:** cadastro, listagem, busca por ID, atualização (`multipart/form-data`), upload/remoção de ilustração (`PUT`/`DELETE /authors/{id}/illustration`) e exclusão (com bloqueio quando há livros vinculados).
* **Editoras:** cadastro, listagem, busca por ID, atualização e exclusão (com bloqueio quando há livros vinculados).

### Leitores e Empréstimos (`ReadersController`, `RentsController`)
* **Leitores:** cadastro (`POST /readers`) e atualização (`PATCH /readers/profile/{id}`), com validação de e-mail, telefone, CPF (otimizada com `ReadOnlySpan<char>`) e municípios da Bahia.
* **Empréstimos:** registro de empréstimo por exemplar físico disponível (`POST /rents`) e devolução (`PATCH /rents/{id}/return`).

### Aplicação Desktop (.NET MAUI)
* Interface organizada com `ViewModels` especializadas (`LoginViewModel`, `RegisterViewModel`, `MainViewModel`, `BooksViewModel`, `BookSearchViewModel`, `AuthorsViewModel`, `PublishersViewModel`, `ReadersViewModel`, `RentsViewModel` e `UserProfileViewModel`).
* Suporte a autenticação externa via `GoogleLoginWindow`, seleção local de imagens (`MauiPhotoPickerService`) e gerenciamento de token (`DesktopTokenProvider`).

---

## Estrutura do Projeto

* `src/Backend/WestmarchBook.Api` - API RESTful, autenticação JWT/Google, `UseRefreshTokenUseCase` e `DeleteUserAccountConsumer`.
* `src/Backend/WestmarchBook.Application` - Casos de uso, mapeamentos (Mapster) e validadores (FluentValidation).
* `src/Backend/WestmarchBook.Domain` - Entidades (`Book`, `BookCopy`, `Author`, `Publisher`, `Reader`, `Rent`, `User`, `RefreshToken`, `VerificationCode`), enums, DTOs e contratos.
* `src/Backend/WestmarchBook.Infrastructure` - `WestmarchBookDbContext` (MySQL/SQLite), repositórios, migrações, Argon2id, tokens JWT/RefreshToken, RabbitMQ, Brevo e Azure Blob Storage.
* `src/Shared/WestmarchBook.Communication` - Contratos de requisição/resposta.
* `src/Shared/WestmarchBook.Exception` - Exceções de domínio e recursos de mensagens localizadas (`ResourceMessagesException`).
* `src/Frontend/WestmarchBook.Desktop` - Aplicação desktop multiplataforma construída em .NET MAUI.
* `tests/CommonTestUtilities` - Builders e utilitários compartilhados de teste.
* `tests/Validators.Tests` - Testes de unidade dos validadores.
* `tests/UseCases.Tests` - Testes de unidade dos casos de uso (incluindo fluxo de `RefreshToken` e exclusão de conta).
* `tests/WebApi.Tests` - Testes de integração HTTP end-to-end com `WebApplicationFactory` e Testcontainers MySQL.
* `tests/Desktop.Tests` - Testes de unidade das ViewModels e serviços do aplicativo Desktop.

---

## Versões e Atualizações

Para acompanhar o histórico detalhado de versões e melhorias da interface desktop, consulte o [CHANGELOG.md](./CHANGELOG.md).

---

## Como Executar o Projeto Localmente

1. Clone o repositório para a sua máquina.
2. Certifique-se de ter o **.NET 10 SDK** instalado.
3. Configure as definições necessárias em `appsettings.json` / `appsettings.Development.json` da API (banco MySQL ou SQLite local, chave JWT `JsonWebToken:SigningKey`, e opcionalmente `RabbitMq`, `BlobStorage`, `Authentication:Google` e serviço de e-mail).
4. Execute a API (`src/Backend/WestmarchBook.Api`) ou publique via `Dockerfile`.
5. Para executar os testes:
   * Testes unitários: `dotnet test tests/Validators.Tests/Validators.Tests.csproj` e `dotnet test tests/UseCases.Tests/UseCases.Tests.csproj`
   * Testes de integração (requer Docker ativo para Testcontainers MySQL): `dotnet test tests/WebApi.Tests/WebApi.Tests.csproj -- xUnit.ParallelizeTestCollections=false`
