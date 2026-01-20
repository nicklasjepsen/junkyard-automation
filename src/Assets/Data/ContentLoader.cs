using System;
using System.IO;
using UnityEngine;

namespace JunkyardAutomation.Data
{
    /// <summary>
    /// Loads content definitions from JSON files in StreamingAssets.
    /// </summary>
    public static class ContentLoader
    {
        private const string DATA_FOLDER = "Data";

        /// <summary>
        /// Load all content from StreamingAssets/Data folder.
        /// </summary>
        public static void LoadAllContent()
        {
            Debug.Log("[ContentLoader] Loading content from StreamingAssets...");

            LoadItems();
            LoadMachines();
            LoadRecipes();

            Debug.Log($"[ContentLoader] Content loaded: {ContentRegistry.ItemCount} items, " +
                      $"{ContentRegistry.MachineCount} machines, {ContentRegistry.RecipeCount} recipes");
        }

        private static void LoadItems()
        {
            string json = LoadJsonFile("items.json");
            if (string.IsNullOrEmpty(json))
            {
                Debug.LogWarning("[ContentLoader] items.json not found or empty");
                return;
            }

            try
            {
                var list = JsonUtility.FromJson<ItemDefinitionList>(json);
                if (list?.items == null)
                {
                    Debug.LogError("[ContentLoader] items.json has no 'items' array");
                    return;
                }

                foreach (var item in list.items)
                {
                    if (string.IsNullOrEmpty(item.id))
                    {
                        Debug.LogWarning("[ContentLoader] Skipping item with empty id");
                        continue;
                    }
                    ContentRegistry.RegisterItem(item);
                }

                Debug.Log($"[ContentLoader] Loaded {list.items.Length} items");
            }
            catch (Exception e)
            {
                Debug.LogError($"[ContentLoader] Failed to parse items.json: {e.Message}");
            }
        }

        private static void LoadMachines()
        {
            string json = LoadJsonFile("machines.json");
            if (string.IsNullOrEmpty(json))
            {
                Debug.LogWarning("[ContentLoader] machines.json not found or empty");
                return;
            }

            try
            {
                var list = JsonUtility.FromJson<MachineDefinitionList>(json);
                if (list?.machines == null)
                {
                    Debug.LogError("[ContentLoader] machines.json has no 'machines' array");
                    return;
                }

                foreach (var machine in list.machines)
                {
                    if (string.IsNullOrEmpty(machine.id))
                    {
                        Debug.LogWarning("[ContentLoader] Skipping machine with empty id");
                        continue;
                    }
                    ContentRegistry.RegisterMachine(machine);
                }

                Debug.Log($"[ContentLoader] Loaded {list.machines.Length} machines");
            }
            catch (Exception e)
            {
                Debug.LogError($"[ContentLoader] Failed to parse machines.json: {e.Message}");
            }
        }

        private static void LoadRecipes()
        {
            string json = LoadJsonFile("recipes.json");
            if (string.IsNullOrEmpty(json))
            {
                Debug.LogWarning("[ContentLoader] recipes.json not found or empty");
                return;
            }

            try
            {
                var list = JsonUtility.FromJson<RecipeDefinitionList>(json);
                if (list?.recipes == null)
                {
                    Debug.LogError("[ContentLoader] recipes.json has no 'recipes' array");
                    return;
                }

                foreach (var recipe in list.recipes)
                {
                    if (string.IsNullOrEmpty(recipe.id))
                    {
                        Debug.LogWarning("[ContentLoader] Skipping recipe with empty id");
                        continue;
                    }
                    ContentRegistry.RegisterRecipe(recipe);
                }

                Debug.Log($"[ContentLoader] Loaded {list.recipes.Length} recipes");
            }
            catch (Exception e)
            {
                Debug.LogError($"[ContentLoader] Failed to parse recipes.json: {e.Message}");
            }
        }

        private static string LoadJsonFile(string filename)
        {
            string path = Path.Combine(Application.streamingAssetsPath, DATA_FOLDER, filename);

            if (!File.Exists(path))
            {
                Debug.LogWarning($"[ContentLoader] File not found: {path}");
                return null;
            }

            try
            {
                return File.ReadAllText(path);
            }
            catch (Exception e)
            {
                Debug.LogError($"[ContentLoader] Failed to read {filename}: {e.Message}");
                return null;
            }
        }
    }
}
