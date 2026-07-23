namespace ModularGameEngine.Engine.ECS;

/// <summary>
/// Representa uma entidade no sistema ECS.
/// Uma entidade é apenas um ID único com uma coleção de componentes.
/// </summary>
public class Entity
{
    private static int _nextId = 0;
    
    public int Id { get; }
    public bool IsActive { get; set; } = true;
    
    private readonly Dictionary<Type, IComponent> _components = new();
    
    public Entity()
    {
        Id = _nextId++;
    }
    
    /// <summary>
    /// Adiciona um componente à entidade.
    /// </summary>
    public void AddComponent<T>(T component) where T : IComponent
    {
        var type = typeof(T);
        if (_components.ContainsKey(type))
        {
            _components[type] = component;
        }
        else
        {
            _components.Add(type, component);
        }
    }
    
    /// <summary>
    /// Remove um componente da entidade.
    /// </summary>
    public void RemoveComponent<T>() where T : IComponent
    {
        _components.Remove(typeof(T));
    }
    
    /// <summary>
    /// Obtém um componente da entidade.
    /// </summary>
    public T? GetComponent<T>() where T : class, IComponent
    {
        _components.TryGetValue(typeof(T), out var component);
        return component as T;
    }
    
    /// <summary>
    /// Verifica se a entidade possui um componente específico.
    /// </summary>
    public bool HasComponent<T>() where T : IComponent
    {
        return _components.ContainsKey(typeof(T));
    }
    
    /// <summary>
    /// Verifica se a entidade possui todos os componentes especificados.
    /// </summary>
    public bool HasComponents(params Type[] componentTypes)
    {
        return componentTypes.All(type => _components.ContainsKey(type));
    }
}
