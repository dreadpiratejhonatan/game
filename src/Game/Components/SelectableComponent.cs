using ModularGameEngine.Engine.ECS;

namespace ModularGameEngine.Game.Components;

/// <summary>
/// Marca uma entidade como selecionável pelo jogador (clique ou caixa de seleção).
/// </summary>
public class SelectableComponent : IComponent
{
    public bool IsSelected { get; set; }
}
