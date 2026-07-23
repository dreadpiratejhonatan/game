using System.Text.Json;
using ModularGameEngine.Mods.Models;

namespace ModularGameEngine.Mods;

/// <summary>
/// Carrega conteúdo data-driven de data/base_mod e data/user_mods.
/// Ordem: base primeiro; depois cada pasta em user_mods (ordem alfabética).
/// </summary>
public class ModManager
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
        AllowTrailingCommas = true
    };

    private readonly string _dataPath;
    private readonly Dictionary<string, UnitDefinition> _unitsById = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, SceneDefinition> _scenesById = new(StringComparer.OrdinalIgnoreCase);
    private readonly List<ModManifest> _loadedMods = new();

    public IReadOnlyList<UnitDefinition> UnitDefinitions => _unitsById.Values.ToList();
    public IReadOnlyList<SceneDefinition> Scenes => _scenesById.Values.ToList();
    public IReadOnlyList<ModManifest> LoadedMods => _loadedMods.AsReadOnly();

    public ModManager(string dataPath = "data")
    {
        _dataPath = dataPath;
    }

    public void LoadMods()
    {
        Console.WriteLine("[ModManager] Iniciando carregamento de mods...");
        _unitsById.Clear();
        _scenesById.Clear();
        _loadedMods.Clear();

        LoadModFolder(Path.Combine(_dataPath, "base_mod"), isBase: true);

        var userModsRoot = Path.Combine(_dataPath, "user_mods");
        if (Directory.Exists(userModsRoot))
        {
            foreach (var modDir in Directory.GetDirectories(userModsRoot).OrderBy(d => d, StringComparer.OrdinalIgnoreCase))
            {
                var name = Path.GetFileName(modDir);
                if (name.StartsWith('_') || name.StartsWith('.'))
                    continue;

                LoadModFolder(modDir, isBase: false);
            }
        }
        else
        {
            Directory.CreateDirectory(userModsRoot);
            Console.WriteLine($"[ModManager] Pasta criada: {userModsRoot}");
        }

        Console.WriteLine(
            $"[ModManager] Concluído. {_loadedMods.Count} mod(s), {_unitsById.Count} unidade(s), {_scenesById.Count} cena(s).");
    }

    private void LoadModFolder(string modPath, bool isBase)
    {
        if (!Directory.Exists(modPath))
        {
            if (isBase)
                Console.WriteLine($"[ModManager] AVISO: base_mod não encontrado em {modPath}");
            return;
        }

        var manifest = ReadManifest(modPath) ?? new ModManifest
        {
            Id = Path.GetFileName(modPath),
            Name = Path.GetFileName(modPath),
            Version = "0.0.0",
            Description = isBase ? "Conteúdo oficial" : "Mod sem mod.json"
        };

        Console.WriteLine($"[ModManager] Carregando: {manifest.Name} ({manifest.Id}) v{manifest.Version}");

        LoadUnits(modPath, manifest, isBase);
        LoadScenes(modPath, manifest, isBase);
        _loadedMods.Add(manifest);
    }

    private ModManifest? ReadManifest(string modPath)
    {
        var file = Path.Combine(modPath, "mod.json");
        if (!File.Exists(file)) return null;

        try
        {
            return JsonSerializer.Deserialize<ModManifest>(File.ReadAllText(file), JsonOptions);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ModManager] ERRO ao ler mod.json em {modPath}: {ex.Message}");
            return null;
        }
    }

    private void LoadUnits(string modPath, ModManifest manifest, bool isBase)
    {
        var unitsFile = Path.Combine(modPath, "units.json");
        if (!File.Exists(unitsFile))
        {
            if (isBase)
                Console.WriteLine($"[ModManager] AVISO: {unitsFile} não encontrado.");
            return;
        }

        try
        {
            var wrapper = JsonSerializer.Deserialize<UnitsWrapper>(File.ReadAllText(unitsFile), JsonOptions);
            if (wrapper?.Units == null) return;

            var added = 0;
            var replaced = 0;

            foreach (var unit in wrapper.Units)
            {
                if (string.IsNullOrWhiteSpace(unit.Id))
                {
                    Console.WriteLine("[ModManager] AVISO: unidade sem id ignorada.");
                    continue;
                }

                if (_unitsById.ContainsKey(unit.Id))
                {
                    if (!isBase && manifest.OverrideBase)
                    {
                        _unitsById[unit.Id] = unit;
                        replaced++;
                    }
                    else
                    {
                        Console.WriteLine($"[ModManager] AVISO: id duplicado '{unit.Id}' ignorado ({manifest.Id}).");
                    }
                }
                else
                {
                    _unitsById[unit.Id] = unit;
                    added++;
                }
            }

            Console.WriteLine(
                $"[ModManager]   units.json → +{added} novas" +
                (replaced > 0 ? $", ~{replaced} substituídas" : string.Empty));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ModManager] ERRO ao carregar {unitsFile}: {ex.Message}");
        }
    }

    private void LoadScenes(string modPath, ModManifest manifest, bool isBase)
    {
        var scenesDir = Path.Combine(modPath, "scenes");
        if (!Directory.Exists(scenesDir))
            return;

        var loaded = 0;
        foreach (var file in Directory.GetFiles(scenesDir, "*.json").OrderBy(f => f, StringComparer.OrdinalIgnoreCase))
        {
            try
            {
                var scene = JsonSerializer.Deserialize<SceneDefinition>(File.ReadAllText(file), JsonOptions);
                if (scene == null || string.IsNullOrWhiteSpace(scene.Id))
                {
                    Console.WriteLine($"[ModManager] AVISO: cena inválida em {file}");
                    continue;
                }

                if (_scenesById.ContainsKey(scene.Id) && (isBase || !manifest.OverrideBase))
                {
                    Console.WriteLine($"[ModManager] AVISO: cena duplicada '{scene.Id}' ignorada.");
                    continue;
                }

                _scenesById[scene.Id] = scene;
                loaded++;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ModManager] ERRO ao carregar {file}: {ex.Message}");
            }
        }

        if (loaded > 0)
            Console.WriteLine($"[ModManager]   scenes/ → {loaded} cena(s)");
    }

    public UnitDefinition? GetUnitDefinition(string unitId)
    {
        _unitsById.TryGetValue(unitId, out var def);
        return def;
    }

    public SceneDefinition? GetScene(string sceneId)
    {
        _scenesById.TryGetValue(sceneId, out var scene);
        return scene;
    }

    public UnitDefinition? GetRandomUnitDefinition(Random random)
    {
        if (_unitsById.Count == 0) return null;
        return _unitsById.Values.ElementAt(random.Next(_unitsById.Count));
    }

    /// <summary>
    /// Valida se todos os unit_id referenciados na cena existem.
    /// </summary>
    public IReadOnlyList<string> ValidateScene(SceneDefinition scene)
    {
        var missing = new List<string>();
        if (scene.Player != null && !_unitsById.ContainsKey(scene.Player.UnitId))
            missing.Add(scene.Player.UnitId);

        foreach (var spawn in scene.Spawns)
        {
            if (!_unitsById.ContainsKey(spawn.UnitId) && !missing.Contains(spawn.UnitId))
                missing.Add(spawn.UnitId);
        }

        return missing;
    }
}
