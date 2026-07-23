# Contrato de Mods v1 (estável)

Versão do contrato: **1.0**  
Qualquer breaking change sobe a versão maior e deve ser anunciada no README.

## Pastas

```text
data/
  base_mod/          # oficial, carrega primeiro
    mod.json         # recomendado
    units.json
    scenes/          # cenas (baseline.json, etc.)
  user_mods/
    <mod_id>/
      mod.json       # obrigatório para mods de comunidade
      units.json     # opcional
      scenes/        # opcional
```

## Garantias (não quebrar sem major)

1. `base_mod` sempre carrega antes de `user_mods`.
2. `user_mods` em ordem alfabética.
3. Pastas `_nome` ou `.nome` são ignoradas.
4. Campo `id` em unidades é a chave de identidade/override.
5. `override_base: true` no `mod.json` permite substituir unidade de mesmo `id`.
6. Facções reconhecidas: `player`, `ally`, `enemy`, `neutral`.
7. Comentários JSON e trailing commas são aceitos.

## units.json — campos estáveis

| Campo | Tipo | Obrigatório |
|-------|------|-------------|
| `id` | string | sim |
| `name` | string | sim |
| `faction` | string | não (default `neutral`) |
| `stats.max_health` | number | sim |
| `stats.move_speed` | number | sim |
| `stats.attack_damage` | number | sim |
| `stats.attack_range` | number | sim |
| `stats.attack_cooldown` | number | não (default 0.8) |
| `stats.detection_range` | number | não (default 220) |
| `visual.width` / `height` | number | sim |
| `visual.color` | `#RRGGBB` | sim |
| `visual.shape` | `rectangle` \| `circle` | não |

## scenes/*.json — campos estáveis

| Campo | Tipo | Obrigatório |
|-------|------|-------------|
| `id` | string | sim |
| `name` | string | sim |
| `world_width` / `world_height` | number | não (default 3200×2400) |
| `player.unit_id` + `x`/`y` | — | sim para cena jogável |
| `spawns[]` | lista | não |
| `spawns[].unit_id` + `x`/`y` | — | sim por spawn |
| `spawns[].faction` | string | não (override opcional) |

Cena oficial de teste: `baseline`.

## Extensões futuras (não estáveis ainda)

Arquivos planejados: `items.json`, `skills.json`, `factions.json`.  
Enquanto não documentados aqui como estáveis, podem mudar.

## Teste rápido

```bash
dotnet run -- --smoke
```
