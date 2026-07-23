# Documentação de Arquitetura

## Visão Geral do Sistema ECS

### O que é ECS?

Entity-Component-System é um padrão arquitetural onde:
- **Entities** são containers de IDs
- **Components** são estruturas de dados puros
- **Systems** processam entidades com componentes específicos

### Fluxo de Dados

```
JSON (data/base_mod/units.json)
    ↓
ModManager (deserializa)
    ↓
UnitDefinition (objetos C#)
    ↓
World.CreateEntity() + AddComponent()
    ↓
Entity com Components
    ↓
Systems processam (Update/Draw)
```

## Componentes Implementados

### TransformComponent
Armazena posição, rotação e escala da entidade no mundo 2D.

**Propriedades:**
- `Vector2 Position`
- `float Rotation`
- `Vector2 Scale`

### RenderComponent
Define como a entidade deve ser renderizada (placeholder geométrico).

**Propriedades:**
- `Vector2 Size`: largura e altura
- `Color Color`: cor RGBA
- `RenderShape Shape`: Rectangle ou Circle

### UnitStatsComponent
Atributos de gameplay carregados do ModManager (data-driven).

**Propriedades:**
- `string UnitType`: ID da unidade
- `float MaxHealth`
- `float CurrentHealth`
- `float MoveSpeed`
- `float AttackDamage`
- `float AttackRange`

## Sistemas Implementados

### RenderSystem
Renderiza todas as entidades que possuem TransformComponent + RenderComponent.

**Responsabilidades:**
- Desenhar retângulos ou círculos na tela
- Suporte futuro para sprite sheets

## ModManager - Sistema Data-Driven

O `ModManager` é responsável por:
1. Ler arquivos JSON da pasta `data/`
2. Deserializar em objetos C# (`UnitDefinition`, `ItemDefinition`, etc.)
3. Fornecer acesso às definições via métodos `Get*()`

**Vantagens:**
- Zero hardcode de stats
- Hot-reloading possível no futuro
- Mods da comunidade sem recompilação

## Extensões Futuras

### Sistemas Planejados
- `MovementSystem`: pathfinding e navegação
- `CombatSystem`: detecção de colisão e dano
- `AISystem`: comportamento de NPCs
- `InputSystem`: controle de câmera e seleção

### Componentes Planejados
- `VelocityComponent`: movimento físico
- `HealthComponent`: separado de UnitStats para reuso
- `InventoryComponent`: itens equipados
- `SkillComponent`: habilidades ativas

### Tipos de Dados Planejados
- `items.json`: armas, armaduras, consumíveis
- `skills.json`: magias, habilidades, buffs
- `factions.json`: facções e relações diplomáticas
- `biomes.json`: tipos de terreno e clima

## Performance Considerations

### Cache Locality
O design atual usa listas de entidades. Para jogos com milhares de unidades, considere:
- Armazenar componentes em arrays contíguos (archetypos)
- Usar bibliotecas ECS otimizadas (Arch, DefaultEcs)

### Multithreading
Sistemas independentes podem rodar em paralelo:
```csharp
Parallel.ForEach(systems, system => system.Update(entities, deltaTime));
```

## Convenções de Código

- **Namespaces**: seguem a estrutura de pastas
- **Componentes**: sufixo `Component`
- **Sistemas**: sufixo `System`
- **Interfaces**: prefixo `I`
- **Campos privados**: prefixo `_`

## Diagramas

### Diagrama de Classes (Simplificado)

```
┌─────────────┐
│   World     │
├─────────────┤
│ + Entities  │
│ + Systems   │
└──────┬──────┘
       │
       ├─────> Entity (possui Components)
       │
       └─────> ISystem (processa Entities)
```

### Ciclo de Vida de uma Entidade

```
1. World.CreateEntity()
   ↓
2. entity.AddComponent(...)
   ↓
3. System.Update(entities) processa
   ↓
4. RenderSystem.Draw(entities) renderiza
   ↓
5. World.DestroyEntity(entity) [opcional]
```

---

**Documentação gerada para**: ModularGameEngine v0.1.0
