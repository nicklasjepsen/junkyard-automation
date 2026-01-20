using Godot;
using System.Collections.Generic;

namespace JunkyardAutomation;

/// <summary>
/// ContentRegistry - Central access point for loaded game definitions.
/// Stores and provides access to all loaded content definitions.
/// </summary>
public partial class ContentRegistry : Node
{
    [Signal]
    public delegate void ContentLoadedEventHandler();

    /// <summary>Loaded item definitions (item_id -> definition)</summary>
    public Dictionary<string, Dictionary<string, Variant>> Items { get; } = new();

    /// <summary>Loaded machine definitions (machine_id -> definition)</summary>
    public Dictionary<string, Dictionary<string, Variant>> Machines { get; } = new();

    /// <summary>Loaded recipe definitions (recipe_id -> definition)</summary>
    public Dictionary<string, Dictionary<string, Variant>> Recipes { get; } = new();

    /// <summary>Whether content has been loaded</summary>
    public bool IsLoaded { get; private set; } = false;

    /// <summary>
    /// Load all content from JSON files.
    /// </summary>
    public void LoadContent()
    {
        LoadItems();
        LoadMachines();
        LoadRecipes();
        IsLoaded = true;
        EmitSignal(SignalName.ContentLoaded);
    }

    /// <summary>
    /// Get an item definition by ID.
    /// </summary>
    public Dictionary<string, Variant>? GetItem(string itemId)
    {
        if (Items.TryGetValue(itemId, out var item))
            return item;

        GD.PushWarning($"Item not found: {itemId}");
        return null;
    }

    /// <summary>
    /// Get a machine definition by ID.
    /// </summary>
    public Dictionary<string, Variant>? GetMachine(string machineId)
    {
        if (Machines.TryGetValue(machineId, out var machine))
            return machine;

        GD.PushWarning($"Machine not found: {machineId}");
        return null;
    }

    /// <summary>
    /// Get a recipe definition by ID.
    /// </summary>
    public Dictionary<string, Variant>? GetRecipe(string recipeId)
    {
        if (Recipes.TryGetValue(recipeId, out var recipe))
            return recipe;

        GD.PushWarning($"Recipe not found: {recipeId}");
        return null;
    }

    /// <summary>
    /// Get all item IDs.
    /// </summary>
    public IEnumerable<string> GetAllItemIds() => Items.Keys;

    /// <summary>
    /// Get all machine IDs.
    /// </summary>
    public IEnumerable<string> GetAllMachineIds() => Machines.Keys;

    /// <summary>
    /// Get all recipe IDs.
    /// </summary>
    public IEnumerable<string> GetAllRecipeIds() => Recipes.Keys;

    /// <summary>
    /// Get machines that can be placed by the player.
    /// </summary>
    public List<Dictionary<string, Variant>> GetPlaceableMachines()
    {
        var placeable = new List<Dictionary<string, Variant>>();
        foreach (var machine in Machines.Values)
        {
            bool isPlaceable = true;
            if (machine.TryGetValue("placeable", out var val))
            {
                isPlaceable = val.AsBool();
            }
            if (isPlaceable)
            {
                placeable.Add(machine);
            }
        }
        return placeable;
    }

    /// <summary>
    /// Get recipes that a specific machine can perform.
    /// </summary>
    public List<Dictionary<string, Variant>> GetRecipesForMachine(string machineId)
    {
        var matching = new List<Dictionary<string, Variant>>();
        foreach (var recipe in Recipes.Values)
        {
            if (recipe.TryGetValue("machine_id", out var val) && val.AsString() == machineId)
            {
                matching.Add(recipe);
            }
        }
        return matching;
    }

    private void LoadItems()
    {
        var data = LoadJsonFile("res://data/items.json");
        if (data.VariantType == Variant.Type.Array)
        {
            foreach (var item in data.AsGodotArray())
            {
                var dict = item.AsGodotDictionary();
                if (dict.TryGetValue("id", out var id))
                {
                    Items[id.AsString()] = ConvertToTypedDict(dict);
                }
            }
        }
    }

    private void LoadMachines()
    {
        var data = LoadJsonFile("res://data/machines.json");
        if (data.VariantType == Variant.Type.Array)
        {
            foreach (var machine in data.AsGodotArray())
            {
                var dict = machine.AsGodotDictionary();
                if (dict.TryGetValue("id", out var id))
                {
                    Machines[id.AsString()] = ConvertToTypedDict(dict);
                }
            }
        }
    }

    private void LoadRecipes()
    {
        var data = LoadJsonFile("res://data/recipes.json");
        if (data.VariantType == Variant.Type.Array)
        {
            foreach (var recipe in data.AsGodotArray())
            {
                var dict = recipe.AsGodotDictionary();
                if (dict.TryGetValue("id", out var id))
                {
                    Recipes[id.AsString()] = ConvertToTypedDict(dict);
                }
            }
        }
    }

    private static Dictionary<string, Variant> ConvertToTypedDict(Godot.Collections.Dictionary dict)
    {
        var result = new Dictionary<string, Variant>();
        foreach (var key in dict.Keys)
        {
            result[key.AsString()] = dict[key];
        }
        return result;
    }

    private static Variant LoadJsonFile(string path)
    {
        if (!FileAccess.FileExists(path))
        {
            GD.PushWarning($"JSON file not found: {path}");
            return new Variant();
        }

        using var file = FileAccess.Open(path, FileAccess.ModeFlags.Read);
        if (file == null)
        {
            GD.PushError($"Failed to open file: {path}");
            return new Variant();
        }

        var content = file.GetAsText();
        var json = new Json();
        var error = json.Parse(content);

        if (error != Error.Ok)
        {
            GD.PushError($"Failed to parse {path}: {json.GetErrorMessage()}");
            return new Variant();
        }

        return json.Data;
    }
}
