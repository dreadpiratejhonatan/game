namespace ModularGameEngine.Engine.ECS;

/// <summary>
/// Gerencia todas as entidades e sistemas do jogo.
/// O World é o container principal do ECS.
/// </summary>
public class World
{
    private readonly List<Entity> _entities = new();
    private readonly List<ISystem> _systems = new();
    
    /// <summary>
    /// Cria uma nova entidade no mundo.
    /// </summary>
    public Entity CreateEntity()
    {
        var entity = new Entity();
        _entities.Add(entity);
        return entity;
    }
    
    /// <summary>
    /// Remove uma entidade do mundo.
    /// </summary>
    public void DestroyEntity(Entity entity)
    {
        entity.IsActive = false;
        _entities.Remove(entity);
    }
    
    /// <summary>
    /// Adiciona um sistema ao mundo.
    /// </summary>
    public void AddSystem(ISystem system)
    {
        _systems.Add(system);
    }
    
    /// <summary>
    /// Atualiza todos os sistemas com as entidades ativas.
    /// </summary>
    public void Update(float deltaTime)
    {
        var activeEntities = _entities.Where(e => e.IsActive);
        
        foreach (var system in _systems)
        {
            system.Update(activeEntities, deltaTime);
        }
    }
    
    /// <summary>
    /// Obtém todas as entidades ativas.
    /// </summary>
    public IEnumerable<Entity> GetEntities() => _entities.Where(e => e.IsActive);
    
    /// <summary>
    /// Obtém todas as entidades com componentes específicos.
    /// </summary>
    public IEnumerable<Entity> GetEntitiesWithComponents(params Type[] componentTypes)
    {
        return _entities.Where(e => e.IsActive && e.HasComponents(componentTypes));
    }
}
