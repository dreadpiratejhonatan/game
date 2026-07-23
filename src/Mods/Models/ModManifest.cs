using System.Text.Json.Serialization;

namespace ModularGameEngine.Mods.Models;

/// <summary>
/// Metadados de um mod (arquivo mod.json na raiz da pasta do mod).
/// Obrigatório para mods em data/user_mods/; opcional no base_mod.
/// </summary>
public class ModManifest
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("version")]
    public string Version { get; set; } = "1.0.0";

    [JsonPropertyName("author")]
    public string Author { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Se true, unidades com o mesmo id substituem as do base_mod.
    /// Se false, ids duplicados são ignorados com aviso.
    /// </summary>
    [JsonPropertyName("override_base")]
    public bool OverrideBase { get; set; } = true;
}
