# Changelog - WestmarchBook

## [2.1.3] - 2026-11-05
### Adicionado
- **Nova Identidade Visual e Dashboard:** Redesign visual completo baseado nas cores oficiais azul e branco da Biblioteca Comunitária Filarmônica 30 de Junho. Inclusão da logomarca centralizada em alta resolução e barra lateral (*Sidebar*) escura com ícones amplos inspirados na usabilidade e fluidez do Docker Desktop.
- **Painel de Métricas Operacionais:** Dashboard inicial com 5 cards dinâmicos estruturados: *Total de Livros*, *Membros Ativos*, *Empréstimos Próximos ao Vencimento* (próximos 7 dias), *Empréstimos Vencidos* e *Leitores Bloqueados*.
- **Barra de Status (Status Bar):** Faixa de rodapé com monitoramento em tempo real contendo dados de conexão (`Servidor: 127.0.0.1`, nome da base de dados, usuário logado em caixa alta), exibição da versão `v.2.1.3` e relógio dinâmico à direita.
- **Navegação Inteligente por Foco (Enter):** Suporte global à navegação por teclado nos formulários (Login, Leitores, Livros e Pesquisa), permitindo transição contínua entre campos e disparo de ações automáticas ao pressionar a tecla `Enter`.
- **Módulo de Pesquisa com Filtros:** Nova tela de busca com seleção via seletores circulares (*RadioButtons*) por Nome ou ID, exibição categorizada de metadados/gênero e listagem modal de exemplares físicos com indicadores de disponibilidade por número de registro.
- **Empacotamento e Distribuição Standalone:** Configuração de publicação nativa via CLI (`net10.0-windows10.0.19041.0`, `self-contained`, sem empacotamento MSIX) e criação do script Inno Setup para geração do instalador executável `Instalador_Biblioteca_Filarmonica.exe`.

### Corrigido & Melhorado
- **Customização e Controle de Janelas:** Implementação de *Custom TitleBar* com o título centralizado `"USO EXCLUSIVO DA FILARMÔNICA 30 DE JUNHO"`, com bloqueio do botão nativo de maximizar/redimensionar (mantendo dimensões fixas com apenas minimizar e fechar).
- **Proteção e Encerramento Seguro:** Remoção do botão "X" nativo da tela de autenticação e interceptação do evento de fechamento da janela principal e de login, exigindo confirmação explícita do usuário (*"Deseja realmente sair do sistema?"*) antes de finalizar o processo.
- **Módulo de Empréstimos (Backend):** Conclusão das regras de negócio, contratos de repositório, validadores assíncronos e casos de uso para o fluxo de aluguel e retenção de leitores em atraso.

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