using ModularGameEngine.Engine.ECS;

namespace ModularGameEngine.Game.Components;

/// <summary>
/// Facção da unidade (carregada do JSON). Define quem é inimigo de quem.
/// Valores padrão: "player", "ally", "enemy", "neutral".
/// </summary>
public class TeamComponent : IComponent
{
    public string Faction { get; set; }

    public TeamComponent(string faction)
    {
        Faction = faction;
    }

    public static bool AreHostile(string factionA, string factionB)
    {
        if (string.IsNullOrEmpty(factionA) || string.IsNullOrEmpty(factionB))
            return false;
        if (factionA == factionB)
            return false;
        if (factionA == "neutral" || factionB == "neutral")
            return false;

        // inimigos atacam player/ally; player/ally atacam inimigos
        var aEnemy = factionA == "enemy";
        var bEnemy = factionB == "enemy";
        return aEnemy != bEnemy;
    }
}
