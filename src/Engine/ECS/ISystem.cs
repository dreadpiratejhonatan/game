namespace ModularGameEngine.Engine.ECS;

/// <summary>
/// Interface base para todos os sistemas do ECS.
/// Sistemas contêm a lógica de comportamento que opera sobre entidades com componentes específicos.
/// </summary>
public interface ISystem
{
    /// <summary>
    /// Atualiza o sistema a cada frame.
    /// </summary>
    /// <param name="entities">Lista de todas as entidades ativas no mundo.</param>
    /// <param name="deltaTime">Tempo decorrido desde o último frame em segundos.</param>
    void Update(IEnumerable<Entity> entities, float deltaTime);
}
