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

Cena de testes: `Assets/_Project/Scenes/Sandbox.unity`.

## Estrutura

```
Assets/_Project/
├── Core/                 # Bootstrap e serviços comuns
├── Gameplay/
│   └── Movement/         # Controlador 2D comum a todos os personagens
├── Content/              # ScriptableObjects (vícios, habilidades, itens)
└── Scenes/
```

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
