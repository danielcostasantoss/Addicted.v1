# ADR-001 — Arquitetura Unity e Infraestrutura de Publicação

- **Status:** Proposto (aguardando validação da equipe)
- **Data:** 2026-11-15
- **Escopo:** Engine, estrutura do projeto, backend, CI/CD e publicação do *Addicted*

> Esta é uma **proposta**, não um registro de decisões tomadas em reunião. Ela parte das premissas abaixo; se alguma estiver errada, as recomendações devem ser revistas.

## Premissas

- Jogo **narrativo, single-player, offline**, organizado em capítulos (um por vício), todos convergindo para a reabilitação.
- Projeto Unity versionado em Git, com código proprietário.
- Lançamento inicial em **PC (Steam)**; outras plataformas depois.

## 1. Decisões de Arquitetura e Unity

| Tema | Recomendação | Motivo |
|---|---|---|
| **Versão da engine** | **Unity 6 LTS** (a versão LTS mais recente no início do projeto), travada por todo o desenvolvimento | LTS tem 2 anos de correções; trocar de versão no meio do projeto é risco desnecessário |
| **Render Pipeline** | **URP** | Roda bem em PC, Steam Deck, Switch e mobile; HDRP só vale para fotorrealismo em PC/console topo de linha |
| **Controle de versão** | Git + **Git LFS** para binários (`.psd`, `.fbx`, `.wav`, `.png`); *Asset Serialization = Force Text*; *Visible Meta Files*; **UnityYAMLMerge** como merge tool | Sem LFS o repositório incha rápido; texto permite diff e merge de cenas/prefabs |
| **Gerenciamento de assets** | **Addressables**, com um grupo por capítulo/vício | Carrega só o capítulo atual, reduz memória e facilita patches e DLCs |
| **Estrutura de código** | **Assembly Definitions** por módulo (`Core`, `Narrative`, `Gameplay`, `UI`, `Save`); **ScriptableObjects** para os dados de cada capítulo | Compila mais rápido, separa responsabilidades e permite que designers editem dados sem código |
| **Narrativa** | **Ink** (inkle) ou **Yarn Spinner**, com o roteiro em texto versionado | Roteiristas trabalham fora da Unity, o diff é legível e a ramificação de escolhas é nativa |
| **Pacotes base** | Input System, Cinemachine, Timeline (cutscenes), **Localization** (pt-BR + en desde o início) | Adicionar localização depois custa caro |
| **Save** | JSON versionado (campo `saveVersion`) em `Application.persistentDataPath` | Torna simples o Steam Cloud e a migração entre versões |
| **Otimização** | Profiler e Frame Debugger desde o protótipo; SRP Batcher ativo; compressão de textura por plataforma; orçamento de performance definido (ex.: 60 fps no Steam Deck) | Medir cedo é mais barato que otimizar no fim |

## 2. Infraestrutura e Publicação

| Tema | Recomendação | Motivo |
|---|---|---|
| **Multiplayer / servidores** | **Nenhum servidor dedicado.** Jogo 100% offline | Um jogo narrativo single-player não precisa de backend próprio, o que elimina custo fixo e manutenção |
| **Backend** | **Steamworks** (conquistas, Steam Cloud saves, estatísticas) via *Steamworks.NET*; acesso isolado atrás de uma interface `IPlatformServices` | A interface permite plugar depois GOG, consoles ou mobile sem reescrever o jogo |
| **Unity Gaming Services** | Opcional: **Analytics** só com consentimento do jogador (LGPD/GDPR); **Cloud Diagnostics** ou Sentry para crashes | Telemetria de funil (onde os jogadores abandonam cada capítulo) ajuda o design; crash reporting é obrigatório |
| **CI/CD** | **GitHub Actions + GameCI** (build Windows a cada push na `main`, testes EditMode/PlayMode) com cache da pasta `Library`; alternativa: **Unity Build Automation** | Build reproduzível; a licença Unity entra como *secret* do repositório |
| **Distribuição de builds** | Upload automático para uma **branch beta privada da Steam** via SteamPipe (`steamcmd`) a partir da CI | Testers sempre jogam a build mais recente |
| **Stores** | **1º Steam (PC + Steam Deck)**; demo no **Steam Next Fest** e/ou itch.io; consoles e mobile só depois do lançamento no PC | Consoles exigem cadastro de desenvolvedor, dev kits e certificação, o que é caro para um primeiro lançamento |
| **Classificação etária** | Planejar cedo: **ClassInd** (Brasil), **IARC** (consoles/mobile) e os *content descriptors* da Steam (drogas e temas sensíveis) | O tema do jogo afeta a classificação, a visibilidade na loja e as regras de marketing |
| **Responsabilidade temática** | Incluir uma tela de aviso de conteúdo e links para apoio (ex.: CVV 188 e CAPS-AD no Brasil) | Boa prática para um jogo sobre dependência, e bem vista por lojas e imprensa |

## Pendências para validação

- [ ] Gênero e câmera (2D, 3D ou point-and-click)
- [ ] Plataformas-alvo no lançamento
- [ ] Ferramenta de narrativa: Ink ou Yarn Spinner
- [ ] Haverá telemetria (Analytics)? Se sim, fluxo de consentimento
- [ ] CI: GameCI (GitHub Actions) ou Unity Build Automation
