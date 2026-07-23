# Guia de Mods

O jogo é **data-driven** e pensado para a comunidade. Quase todo conteúdo jogável deve viver em JSON sob `data/`, não no C#.

## Filosofia

| Faça no JSON (mod) | Faça no C# (código) |
|--------------------|---------------------|
| Stats, cores, facções, novos tipos de unidade | Sistemas novos (combate, câmera, inventário) |
| Balanceamento e variantes | Regras genéricas reutilizáveis |
| Overrides de conteúdo | Motor ECS / render / input |

Objetivo: um modder consegue criar inimigos e ajustar o jogo **sem recompilar**.

## Estrutura de pastas

```
data/
├── base_mod/                 # Conteúdo oficial (sempre carrega primeiro)
│   ├── mod.json
│   └── units.json
└── user_mods/                # Mods da comunidade
    ├── README.md
    └── meu_mod/
        ├── mod.json          # obrigatório
        └── units.json        # opcional
```

## mod.json (manifesto)

```json
{
  "id": "meu_mod",
  "name": "Meu Mod Incrível",
  "version": "1.0.0",
  "author": "Seu Nome",
  "description": "O que o mod faz",
  "override_base": true
}
```

- `override_base: true` → unidades com o mesmo `id` do base_mod são substituídas
- `override_base: false` → ids duplicados são ignorados (aviso no console)

## units.json

Campos principais:

| Campo | Significado |
|-------|-------------|
| `id` | Identificador único (chave de override) |
| `faction` | `ally` \| `enemy` \| `neutral` |
| `stats.*` | HP, velocidade, dano, alcance, cooldown, detecção |
| `visual.color` | Hex `#RRGGBB` |
| `visual.shape` | `rectangle` (humanoide/veículo) ou `circle` (blob) |

Exemplo mínimo: veja `data/user_mods/example_hostile_blob/`.

## Ordem de carregamento

1. `base_mod`
2. Pastas em `user_mods/` (ordem alfabética)
3. Pastas `_nome` ou `.nome` são ignoradas (útil para desativar um mod)

## Como testar um mod

1. Crie a pasta em `data/user_mods/seu_mod/`
2. Adicione `mod.json` + `units.json`
3. `dotnet run`
4. No console, confirme: `[ModManager] Carregando: ...`
5. Use **Espaço** para spawnar unidades aleatórias (inclui as do mod)

## Extensão futura (contrato)

Quando novos arquivos forem suportados, seguirão o mesmo padrão:

```
meu_mod/
├── mod.json
├── units.json
├── items.json      # planejado
├── skills.json     # planejado
└── factions.json   # planejado
```

Modelos C# ficam em `src/Mods/Models/`. Loader em `ModManager`.

## Boas práticas

- Prefira **ids novos** a sobrescrever o base, salvo quando for balance patch
- Documente o mod no `description` do `mod.json`
- Não coloque binários ou segredos na pasta do mod
- Mantenha o jogo suave: evite stats absurdos que quebrem o ritmo (dano instantâneo, velocidade extrema)
