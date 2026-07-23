# Diagrama de Arquitetura - Modular Game Engine

## 📐 Visão Geral do Sistema

```
┌─────────────────────────────────────────────────────────────┐
│                     MODULAR GAME ENGINE                     │
└─────────────────────────────────────────────────────────────┘
                              │
         ┌────────────────────┼────────────────────┐
         ▼                    ▼                    ▼
    ┌─────────┐          ┌─────────┐        ┌──────────┐
    │ ENGINE  │          │  GAME   │        │   MODS   │
    │ (Core)  │          │(Logic)  │        │ (Data)   │
    └─────────┘          └─────────┘        └──────────┘
         │                    │                    │
         │                    │                    │
    ┌────┴────┐          ┌────┴────┐         ┌────┴────┐
    │   ECS   │          │Component│         │ModManager│
    │Graphics │          │Systems  │         │  JSON    │
    │  Core   │          │         │         │ Loader   │
    └─────────┘          └─────────┘         └──────────┘
```

## 🔄 Fluxo de Execução (Game Loop)

```
   ┌─────────────────────────────────────┐
   │       Program.cs (Main)             │
   │   new GameEngine().Run()            │
   └───────────────┬─────────────────────┘
                   │
                   ▼
   ┌─────────────────────────────────────┐
   │      GameEngine.Initialize()        │
   │   - Cria World                      │
   │   - Inicializa ModManager           │
   └───────────────┬─────────────────────┘
                   │
                   ▼
   ┌─────────────────────────────────────┐
   │     ModManager.LoadMods()           │
   │   - Lê data/base_mod/units.json    │
   │   - Deserializa em UnitDefinition   │
   └───────────────┬─────────────────────┘
                   │
                   ▼
   ┌─────────────────────────────────────┐
   │      GameEngine.LoadContent()       │
   │   - Cria Entidades (SpawnUnits)    │
   │   - Inicializa RenderSystem         │
   └───────────────┬─────────────────────┘
                   │
                   ▼
        ┌──────────────────────┐
        │   GAME LOOP ATIVO    │
        └──────────┬───────────┘
                   │
      ┌────────────┴────────────┐
      ▼                         ▼
┌───────────┐           ┌──────────────┐
│  Update   │           │     Draw     │
│ (Logic)   │           │  (Render)    │
└─────┬─────┘           └──────┬───────┘
      │                        │
      ▼                        ▼
┌───────────┐           ┌──────────────┐
│World.     │           │RenderSystem. │
│Update()   │           │Draw()        │
└─────┬─────┘           └──────────────┘
      │
      ▼
┌───────────────────────┐
│ Systems.Update()      │
│ - MovementSystem      │
│ - CombatSystem        │
│ - AISystem            │
└───────────────────────┘
```

## 🧩 Arquitetura ECS Detalhada

```
┌────────────────────────────────────────────────────────────┐
│                          WORLD                             │
│  ┌──────────────────────┐    ┌──────────────────────┐    │
│  │  List<Entity>        │    │  List<ISystem>       │    │
│  │  - entity1           │    │  - RenderSystem      │    │
│  │  - entity2           │    │  - MovementSystem    │    │
│  │  - entity3           │    │  - CombatSystem      │    │
│  │  - ...               │    │  - ...               │    │
│  └──────────────────────┘    └──────────────────────┘    │
└────────────────────────────────────────────────────────────┘
                   │
                   │ possui
                   ▼
        ┌────────────────────┐
        │      ENTITY        │
        │  ID: 42            │
        │  IsActive: true    │
        └──────┬─────────────┘
               │
               │ possui
               ▼
    ┌──────────────────────────────┐
    │    Dictionary<Type, IComponent>│
    └──────┬───────────────────────┘
           │
           ├─► TransformComponent { Position, Rotation, Scale }
           │
           ├─► RenderComponent { Size, Color, Shape }
           │
           ├─► UnitStatsComponent { HP, Speed, Damage }
           │
           └─► VelocityComponent { Velocity }


    ┌────────────────────────────────┐
    │         SYSTEM                 │
    │  Update(entities, deltaTime)   │
    └────────────┬───────────────────┘
                 │
                 │ processa
                 ▼
    ┌────────────────────────────────┐
    │  foreach entity in entities    │
    │    if has RequiredComponents   │
    │      do logic                  │
    └────────────────────────────────┘
```

## 📊 Fluxo de Dados: JSON → Entidade

