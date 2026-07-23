# Quick Reference - Modular Game Engine

## 🎯 Comandos Rápidos

```bash
# Compilar
dotnet build

# Executar
dotnet run

# Executar em Release
dotnet run -c Release

# Limpar build
dotnet clean

# Ver estrutura
tree /F /A
```

## 📦 Adicionar Novo Componente

### 1. Criar o arquivo (ex: `src/Game/Components/VelocityComponent.cs`)

```csharp
using Microsoft.Xna.Framework;
using ModularGameEngine.Engine.ECS;

namespace ModularGameEngine.Game.Components;

public class VelocityComponent : IComponent
{
    public Vector2 Velocity { get; set; }
    public float MaxSpeed { get; set; } = 200f;
    
    public VelocityComponent(Vector2 velocity)
    {
        Velocity = velocity;
    }
}
```

### 2. Adicionar à entidade

```csharp
var entity = _world.CreateEntity();
entity.AddComponent(new TransformComponent(100, 100));
entity.AddComponent(new VelocityComponent(new Vector2(50, 0)));
```

## 🔧 Adicionar Novo Sistema

### 1. Criar o arquivo (ex: `src/Game/Systems/MovementSystem.cs`)

```csharp
using ModularGameEngine.Engine.ECS;
using ModularGameEngine.Game.Components;

namespace ModularGameEngine.Game.Systems;

public class MovementSystem : ISystem
{
    public void Update(IEnumerable<Entity> entities, float deltaTime)
    {
        foreach (var entity in entities)
        {
            var transform = entity.GetComponent<TransformComponent>();
            var velocity = entity.GetComponent<VelocityComponent>();
            
            if (transform == null || velocity == null) continue;
            
            // Aplicar movimento
            transform.Position += velocity.Velocity * deltaTime;
        }
    }
}
```

### 2. Registrar no GameEngine.cs

```csharp
protected override void Initialize()
{
    _world = new World();
    _world.AddSystem(new MovementSystem()); // ← Adicione aqui
    
    base.Initialize();
}
```

## 📊 Adicionar Novo Tipo de Dado (JSON)

### 1. Criar o arquivo JSON (ex: `data/base_mod/items.json`)

```json
{
  "items": [
    {
      "id": "sword_iron",
      "name": "Espada de Ferro",
      "type": "weapon",
      "stats": {
        "damage": 25,
        "attack_speed": 1.2
      }
    }
  ]
}
```

### 2. Adicionar classes de definição no `ModManager.cs`

```csharp
public class ItemsWrapper
{
    [JsonPropertyName("items")]
    public List<ItemDefinition> Items { get; set; } = new();
}

public class ItemDefinition
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;
    
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
    
    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;
    
    [JsonPropertyName("stats")]
    public ItemStats Stats { get; set; } = new();
}

public class ItemStats
{
    [JsonPropertyName("damage")]
    public float Damage { get; set; }
    
    [JsonPropertyName("attack_speed")]
    public float AttackSpeed { get; set; }
}
```

### 3. Carregar no `ModManager`

```csharp
private List<ItemDefinition> _itemDefinitions = new();
public IReadOnlyList<ItemDefinition> ItemDefinitions => _itemDefinitions.AsReadOnly();

private void LoadItems(string modPath)
{
    var itemsFile = Path.Combine(modPath, "items.json");
    if (!File.Exists(itemsFile)) return;
    
    var json = File.ReadAllText(itemsFile);
    var wrapper = JsonSerializer.Deserialize<ItemsWrapper>(json);
    
    if (wrapper?.Items != null)
    {
        _itemDefinitions.AddRange(wrapper.Items);
        Console.WriteLine($"[ModManager] {wrapper.Items.Count} items carregados");
    }
}

// Chamar em LoadBaseMod()
private void LoadBaseMod()
{
    var basePath = Path.Combine(_dataPath, "base_mod");
    LoadUnits(basePath);
    LoadItems(basePath); // ← Adicione aqui
}
```

## 🎨 Cores Comuns (Hexadecimal)

```
Vermelho:  #E74C3C
Azul:      #4A90E2
Verde:     #50C878
Amarelo:   #F39C12
Roxo:      #9B59B6
Laranja:   #FF6347
Cinza:     #95A5A6
Preto:     #2C3E50
Branco:    #ECF0F1
```

## 🔍 Debug e Logs

### Console.WriteLine

```csharp
Console.WriteLine($"Entity {entity.Id} HP: {stats.CurrentHealth}/{stats.MaxHealth}");
```

### Verificar componentes

```csharp
if (entity.HasComponent<TransformComponent>())
{
    Console.WriteLine("Entidade tem Transform!");
}
```

### Contar entidades

```csharp
var count = _world.GetEntities().Count();
Console.WriteLine($"Total de entidades: {count}");
```

## 🚀 Otimizações Futuras

### Cache de queries ECS

```csharp
// Em vez de:
foreach (var entity in entities)
{
    if (!entity.HasComponents(typeof(Transform), typeof(Render))) continue;
    // ...
}

// Use:
var renderableEntities = _world.GetEntitiesWithComponents(
    typeof(TransformComponent), 
    typeof(RenderComponent)
);
foreach (var entity in renderableEntities)
{
    // Garantido que tem os componentes
}
```

### Object pooling para entidades

```csharp
// Para criar/destruir muitas entidades (projéteis, partículas)
private Queue<Entity> _entityPool = new();

public Entity GetPooledEntity()
{
    if (_entityPool.Count > 0)
    {
        var entity = _entityPool.Dequeue();
        entity.IsActive = true;
        return entity;
    }
    return _world.CreateEntity();
}

public void ReturnToPool(Entity entity)
{
    entity.IsActive = false;
    _entityPool.Enqueue(entity);
}
```

## 📐 Padrões de Código

### Nomenclatura

- **Componentes**: `NomeComponent` (ex: `TransformComponent`)
- **Sistemas**: `NomeSystem` (ex: `RenderSystem`)
- **Interfaces**: `INome` (ex: `IComponent`)
- **Campos privados**: `_nome` (ex: `_world`)
- **Propriedades**: `PascalCase` (ex: `Position`)
- **Métodos**: `PascalCase` (ex: `GetComponent()`)

### Organização de arquivos

```
1 classe = 1 arquivo
Nome do arquivo = Nome da classe
```

## 🔗 Links Úteis

- **MonoGame Docs**: https://docs.monogame.net/
- **.NET API**: https://learn.microsoft.com/dotnet/
- **C# Reference**: https://learn.microsoft.com/dotnet/csharp/

---

**Tip**: Use `Ctrl+C` no terminal para parar o jogo em execução.
