using System.Text.Json.Serialization;

namespace ModularGameEngine.Mods.Models;

/// <summary>
/// Modelos de dados carregados de JSON. Contrato público para autores de mods.
/// Novos tipos de conteúdo (items, skills) devem seguir o mesmo padrão nesta pasta.
/// </summary>
public class UnitsWrapper
{
    [JsonPropertyName("units")]
    public List<UnitDefinition> Units { get; set; } = new();
}

public class UnitDefinition
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    /// <summary>Facção: player | ally | enemy | neutral</summary>
    [JsonPropertyName("faction")]
    public string Faction { get; set; } = "neutral";

    [JsonPropertyName("stats")]
    public UnitStats Stats { get; set; } = new();

    [JsonPropertyName("visual")]
    public UnitVisual Visual { get; set; } = new();
}

public class UnitStats
{
    [JsonPropertyName("max_health")]
    public float MaxHealth { get; set; }

    [JsonPropertyName("move_speed")]
    public float MoveSpeed { get; set; }

    [JsonPropertyName("attack_damage")]
    public float AttackDamage { get; set; }

    [JsonPropertyName("attack_range")]
    public float AttackRange { get; set; }

    [JsonPropertyName("attack_cooldown")]
    public float AttackCooldown { get; set; } = 0.8f;

    [JsonPropertyName("detection_range")]
    public float DetectionRange { get; set; } = 220f;
}

public class UnitVisual
{
    [JsonPropertyName("width")]
    public float Width { get; set; }

    [JsonPropertyName("height")]
    public float Height { get; set; }

    [JsonPropertyName("color")]
    public string Color { get; set; } = "#FFFFFF";

    [JsonPropertyName("shape")]
    public string Shape { get; set; } = "rectangle";
}