```
data/base_mod/units.json
         │
         │ File.ReadAllText()
         ▼
    JSON string
         │
         │ JsonSerializer.Deserialize()
         ▼
  UnitDefinition object
    {
      id: "soldier_basic",
      stats: { max_health: 100, ... },
      visual: { width: 32, ... }
    }
         │
         │ ModManager.GetUnitDefinition()
         ▼
  GameEngine.SpawnUnitsFromMods()
         │
         │ World.CreateEntity()
         ▼
     Entity (ID: 1)
         │
         │ entity.AddComponent(...)
         ▼
    ┌─────────────────────────────┐
    │ TransformComponent          │
    │   Position = (100, 100)     │
    ├─────────────────────────────┤
    │ UnitStatsComponent          │
    │   MaxHealth = 100           │
    │   MoveSpeed = 120           │
    ├─────────────────────────────┤
    │ RenderComponent             │
    │   Size = (32, 32)           │
    │   Color = #4A90E2           │
    └─────────────────────────────┘
```

## 🎮 Ciclo de Vida de uma Entidade

```
1. CRIAÇÃO
   World.CreateEntity()
        ↓
   new Entity()
        ↓
   entity.AddComponent(...)
        ↓
   [Entity pronta na lista]

2. PROCESSAMENTO (a cada frame)
   World.Update(deltaTime)
        ↓
   System.Update(entities)
        ↓
   foreach entity in entities
        ↓
   entity.GetComponent<T>()
        ↓
   [Modifica componentes]

3. RENDERIZAÇÃO
   RenderSystem.Draw(entities)
        ↓
   entity.GetComponent<Transform>()
   entity.GetComponent<Render>()
        ↓
   [Desenha na tela]

4. DESTRUIÇÃO (quando necessário)
   World.DestroyEntity(entity)
        ↓
   entity.IsActive = false
        ↓
   [Removido da lista]
```

## 🔗 Relacionamentos entre Classes

```
Program.cs
    │
    └─► GameEngine (herda de Microsoft.Xna.Framework.Game)
            │
            ├─► World
            │     │
            │     ├─► List<Entity>
            │     │       │
            │     │       └─► Dictionary<Type, IComponent>
            │     │
            │     └─► List<ISystem>
            │
            ├─► ModManager
            │     │
            │     └─► List<UnitDefinition>
            │
            └─► RenderSystem
                  │
                  └─► SpriteBatch (MonoGame)
```

## 📁 Mapeamento: Pastas → Responsabilidades

```
src/Engine/
  ├─ ECS/              → Núcleo do padrão ECS
  │   ├─ IComponent.cs → Interface de componentes
  │   ├─ Entity.cs     → Container de componentes
  │   ├─ ISystem.cs    → Interface de sistemas
  │   └─ World.cs      → Gerenciador de entidades/sistemas
  │
  ├─ Graphics/         → Renderização avançada (futuro)
  │
  └─ Core/
      └─ GameEngine.cs → Game loop principal

src/Game/
  ├─ Components/       → Dados específicos do jogo
  │   ├─ TransformComponent.cs
  │   ├─ RenderComponent.cs
  │   └─ UnitStatsComponent.cs
  │
  └─ Systems/          → Lógica específica do jogo
      └─ RenderSystem.cs

src/Mods/
  └─ ModManager.cs     → Carregador de dados JSON

data/
  └─ base_mod/
      └─ units.json    → Definições de unidades
```

## 🚀 Performance: Complexidade

```
Operação                          Complexidade    Otimização Futura
──────────────────────────────────────────────────────────────────
CreateEntity()                    O(1)            -
AddComponent(comp)                O(1)            -
GetComponent<T>()                 O(1)            -
World.Update() [N entities]       O(N × S)        Spatial hashing
RenderSystem.Draw() [N entities]  O(N)            Frustum culling
HasComponents(types)              O(T)            Archetypos
GetEntitiesWithComponents(types)  O(N × T)        Cache queries

N = número de entidades
S = número de sistemas
T = número de tipos de componentes
```

## 🔮 Evolução Futura

```
AGORA (v0.1)                    FUTURO (v1.0)
──────────────────────────────────────────────────────

Simple ECS           ───────►   Archetype ECS
(Lista de entidades)            (Arrays por tipo)

JSON Loader          ───────►   Hot Reloading
(Carrega no início)             (Recarrega em runtime)

Placeholder Render   ───────►   Sprite Sheets
(Retângulos)                    (Pixel art animado)

Single Thread        ───────►   Parallel Systems
(Update sequencial)             (Systems em threads)

Local Only           ───────►   Multiplayer
(Single player)                 (Co-op online)
```

---

**Legenda:**
- `┌─┐` = Container/Módulo
- `│` = Relação vertical
- `─` = Relação horizontal
- `►` = Fluxo de dados
- `...` = Continuação
