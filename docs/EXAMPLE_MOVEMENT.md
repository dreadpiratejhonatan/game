# Exemplo: Sistema de Movimento Simples

Este é um exemplo prático de como adicionar um novo sistema ao jogo.

## Objetivo
Fazer as unidades se moverem em uma direção aleatória, demonstrando o sistema ECS em ação.

## Passo 1: Criar o VelocityComponent

Arquivo: `src/Game/Components/VelocityComponent.cs`

```csharp
using Microsoft.Xna.Framework;
using ModularGameEngine.Engine.ECS;

namespace ModularGameEngine.Game.Components;

/// <summary>
/// Componente que armazena a velocidade de movimento de uma entidade.
/// </summary>
public class VelocityComponent : IComponent
{
    public Vector2 Velocity { get; set; }
    
    public VelocityComponent(Vector2 velocity)
    {
        Velocity = velocity;
    }
    
    public VelocityComponent(float x, float y) : this(new Vector2(x, y))
    {
    }
}
```

## Passo 2: Criar o MovementSystem

Arquivo: `src/Game/Systems/MovementSystem.cs`

```csharp
using ModularGameEngine.Engine.ECS;
using ModularGameEngine.Game.Components;

namespace ModularGameEngine.Game.Systems;

/// <summary>
/// Sistema que move entidades baseado em seu VelocityComponent.
/// </summary>
public class MovementSystem : ISystem
{
    public void Update(IEnumerable<Entity> entities, float deltaTime)
    {
        foreach (var entity in entities)
        {
            var transform = entity.GetComponent<TransformComponent>();
            var velocity = entity.GetComponent<VelocityComponent>();
            
            // Só processa entidades que têm ambos os componentes
            if (transform == null || velocity == null) continue;
            
            // Aplica o movimento
            transform.Position += velocity.Velocity * deltaTime;
            
            // Wrap around nas bordas da tela (1280x720)
            if (transform.Position.X < 0) transform.Position.X = 1280;
            if (transform.Position.X > 1280) transform.Position.X = 0;
            if (transform.Position.Y < 0) transform.Position.Y = 720;
            if (transform.Position.Y > 720) transform.Position.Y = 0;
        }
    }
}
```

## Passo 3: Modificar o GameEngine.cs

### 3.1 - Adicionar o campo do sistema

```csharp
private MovementSystem _movementSystem = null!;
```

### 3.2 - Inicializar o sistema em LoadContent()

```csharp
protected override void LoadContent()
{
    _spriteBatch = new SpriteBatch(GraphicsDevice);
    
    _renderSystem = new RenderSystem(GraphicsDevice, _spriteBatch);
    _movementSystem = new MovementSystem(); // ← ADICIONE AQUI
    
    // Registrar no World
    _world.AddSystem(_movementSystem); // ← ADICIONE AQUI
    
    SpawnUnitsFromMods();
    
    Console.WriteLine($"Conteúdo carregado. {_world.GetEntities().Count()} entidades criadas.");
}
```

### 3.3 - Adicionar velocidade às entidades em SpawnUnitsFromMods()

```csharp
private void SpawnUnitsFromMods()
{
    var random = new Random();
    var unitDefs = _modManager.UnitDefinitions;
    
    if (unitDefs.Count == 0) return;
    
    foreach (var unitDef in unitDefs)
    {
        for (int i = 0; i < 3; i++)
        {
            var x = random.Next(100, 1180);
            var y = random.Next(100, 620);
            
            var entity = _world.CreateEntity();
            
            entity.AddComponent(new TransformComponent(x, y));
            
            entity.AddComponent(new UnitStatsComponent(
                unitDef.Id,
                unitDef.Stats.MaxHealth,
                unitDef.Stats.MoveSpeed,
                unitDef.Stats.AttackDamage,
                unitDef.Stats.AttackRange
            ));
            
            var color = ParseColor(unitDef.Visual.Color);
            var size = new Vector2(unitDef.Visual.Width, unitDef.Visual.Height);
            var shape = unitDef.Visual.Shape.ToLower() == "circle" 
                ? RenderShape.Circle 
                : RenderShape.Rectangle;
            
            entity.AddComponent(new RenderComponent(size, color, shape));
            
            // ↓↓↓ ADICIONE ESTA LINHA ↓↓↓
            // Velocidade aleatória baseada no MoveSpeed da unidade
            var stats = entity.GetComponent<UnitStatsComponent>()!;
            var velocityX = (float)(random.NextDouble() * 2 - 1) * stats.MoveSpeed;
            var velocityY = (float)(random.NextDouble() * 2 - 1) * stats.MoveSpeed;
            entity.AddComponent(new VelocityComponent(velocityX, velocityY));
            // ↑↑↑ ATÉ AQUI ↑↑↑
            
            Console.WriteLine($"  - Spawned: {unitDef.Name} at ({x}, {y})");
        }
    }
}
```

## Resultado

Agora, ao executar o jogo (`dotnet run`), você verá:
- **12 unidades se movendo** pela tela em direções aleatórias
- **Velocidades diferentes** baseadas no `move_speed` do JSON
- **Wrap-around**: unidades que saem de um lado reaparecem do outro

## Testando

1. Compile: `dotnet build`
2. Execute: `dotnet run`
3. Observe as formas coloridas se movendo
4. Experimente editar `data/base_mod/units.json` e mudar os `move_speed`
5. Reinicie o jogo para ver a diferença

## Próximas Extensões

- Adicionar colisão entre entidades
- Fazer unidades seguirem um alvo (AI básica)
- Adicionar aceleração/desaceleração suave
- Implementar pathfinding (A* ou NavMesh)

---

**Esse exemplo demonstra:**
- ✅ Como criar novos componentes
- ✅ Como criar novos sistemas
- ✅ Como integrar sistemas no World
- ✅ Como componentes trabalham juntos (Transform + Velocity)
- ✅ Como usar dados do ModManager (move_speed)
