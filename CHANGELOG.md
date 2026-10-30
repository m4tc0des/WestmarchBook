# Changelog - WestmarchBook

## [1.1.3] - 2026-10-30
### Adicionado
- **Cadastro de Exemplares Físicos:** Adicionada a entidade de domínio BookCopy e relacionamento 1:N com Book, permitindo o registro de múltiplos números de registro (RegistrationNumber) e controle de status (CopyStatus) para cada cópia física de uma mesma obra.
- **Submodal de Inclusão de Cópias (MAUI):** Nova interface interativa para inclusão sequencial de exemplares no cadastro de livros, com validações de unicidade, contador de cópias e feedback visual imediato.
- **Consultas Globais de Autores e Livros:** Implementação dos casos de uso e contratos de repositório GetAllAuthorsUseCase e GetAllBooksUseCase.

### Corrigido & Melhorado
- **Fluxo de Inicialização e Telas:** O sistema agora inicializa exclusivamente na tela de login (LoginWindow). A tela principal (MainWindow) só é instanciada e exibida maximizada após o primeiro login bem-sucedido, fechando a janela de autenticação de forma limpa.
- **Controle e Enquadramento de Janelas:** Bloqueio das opções de minimizar, maximizar e fechar livremente nas telas de diálogo (LoginWindow e RegisterWindow), forçando a interação com o formulário e padronizando as proporções e molduras em 440x640 px.
- **Reordenação e Renderização de Atalhos:** Atalhos principais da barra superior reorganizados na sequência lógica F2 (Senha/Trocar Usuário), F7 (Livros) e F9 (Leitores).
- **Correção Visual de Repintura (WinUI 3):** Resolução do problema onde botões, textos e rótulos de atalhos só apareciam após passar o mouse (hover); aplicação de estados visuais Normal explícitos e recalibração de espaçamentos para evitar cortes no DirectComposition.
- **Resiliência e Tratamento de Exceções:** Eliminação de NullReferenceException no retorno do cadastro de livros e de InvalidCastException durante atualizações da árvore visual nativa.
- **Estratégia de Migrações (MySQL vs SQLite):** Separação do provisionamento do banco de dados, mantendo MigrateAsync() para MySQL (Docker) e EnsureCreatedAsync() para o ambiente SQLite standalone empacotado.

## [1.1.0] - 2026-10-29
### Corrigido & Melhorado
- **Armazenamento Local:** Migração do banco SQLite para `%LocalAppData%\WestmarchBook`, eliminando erros de permissão (`Read-only database`) em `Program Files`.
- **Persistência de Livros:** Correção no fluxo de edição (`SaveBook`), garantindo gravação real no banco.
- **Validação de Leitores:** Adicionada validação de duplicidade de CPF e E-mail.
- **Arquitetura MAUI:** Divisão do `MainViewModel` em ViewModels especializadas.