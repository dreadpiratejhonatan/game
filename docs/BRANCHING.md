# Fluxo de branches

Repositório: https://github.com/dreadpiratejhonatan/game

## Regras

1. **`main` sempre jogável** — build + smoke verdes.
2. **Uma feature = uma branch** — nome `feature/<slug>`.
3. **PR para `main`** — revisão rápida + CI.
4. **Commits pequenos** — preferir vários commits claros a um gigante.

## Como trabalhar

```bash
git checkout main
git pull
git checkout -b feature/loot

# ... implementação seguindo CONTRIBUTING.md ...

dotnet build
dotnet run -- --smoke

git add -A
git commit -m "Add loot drops for defeated enemies."
git push -u origin HEAD
gh pr create --fill
```

## Nomes sugeridos

| Branch | Escopo |
|--------|--------|
| `feature/loot` | Drops e pickup |
| `feature/inventory` | Inventário do player |
| `feature/squad` | Comandos de aliados |
| `feature/ui-hud` | HUD / menus |
| `feature/save` | Save/load |
| `fix/<descricao>` | Correções |

## O que não fazer

- Commitar direto em `main` features grandes (hotfix pequeno ok)
- Misturar 3 features numa mesma branch
- Deixar `main` quebrada overnight
