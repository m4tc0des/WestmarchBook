# Changelog - WestmarchBook

## [2.2.0] - 2026-11-06
### Adicionado
- **Submodal de Seleção e Edição de Leitores (Novo Empréstimo):** Substituição do dropdown simples por uma janela modal dedicada com busca dinâmica por **Nome**, **CPF**, **E-mail**, **Telefone** ou **ID**. Inclui bloqueio visual e funcional para leitores com status *Bloqueado* ou inativos, além de botão **Editar** integrado para corrigir dados cadastrais durante o atendimento.
- **Submodal de Seleção de Livro e Exemplar Físico (Novo Empréstimo):** Fluxo de seleção em duas etapas onde o operador busca o livro (por Título, Autor, Editora ou ID) e escolhe especificamente qual cópia física (`RegistrationNumber`) será emprestada, exibindo exclusivamente exemplares com status **Disponível**.
- **Expansão dos Filtros de Pesquisa de Livros:** Adicionados os seletores de filtro por **Autor**, **Editora** e **Gênero** tanto na tela **Pesquisar** quanto na barra de busca da aba **Livros** (complementando *Título* e *ID*).

### Corrigido & Melhorado
- **Reorganização Ergonométrica das Telas (UX):** Movimentação das barras de botões de ação (`+ Incluir`, `Editar`, `Ativar/Inativar`, `Prorrogar`, `Devolver`, `Excluir`) do topo para a parte inferior — posicionadas imediatamente acima da barra de pesquisa — nas abas **Livros**, **Leitores** e **Empréstimos**, ampliando significativamente a área vertical das grades de visualização.
- **Barra de Pesquisa Compacta:** Ajuste na largura do campo de digitação (`Entry`) e aproximação do botão **Pesquisar** logo ao lado direito do campo nas abas de busca e listagem, reduzindo o deslocamento do mouse.
- **Validação Rigorosa de E-mail:** Endurecimento das regras de validação no cadastro e edição de leitores e usuários para bloquear endereços e extensões de domínio malformadas (ex.: `.cokm`), exibindo o alerta de validação na interface.
- **Ícone da Barra Lateral:** Correção do glifo de lupa no botão **Pesquisar** da *Sidebar*, alinhando-o ao padrão visual dos demais ícones do menu.
- **Limpeza de Código (Front-end):** Remoção de comentários residuais de desenvolvimento nos arquivos `.cs` e `.xaml` do projeto `WestmarchBook.Desktop`.

## [2.1.5] - 2026-11-06
### Corrigido & Melhorado
- **Performance de Consultas e Banco de Dados (EF Core / SQLite):**
  - **Eliminação de *N+1 Queries* em Empréstimos:** Otimizada a verificação de leitores inadimplentes em `LoadRentsAsync`, reutilizando a navegação `Reader` já carregada em memória e eliminando consultas repetidas por registro no banco.
  - **Prevenção de Explosão Cartesiana:** Adicionado `.AsSplitQuery()` nas consultas de livros (`GetAllActive` e `GetById`) que carregam múltiplos autores e exemplares simultaneamente, além de `.AsNoTracking()` em consultas de leitura de autores.
  - **Cache de Serialização JSON:** `JsonOptionsHelper.Default` convertido para instância estática somente-leitura (`static readonly`), evitando recriações contínuas do `JsonSerializerOptions` e reduzindo alocação de CPU/GC na leitura de gêneros.
- **Fluidez da Interface e Gestão de Memória (.NET MAUI):**
  - **Mapeamento Assíncrono Fora da UI Thread:** A construção e formatação das listas de Livros, Leitores e Empréstimos agora ocorrem em segundo plano antes da atualização visual, eliminando travamentos na navegação entre abas.
  - **Correção de Vazamento de Memória (*Memory Leak*):** Implementado `IDisposable` na `MainViewModel` para encerrar adequadamente o `PeriodicTimer` do relógio da barra de status ao trocar de sessão via atalho `F2`, além de cachear o `Brush` inativo da barra lateral.
  - **Inicialização e Recargas Otimizadas:** Removidas verificações redundantes de criação do banco SQLite no startup (`App.xaml.cs`) e chamadas duplicadas de recarga da lista de leitores após operações de empréstimo.
- **Consistência Transacional na Troca de Senha:** Injetado `IUnitOfWork` em `ChangePasswordUseCase` com chamada explícita a `Commit()`, garantindo que a alteração de senha seja persistida diretamente na camada de aplicação.
- **Ajuste Visual em Seletores de Empréstimo:** Corrigido o dimensionamento vertical e espaçamento do seletor (`Picker`) de *Exemplar Disponível*, impedindo que o texto selecionado fique cortado.

## [2.1.4] - 2026-11-05
### Corrigido & Melhorado
- **Tratamento de Erros e Validação no Login:** Eliminação do alerta genérico `"Erro desconhecido."` ao submeter campos vazios. O fluxo no cliente Desktop agora desserializa corretamente o payload de exceções do backend (`ErrorOnValidationException`), exibindo mensagens claras e precisas enviadas pela API (ex: obrigatoriedade da senha).
- **Atalho F2 e Reutilização de Janela:** Removido o badge visual flutuante (KeyTip nativo do WinUI 3) que surgia sobre a tela. A execução do atalho global `F2` agora reaproveita a tela oficial de autenticação da instituição para bloqueio e troca de operador, eliminando a janela genérica duplicada.
- **Limpeza de Estado em Modais (Form Reset):**
  - **Inclusão de Empréstimos:** Correção da retenção de dados da última operação. A modal agora sempre inicializa limpa, com os seletores de leitor e exemplar desmarcados (`null`) e placeholders adequados.
  - **Inclusão de Livros:** Os campos de seleção de Editora e Gênero deixaram de vir pré-preenchidos com o primeiro item da lista, iniciando vazios com os textos de instrução padrão.
- **Renderização e Clipping de Pickers:** Correção de espaçamentos e dimensionamento vertical nos seletores de Gênero e Editora, resolvendo o corte visual da linha de texto quando um item era selecionado.
- **Padronização Terminológica (Editora):** Adequação do vocabulário técnico e visual de *Publicadora* para **Editora** nas telas de cadastro, listagens e seletores, alinhando a interface aos padrões do mercado bibliotecário.
- **Ordenação na Busca de Leitores:** Aplicação de ordenação padrão crescente por identificador único (`ID ASC`) na consulta e renderização da grade de leitores.
- **Navegação Segura:** Padronização do botão **"Voltar"** em todas as submodais de cadastro e edição, garantindo o cancelamento limpo da ação sem retenção de estado na tela pai.

### Adicionado
- **Submodal de Gestão de Autores (N:N):** Substituição do dropdown simples por uma interface modal dedicada com listagem, busca com filtro dinâmico e suporte a múltiplos autores por obra, além de ações diretas para incluir, editar e excluir registros de autores.

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