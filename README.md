# Modular Game Engine

Protótipo open source de ARPG modular (ECS + data-driven).  
Feito para ser **suave de jogar**, **fácil de manter** e **extremamente adaptável a mods**.

## Executar

```bash
dotnet run
```

Ou abra `ModularGameEngine.sln` no Visual Studio e pressione F5.

**Controles**

| Input | Ação |
|-------|------|
| Clique no chão | Mover personagem |
| Clique no inimigo | Perseguir e atacar |
| Espaço | Spawn aleatório (debug) |
| ESC | Sair |

## Filosofia

- **Open source** — licença MIT (`LICENSE`)
- **Mods first** — conteúdo em `data/`, não no código
- **ECS** — features novas = Components + Systems
- **GameEngine fino** — orquestra; não acumula regra de jogo

## Estrutura (resumo)

```
src/Engine/     → núcleo (ECS, loop, gráficos)
src/Game/       → gameplay (systems, input, spawn)
src/Mods/       → loader + contratos JSON
data/base_mod/  → conteúdo oficial
data/user_mods/ → mods da comunidade
docs/           → documentação
```

Detalhes: [docs/ORGANIZED_STRUCTURE.md](docs/ORGANIZED_STRUCTURE.md)  
Criar mods: [docs/MODDING.md](docs/MODDING.md)

## Mod rápido

1. Copie `data/user_mods/example_hostile_blob/`
2. Renomeie a pasta e edite `mod.json` + `units.json`
3. `dotnet run` — o console lista o mod carregado

## Stack

- C# 12 / .NET 8
- MonoGame 3.8
- System.Text.Json

## Licença

MIT — veja [LICENSE](LICENSE).
