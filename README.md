# Addicted

Cada vício, uma história. Toda história termina na reabilitação.

Roguelike de ação 2D feito na Unity. Protótipo com três personagens: Maconha, Cocaína e Álcool.

## Requisitos

| Ferramenta | Versão |
|---|---|
| Unity | **6.3 LTS** — a versão exata está em `ProjectSettings/ProjectVersion.txt`; instale a mesma pelo Unity Hub |
| Módulo Unity | Windows Build Support (IL2CPP) |
| IDE | Visual Studio com o workload "Desenvolvimento de jogos com Unity" |
| Git | Git + Git LFS |

## Como abrir o projeto

```bash
git lfs install          # uma vez por máquina
git clone https://github.com/Jogos-Absurdos/Addicted.git
```

Depois: Unity Hub → **Add** → selecione a pasta clonada → abra. A primeira abertura demora, porque a Unity recria a pasta `Library/`.

Cena principal do protótipo: `Assets/_Project/Scenes/Escritorio.unity`.

## Estrutura

```
Assets/_Project/
├── Core/                 # Bootstrap e serviços comuns (Addicted.Core)
├── Gameplay/             # Addicted.Gameplay
│   ├── Movement/         # Movimento (briga de rua e plataforma)
│   ├── Addiction/        # Ciclo Rotina → Pico → Crash → Limite cardíaco
│   ├── Player/ Combat/ Enemies/ Room/ UI/
├── Content/              # ScriptableObjects: vícios, habilidades, itens (Addicted.Content)
├── Art/Placeholder/      # Sprites provisórios para protótipo
├── Scenes/               # Sandbox.unity e cenas do jogo
├── Settings/             # URP (2D Renderer), Volume Profile, Input Actions
└── Tests/
    ├── EditMode/         # Testes de lógica, sem física (Addicted.Tests.EditMode)
    └── PlayMode/         # Testes com física 2D real (Addicted.Tests.PlayMode)
```

## Sala de protótipo: Escritório

Cena: `Assets/_Project/Scenes/Escritorio.unity`. É uma sala de combate em vista lateral com profundidade (estilo briga de rua), com arte provisória.

| Ação | Teclado / mouse | Controle |
|---|---|---|
| Andar (dois eixos) | WASD ou setas | Analógico esquerdo |
| Dash | Shift esquerdo | RT / R2 |
| Atacar | Clique esquerdo ou Enter | X / Quadrado |

- **Zona de dose** (cristal no chão): pisar nela leva ao **Pico** (mais rápido). O Pico acaba em **Crash** (mais lento). Três doses seguidas estouram o pulso e levam ao **Limite cardíaco**. O sprite do personagem muda em cada estado.
- **Inimigos**: o perseguidor (triângulo) corre até você; o atirador (losango) mantém distância e atira.
- **Ondas**: a sala tem duas. Ao limpar a segunda, a saída muda de magenta para verde.

As regras e os números do ciclo do vício são provisórios e ficam em `HighSettings`. O HUD é de depuração (`DebugHud`). O jogador ainda não perde vida.

## Movimento de plataforma (cena Sandbox)

A cena `Sandbox.unity` mantém um controlador alternativo, de plataforma com pulo. O personagem é um side-scroller com corrida, pulo e dash. A lógica fica em `PlatformerMotor` (C# puro, testável) e o componente `PlatformerMovement2D` a liga ao `Rigidbody2D` cinemático e ao Input System.

| Ação | Teclado | Controle |
|---|---|---|
| Andar | A/D ou ←/→ | Analógico esquerdo / D-pad |
| Pular (segure para pular mais alto) | Espaço | A / Cruz |
| Dash | Shift esquerdo | RT / R2 |

Os parâmetros (velocidade, altura e duração do pulo, coyote time, buffer de pulo, pulos extras, distância e recarga do dash) ficam em `MovementSettings`, dentro de cada `AddictionDefinition`. Assim, cada vício pode ter um movimento diferente sem mudar código.

Cada pasta de código tem uma Assembly Definition. As dependências seguem uma direção só: `Gameplay → Content → Core`. Nada depende de `Gameplay`.

Para rodar os testes: **Window → General → Test Runner → EditMode → Run All**.

Tudo que é do jogo fica em `Assets/_Project/`. Pacotes e assets de terceiros ficam fora dessa pasta.

## Regras de trabalho

1. **Nunca commitar direto na `main`.** Crie uma branch (`feature/movimentacao`, `fix/dash`) e abra um pull request.
2. **Uma pessoa por cena.** Cenas dão conflito difícil de resolver. Trabalhe em prefabs e use uma cena de teste própria (`Scenes/Dev/SeuNome.unity`).
3. **Sempre commitar o `.meta` junto com o arquivo.** Se criar, mover ou apagar um asset, o `.meta` dele vai no mesmo commit.
4. **Mover e renomear assets só dentro do editor da Unity**, nunca pelo Explorer.
5. **Não atualizar a versão da Unity nem pacotes por conta própria.** Combine com a equipe antes.
6. Faça `git pull` antes de começar a trabalhar.

## Documentação

- [Arquitetura](docs/arquitetura.md)
