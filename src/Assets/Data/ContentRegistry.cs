using System.Collections.Generic;
using UnityEngine;

namespace JunkyardAutomation.Data
{
    /// <summary>
    /// Central registry for all loaded content definitions.
    /// Provides read-only access to items, machines, and recipes.
    /// </summary>
    public static class ContentRegistry
    {
        private static Dictionary<string, ItemDefinition> items = new Dictionary<string, ItemDefinition>();
        private static Dictionary<string, MachineDefinition> machines = new Dictionary<string, MachineDefinition>();
        private static Dictionary<string, RecipeDefinition> recipes = new Dictionary<string, RecipeDefinition>();
        private static Dictionary<string, List<RecipeDefinition>> recipesByMachine = new Dictionary<string, List<RecipeDefinition>>();

        public static int ItemCount => items.Count;
        public static int MachineCount => machines.Count;
        public static int RecipeCount => recipes.Count;

        public static bool IsLoaded { get; private set; }

        /// <summary>
        /// Clear all registered content (called before reload).
        /// </summary>
        public static void Clear()
        {
            items.Clear();
            machines.Clear();
            recipes.Clear();
            recipesByMachine.Clear();
            IsLoaded = false;
        }

        /// <summary>
        /// Mark registry as loaded (called after ContentLoader finishes).
        /// </summary>
        public static void MarkLoaded()
        {
            IsLoaded = true;
        }

        #region Registration (called by ContentLoader)

        public static void RegisterItem(ItemDefinition item)
        {
            if (items.ContainsKey(item.id))
            {
                Debug.LogWarning($"[ContentRegistry] Duplicate item id: {item.id}, keeping first");
                return;
            }
            items[item.id] = item;
        }

        public static void RegisterMachine(MachineDefinition machine)
        {
            if (machines.ContainsKey(machine.id))
            {
                Debug.LogWarning($"[ContentRegistry] Duplicate machine id: {machine.id}, keeping first");
                return;
            }
            machines[machine.id] = machine;
        }

        public static void RegisterRecipe(RecipeDefinition recipe)
        {
            if (recipes.ContainsKey(recipe.id))
            {
                Debug.LogWarning($"[ContentRegistry] Duplicate recipe id: {recipe.id}, keeping first");
                return;
            }
            recipes[recipe.id] = recipe;

            // Index by machine type for fast lookup
            if (!string.IsNullOrEmpty(recipe.machineType))
            {
                if (!recipesByMachine.ContainsKey(recipe.machineType))
                {
                    recipesByMachine[recipe.machineType] = new List<RecipeDefinition>();
                }
                recipesByMachine[recipe.machineType].Add(recipe);
            }
        }

        #endregion

        #region Accessors

        /// <summary>
        /// Get item definition by id, or null if not found.
        /// </summary>
        public static ItemDefinition GetItem(string id)
        {
            return items.TryGetValue(id, out var item) ? item : null;
        }

        /// <summary>
        /// Get machine definition by id, or null if not found.
        /// </summary>
        public static MachineDefinition GetMachine(string id)
        {
            return machines.TryGetValue(id, out var machine) ? machine : null;
        }

        /// <summary>
        /// Get recipe definition by id, or null if not found.
        /// </summary>
        public static RecipeDefinition GetRecipe(string id)
        {
            return recipes.TryGetValue(id, out var recipe) ? recipe : null;
        }

        /// <summary>
        /// Get all recipes that can run on a given machine type.
        /// </summary>
        public static IReadOnlyList<RecipeDefinition> GetRecipesForMachine(string machineType)
        {
            if (recipesByMachine.TryGetValue(machineType, out var list))
                return list;
            return System.Array.Empty<RecipeDefinition>();
        }

        /// <summary>
        /// Get all registered items.
        /// </summary>
        public static IEnumerable<ItemDefinition> GetAllItems()
        {
            return items.Values;
        }

        /// <summary>
        /// Get all registered machines.
        /// </summary>
        public static IEnumerable<MachineDefinition> GetAllMachines()
        {
            return machines.Values;
        }

        /// <summary>
        /// Get all registered recipes.
        /// </summary>
        public static IEnumerable<RecipeDefinition> GetAllRecipes()
        {
            return recipes.Values;
        }

        /// <summary>
        /// Check if a machine id exists.
        /// </summary>
        public static bool HasMachine(string id)
        {
            return machines.ContainsKey(id);
        }

        /// <summary>
        /// Check if an item id exists.
        /// </summary>
        public static bool HasItem(string id)
        {
            return items.ContainsKey(id);
        }

        #endregion
    }
}
