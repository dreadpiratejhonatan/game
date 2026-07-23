# Contributing / Como adicionar features

Este projeto vai crescer com **muitas** features. Para não virar um emaranhado, siga este contrato.

## Princípios

1. **GameEngine não engorda** — só orquestra o loop.
2. **1 feature = Component(s) + System** registrados em `SystemRegistrar`.
3. **Conteúdo jogável vem de mods (JSON)** — stats, facções, cenas, variantes.
4. **Código C# = regras genéricas** — combate, câmera, inventário como sistemas.
5. **main sempre jogável** — branch por feature, PR, CI verde.

## Fluxo Git (obrigatório)

Detalhes: [docs/BRANCHING.md](docs/BRANCHING.md)

```text
main  ← sempre jogável + CI
  └── feature/<nome>  →  pull request  →  merge
```

```bash
git checkout -b feature/minha-feature
dotnet run -- --smoke
git push -u origin HEAD
gh pr create --fill
```

## Checklist obrigatório por feature

- [ ] Branch `feature/...` (não desenvolver direto na main)
- [ ] Componente novo em `src/Game/Components/` (se precisar de estado)
- [ ] Sistema novo em `src/Game/Systems/`
- [ ] Registro em [`src/Game/Bootstrap/SystemRegistrar.cs`](src/Game/Bootstrap/SystemRegistrar.cs)
- [ ] Se for data-driven: model em `src/Mods/Models/` + exemplo em `data/`
- [ ] Cenas novas: `data/**/scenes/*.json`
- [ ] Input novo: `src/Game/Input/` (não no GameEngine)
- [ ] Spawn/criação: `UnitFactory`
- [ ] Atualizar [`docs/MODDING.md`](docs/MODDING.md) / [`docs/MOD_CONTRACT.md`](docs/MOD_CONTRACT.md) se afetar modders
- [ ] `dotnet build` sem erros
- [ ] `dotnet run -- --smoke` passa
- [ ] PR para `main`

## Onde NÃO colocar lógica

| Evite | Prefira |
|-------|---------|
| `GameEngine.cs` com regras de jogo | System / Input / Factory |
| Stats hardcoded em C# | `data/**/*.json` |
| Models JSON misturados com Components | `Mods/Models` vs `Game/Components` |
| Spawn aleatório como “nível” | `scenes/baseline.json` |

## Cena baseline

O jogo carrega `data/base_mod/scenes/baseline.json` no boot.  
Use essa cena para testes reproduzíveis — não dependa de RNG de spawn.

## Debug vs jogo

- **F1** liga/desliga debug
- Spawn com **Espaço** só em debug
- Overlay de telemetria só em debug

## Smoke check / CI

```bash
dotnet run -- --smoke
```

CI no GitHub Actions roda build Release + smoke em todo push/PR para `main`.
