using System.Text.Json.Serialization;

namespace ModularGameEngine.Mods.Models;

/// <summary>
/// Cena data-driven (mapa/baseline). Contrato estável para testes e conteúdo.
/// </summary>
public class SceneDefinition
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("world_width")]
    public int WorldWidth { get; set; } = 3200;

    [JsonPropertyName("world_height")]
    public int WorldHeight { get; set; } = 2400;

    [JsonPropertyName("player")]
    public SceneActor? Player { get; set; }

    [JsonPropertyName("spawns")]
    public List<SceneActor> Spawns { get; set; } = new();
}

public class SceneActor
{
    [JsonPropertyName("unit_id")]
    public string UnitId { get; set; } = string.Empty;

    [JsonPropertyName("x")]
    public float X { get; set; }

    [JsonPropertyName("y")]
    public float Y { get; set; }

    /// <summary>
    /// Se definido, sobrescreve a facção do units.json (útil para testes).
    /// </summary>
    [JsonPropertyName("faction")]
    public string? Faction { get; set; }
}
