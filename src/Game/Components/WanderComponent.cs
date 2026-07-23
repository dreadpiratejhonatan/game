using ModularGameEngine.Engine.ECS;

namespace ModularGameEngine.Game.Components;

/// <summary>
/// Faz a unidade vagar sozinha pelo mapa quando está ociosa (comportamento ambiente,
/// estilo NPCs de Kenshi). Unidades selecionadas pelo jogador não vagam.
/// </summary>
public class WanderComponent : IComponent
{
    /// <summary>Tempo restante até decidir o próximo passeio.</summary>
    public float Cooldown { get; set; }
}
