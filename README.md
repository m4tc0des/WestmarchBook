# WestmarchBook

Sistema completo de gerenciamento de biblioteca, com controle de acervo, exemplares físicos, leitores, empréstimos e autenticação, construído com **Clean Architecture** e **.NET 10** (API RESTful + aplicativo Desktop).

---

## O que o projeto resolve

O **WestmarchBook** centraliza a operação de uma biblioteca desde o cadastro de obras e cópias físicas até o acompanhamento de leitores e devoluções, combinando uma API desacoplada para produção (MySQL, mensageria e armazenamento em nuvem) com uma interface desktop multiplataforma em **.NET MAUI**.

---

## Principais Funcionalidades

* **Autenticação e Segurança:** Login com JWT (*Access Token* + *Refresh Token* com rotação automática), login social com **Google**, recuperação de senha por código enviado via e-mail e hashing com **Argon2id**.
* **Gestão de Usuários:** Cadastro, edição de perfil, upload de foto e **exclusão de conta assíncrona** processada em background via **RabbitMQ**.
* **Acervo Literário:** Gestão completa de livros, autores e editoras, com controle individual de **exemplares físicos**, filtros combinados e upload de capas e ilustrações no **Azure Blob Storage**.
* **Leitores e Empréstimos:** Cadastro de leitores com validação de CPF, telefone, e-mail e município, além de controle de empréstimos por exemplar disponível e devoluções.
* **Aplicativo Desktop (.NET MAUI):** Interface organizada com MVVM, métricas operacionais e integração com login Google e armazenamento local.

---

## Stack e Decisões Técnicas

* **Backend:** ASP.NET Core Web API (.NET 10) seguindo Clean Architecture (`Domain`, `Application`, `Infrastructure`, `Api`)
* **Banco de Dados:** Entity Framework Core com suporte a **MySQL** (produção/Docker) e **SQLite** (execução local)
* **Mensageria & Cloud:** RabbitMQ (`RabbitMQ.Client`), Azure Blob Storage (`Azure.Storage.Blobs`) e serviço transacional de e-mail (Brevo)
* **Validação & Mapeamento:** FluentValidation e Mapster
* **Qualidade e Testes:** Cobertura com testes unitários (`xUnit`, `Shouldly`, `Moq`, `Bogus`) e testes de integração end-to-end com banco real via **Testcontainers (MySQL)**
* **Deploy:** `Dockerfile` multi-stage para a API e script de instalador Windows para o cliente Desktop

---

## Estrutura da Solução

* `src/Backend/` - Camadas da API (`Domain`, `Application`, `Infrastructure`, `Api`)
* `src/Shared/` - Contratos de comunicação (`Communication`) e exceções/mensagens localizadas (`Exception`)
* `src/Frontend/WestmarchBook.Desktop/` - Aplicativo desktop em .NET MAUI
* `tests/` - Projetos de testes (`Validators.Tests`, `UseCases.Tests`, `WebApi.Tests`, `Desktop.Tests` e `CommonTestUtilities`)

---

## Como Executar

1. Clone o repositório e certifique-se de ter o **.NET 10 SDK** instalado.
2. Configure `appsettings.json` / `appsettings.Development.json` na API (MySQL ou SQLite e `JsonWebToken:SigningKey`; opcionalmente RabbitMQ, Blob Storage, Google OAuth e e-mail).
3. Execute `WestmarchBook.Api` ou suba via `Dockerfile`.
4. Rode os testes:
   * Unitários: `dotnet test tests/Validators.Tests/Validators.Tests.csproj` e `dotnet test tests/UseCases.Tests/UseCases.Tests.csproj`
   * Integração (requer Docker): `dotnet test tests/WebApi.Tests/WebApi.Tests.csproj -- xUnit.ParallelizeTestCollections=false`

Para o histórico detalhado de versões, consulte o [CHANGELOG.md](./CHANGELOG.md).
