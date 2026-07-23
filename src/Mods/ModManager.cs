using System.Text.Json;
using ModularGameEngine.Mods.Models;

namespace ModularGameEngine.Mods;

/// <summary>
/// Carrega conteúdo data-driven de data/base_mod e data/user_mods.
/// Ordem: base primeiro; depois cada pasta em user_mods (ordem alfabética).
/// Mods com override_base=true substituem unidades de mesmo id.
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
    private readonly List<ModManifest> _loadedMods = new();

    public IReadOnlyList<UnitDefinition> UnitDefinitions => _unitsById.Values.ToList();
    public IReadOnlyList<ModManifest> LoadedMods => _loadedMods.AsReadOnly();

    public ModManager(string dataPath = "data")
    {
        _dataPath = dataPath;
    }

    public void LoadMods()
    {
        Console.WriteLine("[ModManager] Iniciando carregamento de mods...");
        _unitsById.Clear();
        _loadedMods.Clear();

        LoadModFolder(Path.Combine(_dataPath, "base_mod"), isBase: true);

        var userModsRoot = Path.Combine(_dataPath, "user_mods");
        if (Directory.Exists(userModsRoot))
        {
            foreach (var modDir in Directory.GetDirectories(userModsRoot).OrderBy(d => d, StringComparer.OrdinalIgnoreCase))
            {
                // Ignora pastas que começam com _ ou .
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
            $"[ModManager] Concluído. {_loadedMods.Count} mod(s), {_unitsById.Count} unidade(s) ativas.");
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
        _loadedMods.Add(manifest);

        // Extensível: LoadItems(modPath, manifest); LoadSkills(modPath, manifest);
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

    public UnitDefinition? GetUnitDefinition(string unitId)
    {
        _unitsById.TryGetValue(unitId, out var def);
        return def;
    }

    public UnitDefinition? GetRandomUnitDefinition(Random random)
    {
        if (_unitsById.Count == 0) return null;
        return _unitsById.Values.ElementAt(random.Next(_unitsById.Count));
    }
}
