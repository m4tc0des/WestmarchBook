# Changelog - WestmarchBook

## [1.1.0] - 2026-10-29
### Corrigido & Melhorado
- **Armazenamento Local:** Migração do banco SQLite para `%LocalAppData%\WestmarchBook`, eliminando erros de permissão (`Read-only database`) em `Program Files`.
- **Persistência de Livros:** Correção no fluxo de edição (`SaveBook`), garantindo gravação real no banco.
- **Validação de Leitores:** Adicionada validação de duplicidade de CPF e E-mail.
- **Arquitetura MAUI:** Divisão do `MainViewModel` em ViewModels especializadas.