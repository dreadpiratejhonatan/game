# Contributing / Como adicionar features

Este projeto vai crescer com **muitas** features. Para não virar um emaranhado, siga este contrato.

## Princípios

1. **GameEngine não engorda** — só orquestra o loop.
2. **1 feature = Component(s) + System** registrados em `SystemRegistrar`.
3. **Conteúdo jogável vem de mods (JSON)** — stats, facções, variantes.
4. **Código C# = regras genéricas** — combate, câmera, inventário como sistemas.
5. **main sempre jogável** — branch por feature, commits pequenos.

## Checklist obrigatório por feature

- [ ] Componente novo em `src/Game/Components/` (se precisar de estado)
- [ ] Sistema novo em `src/Game/Systems/`
- [ ] Registro em [`src/Game/Bootstrap/SystemRegistrar.cs`](src/Game/Bootstrap/SystemRegistrar.cs)
- [ ] Se for data-driven: model em `src/Mods/Models/` + exemplo em `data/`
- [ ] Input novo: `src/Game/Input/` (não no GameEngine)
- [ ] Spawn/criação de entidades: `UnitFactory`
- [ ] Atualizar [`docs/MODDING.md`](docs/MODDING.md) se afetar modders
- [ ] `dotnet run -- --smoke` passa
- [ ] `dotnet build` sem erros

## Onde NÃO colocar lógica

| Evite | Prefira |
|-------|---------|
| `GameEngine.cs` com regras de jogo | System / Input / Factory |
| Stats hardcoded em C# | `data/**/*.json` |
| Models JSON misturados com Components | `Mods/Models` vs `Game/Components` |

## Branches (GitHub: dreadpiratejhonatan/game)

```text
main                 ← sempre jogável
feature/camera
feature/loot
feature/squad
```

## Debug vs jogo

- **F1** liga/desliga debug
- Spawn com **Espaço** só em debug
- Overlay de telemetria só em debug

Não adicione cheats no caminho normal do jogador.

## Smoke check

```bash
dotnet run -- --smoke
```

Deve imprimir `SMOKE OK` e sair com código 0.
