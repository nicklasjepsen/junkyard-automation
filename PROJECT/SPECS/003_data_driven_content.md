# Feature Spec: Data-Driven Content System

## Goal
Load game content (items, machines, recipes) from JSON files at runtime, enabling easy content iteration and modding support.

## Non-goals
- Contract definitions (future PR)
- Save/load system (future PR)
- In-game content editor (future)
- Hot-reload during play (nice-to-have, not M0)

## Design references
- ENGINE/ARCHITECTURE.md - Data loading structure
- DESIGN/CONTENT_CATALOG.md - Item and machine definitions
- DESIGN/SYSTEMS_PROCESSING.md - Processing recipes

## User stories
- As a developer, I want item stats defined in JSON so I can iterate without recompiling
- As a developer, I want machine definitions in JSON so I can add new machines easily
- As a developer, I want recipe definitions in JSON so I can tweak processing chains
- As a developer, I want the placement system to use machine definitions from JSON

## Data Schemas

### ItemDefinition (items.json)
```json
{
  "items": [
    {
      "id": "ScrapFerrous",
      "displayName": "Ferrous Scrap",
      "category": "scrap",
      "color": "#8B7355",
      "sellPrice": 1
    }
  ]
}
```

### MachineDefinition (machines.json)
```json
{
  "machines": [
    {
      "id": "Conveyor",
      "displayName": "Conveyor Belt",
      "category": "logistics",
      "size": { "x": 1, "y": 1 },
      "cost": 10,
      "color": "#666666",
      "ticksPerMove": 4
    },
    {
      "id": "Shredder",
      "displayName": "Shredder",
      "category": "processor",
      "size": { "x": 2, "y": 2 },
      "cost": 250,
      "color": "#AA4444",
      "processingTicks": 20,
      "inputSlots": 1,
      "outputSlots": 1
    }
  ]
}
```

### RecipeDefinition (recipes.json)
```json
{
  "recipes": [
    {
      "id": "shred_ferrous",
      "machineType": "Shredder",
      "inputs": [{ "itemId": "ScrapFerrous", "count": 1 }],
      "outputs": [{ "itemId": "ShreddedFerrous", "count": 1 }],
      "processingTicks": 20
    }
  ]
}
```

## Implementation

### Classes

#### ItemDefinition.cs
```csharp
[Serializable]
public class ItemDefinition
{
    public string id;
    public string displayName;
    public string category;
    public string color;
    public int sellPrice;
}
```

#### MachineDefinition.cs
```csharp
[Serializable]
public class MachineDefinition
{
    public string id;
    public string displayName;
    public string category;
    public Vector2IntSerializable size;
    public int cost;
    public string color;
    public int ticksPerMove;      // For conveyors
    public int processingTicks;   // For processors
    public int inputSlots;
    public int outputSlots;
}
```

#### RecipeDefinition.cs
```csharp
[Serializable]
public class RecipeDefinition
{
    public string id;
    public string machineType;
    public RecipeSlot[] inputs;
    public RecipeSlot[] outputs;
    public int processingTicks;
}
```

#### ContentLoader.cs
- Loads JSON from StreamingAssets folder
- Parses using Unity's JsonUtility
- Validates required fields
- Logs errors for missing/invalid data

#### ContentRegistry.cs
- Singleton holding all loaded definitions
- Dictionary<string, ItemDefinition> items
- Dictionary<string, MachineDefinition> machines
- Dictionary<string, RecipeDefinition> recipes
- List<RecipeDefinition> GetRecipesForMachine(string machineId)

### File Structure
```
src/StreamingAssets/
├── Data/
│   ├── items.json
│   ├── machines.json
│   └── recipes.json
```

## Integration

### PlacementManager
- Query ContentRegistry for machine size when placing
- Use machine color for ghost/placed visuals

### MachineVisualManager
- Use MachineDefinition.size for multi-tile machines
- Use MachineDefinition.color for rendering

### BuildMenuUI (future)
- Populate buttons from ContentRegistry.machines

## Acceptance criteria
- [ ] JSON files exist in StreamingAssets/Data/
- [ ] ContentLoader parses items.json, machines.json, recipes.json
- [ ] ContentRegistry provides access to all definitions
- [ ] All v1 items from CONTENT_CATALOG.md are defined
- [ ] All v1 machines from CONTENT_CATALOG.md are defined
- [ ] PlacementManager uses machine size from registry
- [ ] Console shows loaded content count on startup
- [ ] Invalid JSON logs clear error message

## Test plan

### Manual
1. Start play mode - should see "Loaded X items, Y machines, Z recipes" in console
2. Modify items.json while not in play mode
3. Start play mode - new values should be loaded
4. Delete a required field from JSON - should see error in console
5. Place a conveyor - should work as before (size from registry)

### Edge cases
- Missing JSON file - graceful error, game still runs with empty registry
- Malformed JSON - clear error message with line number if possible
- Duplicate IDs - warn and use first definition
