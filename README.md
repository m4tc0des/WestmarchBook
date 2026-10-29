# WestmarchBook

Sistema completo de gerenciamento de acervo literário e leitores, desenvolvido com foco em robustez, manutenibilidade e separação de responsabilidades, utilizando **Clean Architecture** e ecossistema **.NET**.

---

## Tecnologias e Arquitetura

O projeto é dividido em soluções modernas para garantir alta performance e escalabilidade tanto no backend quanto na interface desktop:

* **Backend:** ASP.NET Core Web API (.NET)
* **Frontend Desktop:** .NET MAUI
* **Persistência de Dados:** Entity Framework Core com suporte a **MySQL** (produção/Docker) e **SQLite** (modo local/demonstração com isolamento no `%LocalAppData%`).
* **Validação de Domínio:** FluentValidation
* **Segurança:** Autenticação JWT e Hashing de senhas com Argon2id.
* **Testes:** Testcontainers e testes de integração automatizados.

---

## Estrutura do Projeto

* `src/Backend/WestmarchBook.Api` — API RESTful em .NET.
* `src/Backend/WestmarchBook.Application` — Casos de uso e regras de negócio.
* `src/Backend/WestmarchBook.Domain` — Entidades, enums e contratos de repositório.
* `src/Backend/WestmarchBook.Infrastructure` — Configuração do EF Core, migrações e persistência.
* `src/Frontend/WestmarchBook.Desktop` — Aplicação desktop multiplataforma construída em .NET MAUI.

---

## Versões e Atualizações

Para acompanhar todas as mudanças, correções e novas features implementadas em cada versão do sistema, consulte o nosso [CHANGELOG.md](./CHANGELOG.md).

---

## Como Executar o Projeto Localmente

1. Clone o repositório para a sua máquina.
2. Certifique-se de ter o **.NET SDK** instalado.
3. Para rodar a API com o banco local de demonstração (SQLite), configure a connection string apontando para o seu diretório local ou execute o instalador gerado via Inno Setup.
