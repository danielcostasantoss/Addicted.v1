# ADR-001 — Arquitetura Unity e Infraestrutura de Publicação

- **Status:** Proposto (aguardando validação da equipe)
- **Data:** 2026-11-15
- **Escopo:** Engine, estrutura do projeto, backend, CI/CD e publicação do *Addicted*

> Esta é uma **proposta**, não um registro de decisões tomadas em reunião. Se alguma premissa abaixo estiver errada, as recomendações devem ser revistas.

## Premissas

- **Roguelike de ação 2D, single-player, offline.** Cada vício é um personagem jogável; o protótipo tem três: Maconha, Cocaína e Álcool.
- Projeto Unity versionado em Git + Git LFS (já configurado em `.gitattributes`), com código proprietário.
- Lançamento inicial em **PC (Steam)**, com Steam Deck como alvo de performance.

## 1. Decisões de Arquitetura e Unity

| Tema | Recomendação | Motivo |
|---|---|---|
| **Versão da engine** | **Unity 6.3 LTS**, travada em `ProjectSettings/ProjectVersion.txt` por todo o desenvolvimento | LTS tem 2 anos de correções; trocar de versão no meio do projeto é risco desnecessário |
| **Render Pipeline** | **URP com 2D Renderer** (luzes 2D, Sprite Lit/Unlit) | Pipeline 2D nativo da Unity; leve o bastante para Steam Deck, Switch e mobile |
| **Física e movimento** | **Rigidbody2D cinemático** com controlador próprio (`Gameplay/Movement`), compartilhado por todos os personagens | Controle preciso de dash e esquiva, sem a "flutuação" da física dinâmica; cada vício só muda os parâmetros |
| **Dados de conteúdo** | **ScriptableObjects** em `Content/`: `AddictionDefinition` (personagem), `AbilityDefinition`, `ItemDefinition`, `EnemyDefinition` | Designers criam e balanceiam conteúdo sem mexer em código; um vício novo vira um asset novo |
| **Habilidades e efeitos** | Habilidades como dados + componentes reutilizáveis (dano, projétil, status, buff/debuff) em vez de uma classe por habilidade | Um roguelike vive de combinações entre itens; composição escala melhor que herança |
| **Geração procedural** | Salas pré-desenhadas (**Tilemap**) montadas por um gerador com **seed** (`System.Random` próprio, nunca `UnityEngine.Random` global) | A seed torna a run reproduzível, o que permite depurar bugs e compartilhar runs |
| **Pooling** | `UnityEngine.Pool.ObjectPool<T>` para projéteis, inimigos, VFX e números de dano | Evita picos de GC no combate, onde centenas de objetos nascem e morrem por segundo |
| **Controle de versão** | Git + **Git LFS**; *Asset Serialization = Force Text*; *Visible Meta Files*; **UnityYAMLMerge** como merge tool | Diff e merge legíveis de cenas e prefabs |
| **Gerenciamento de assets** | **Addressables**, com um grupo por bioma/andar e um por personagem | Carrega só o que a run precisa; facilita patches e DLCs de novos vícios |
| **Estrutura de código** | **Assembly Definitions** espelhando `Assets/_Project/`: `Core`, `Gameplay`, `Content`, `UI`, `Save`, mais `Tests` | Compila mais rápido e impede dependências circulares (ex.: `Core` não pode depender de `Gameplay`) |
| **Pacotes base** | Input System (teclado + controle), Cinemachine (câmera 2D), 2D Tilemap Extras, **Localization** (pt-BR + en desde o início) | Controle é obrigatório para Steam Deck; adicionar localização depois custa caro |
| **Save** | Dois arquivos JSON versionados (`saveVersion`): **meta-progressão** (desbloqueios, moeda permanente) e **run em andamento** (seed + estado atual) | Separar os dois evita que um bug na run corrompa o progresso permanente; ambos sincronizam pelo Steam Cloud |
| **Otimização** | Profiler desde o protótipo; SRP Batcher; **Sprite Atlas** por bioma; orçamento definido (60 fps no Steam Deck com N inimigos na tela) | Roguelikes de ação degradam no fim da run, quando há mais inimigos e efeitos; o teste de estresse deve simular esse cenário |

## 2. Infraestrutura e Publicação

| Tema | Recomendação | Motivo |
|---|---|---|
| **Multiplayer / servidores** | **Nenhum servidor dedicado.** Jogo 100% offline | Single-player não precisa de backend próprio, o que elimina custo fixo e manutenção |
| **Backend** | **Steamworks** (conquistas, Steam Cloud, estatísticas e, opcionalmente, *leaderboards* de run diária com seed fixa) via *Steamworks.NET*, atrás de uma interface `IPlatformServices` | A run diária é barata de fazer graças à seed e não exige servidor próprio; a interface permite plugar outras plataformas depois |
| **Unity Gaming Services** | Opcional: **Analytics** só com consentimento (LGPD/GDPR); **Cloud Diagnostics** ou Sentry para crashes | Telemetria de balanceamento (onde e de quê os jogadores morrem, quais itens escolhem) vale ouro em roguelike |
| **CI/CD** | **GitHub Actions + GameCI** (build Windows a cada push, testes EditMode/PlayMode) com cache da pasta `Library`; alternativa: **Unity Build Automation** | Build reproduzível; a licença Unity entra como *secret* do repositório |
| **Distribuição de builds** | Upload automático para uma **branch beta privada da Steam** via SteamPipe (`steamcmd`) | Testers sempre jogam a build mais recente |
| **Stores** | **1º Steam (PC + Steam Deck)**, com Early Access como opção; demo no **Steam Next Fest**; consoles depois | Early Access é comum no gênero e financia balanceamento com feedback real |
| **Classificação etária** | Planejar cedo: **ClassInd** (Brasil), **IARC** (consoles/mobile) e os *content descriptors* da Steam (drogas, violência) | Personagens que são drogas pesam na classificação, na visibilidade na loja e nas regras de anúncio |
| **Responsabilidade temática** | Tela de aviso de conteúdo e links para apoio (ex.: CVV 188 e CAPS-AD no Brasil) | Coerente com a mensagem do jogo ("toda história termina na reabilitação") e bem visto por lojas e imprensa |

## Pendências para validação

- [ ] Versão exata da Unity 6.3 LTS (o README pede 6.3 LTS)
- [ ] Plataformas-alvo no lançamento e se haverá Early Access
- [ ] Haverá telemetria (Analytics)? Se sim, fluxo de consentimento
- [ ] Run diária com leaderboard entra no escopo?
- [ ] CI: GameCI (GitHub Actions) ou Unity Build Automation
