# Estrutura Organizada do Projeto

Estrutura preparada para manutenção, open source e mods.

## Árvore principal

```
ModularGameEngine/
├── Program.cs
├── ModularGameEngine.sln
├── ModularGameEngine.csproj
├── LICENSE                          # MIT
├── README.md
│
├── src/
│   ├── Engine/                      # Núcleo reutilizável (sem regras de ARPG)
│   │   ├── Core/GameEngine.cs       # Orquestrador fino do loop
│   │   ├── ECS/                     # Entity, Component, System, World
│   │   ├── Graphics/                # SpriteGenerator
│   │   └── Util/                    # ColorUtil, helpers
│   │
│   ├── Game/                        # Gameplay específico
│   │   ├── Bootstrap/               # SystemRegistrar (ordem dos sistemas)
│   │   ├── Components/              # Dados ECS
│   │   ├── Systems/                 # Lógica ECS
│   │   ├── Spawning/UnitFactory.cs  # Criação de entidades a partir de JSON
│   │   └── Input/PlayerInputHandler.cs
│   │
│   └── Mods/
│       ├── ModManager.cs            # Loader (base + user_mods)
│       └── Models/                  # Contratos JSON públicos
│
├── data/
│   ├── base_mod/                    # Conteúdo oficial
│   │   ├── mod.json
│   │   └── units.json
│   └── user_mods/                   # Mods da comunidade
│       ├── README.md
│       └── example_hostile_blob/
│
└── docs/                            # Documentação
    ├── MODDING.md
    ├── ORGANIZED_STRUCTURE.md
    └── ...
```

## Onde colocar o quê

| Quero... | Coloque em |
|----------|------------|
| Novo comportamento (câmera, loot) | `Game/Systems/` + registrar em `SystemRegistrar` |
| Novo dado na entidade | `Game/Components/` + aplicar em `UnitFactory` se vier do JSON |
| Novo campo no JSON de unidade | `Mods/Models/UnitDefinition.cs` + `units.json` |
| Novo tipo de arquivo de mod | `ModManager` + model em `Mods/Models/` |
| Input novo (skill hotkey) | `Game/Input/PlayerInputHandler.cs` |
| Ajustar balance | só `data/**/*.json` |

## Regras de arquitetura (manutenção)

1. **GameEngine não cresce** — só inicializa e chama Update/Draw.
2. **1 feature = 1 System** (e componentes mínimos).
3. **Nada de stats hardcoded** — valores de gameplay vêm de mods.
4. **Models ≠ Components** — JSON models em `Mods/Models`; runtime em `Game/Components`.
5. **Mods nunca exigem recompile** para conteúdo; só para mecânicas novas no motor.

## Fluxo de dados

```
data/*/units.json
    → ModManager
    → UnitDefinition (Models)
    → UnitFactory
    → Entity + Components
    → Systems (Movement, Combat, ...)
    → RenderSystem
```

## Checklist ao adicionar feature

- [ ] Componente novo (se necessário)
- [ ] Sistema novo + entrada em `SystemRegistrar`
- [ ] Campos JSON? → Models + exemplo em `data/`
- [ ] Documentar em `docs/MODDING.md` se afetar modders
- [ ] `dotnet build` limpo
