using Microsoft.Xna.Framework;
using ModularGameEngine.Engine.ECS;

namespace ModularGameEngine.Game.Components;

/// <summary>
/// Destino de movimento de uma unidade (definido pelo clique direito do jogador).
/// Null significa que a unidade está parada.
/// </summary>
public class MoveTargetComponent : IComponent
{
    public Vector2? Target { get; set; }
}
