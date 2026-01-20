using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using JunkyardAutomation.Data;

namespace JunkyardAutomation.Editor
{
    /// <summary>
    /// Editor utility to create and populate the SpriteDatabase with factory icons.
    /// </summary>
    public static class SpriteDatabaseSetup
    {
        private const string ICON_FOLDER = "Assets/Sci Fi Factory Icons Pack Machines Tools Robotics and Engineering Assets/Source/";
        private const string DATABASE_PATH = "Assets/Data/SpriteDatabase.asset";

        // Machine icon mappings based on catalog.csv
        private static readonly Dictionary<string, int> MachineIconNumbers = new Dictionary<string, int>
        {
            { "Conveyor", 55 },       // Conveyor Belt Segment
            { "Splitter", 83 },       // Sorting Robot / Picker Bot
            { "Shredder", 45 },       // Recycling Machine / Compactor
            { "Smelter", 18 },        // Smelter / Furnace
            { "Washer", 38 },         // Water Pump / Fluid Processor
            { "MagnetSeparator", 103 }, // Robotic Arm / Industrial Manipulator
            { "Storage", 25 },        // Cargo Crate / Storage Container
            { "Seller", 112 },        // Compact Forklift / Loader Vehicle
            { "Delivery", 119 }       // Ore Crate / Mining Container
        };

        // Item icon mappings based on catalog.csv
        private static readonly Dictionary<string, int> ItemIconNumbers = new Dictionary<string, int>
        {
            { "ScrapFerrous", 34 },      // Parts Bin / Scrap Container
            { "ScrapNonFerrous", 47 },   // Copper Wire Spool
            { "ScrapPlastic", 110 },     // Ice Cube / Cooling Block
            { "ScrapTrash", 36 },        // Waste Bin / Recycling Container
            { "SteelIngot", 49 },        // I-Beam / Steel Girder
            { "CopperChunk", 12 },       // Energy Crystal / Power Shard
            { "PlasticPellets", 115 },   // Bubbling Flask / Chemical Potion
            { "ShreddedFerrous", 35 },   // Metal Plates / Floor Tiles
            { "ShreddedNonFerrous", 47 } // Copper Wire Spool (same as ScrapNonFerrous)
        };

        [MenuItem("Junkyard/Create Sprite Database")]
        public static void CreateSpriteDatabase()
        {
            // Check if database already exists
            SpriteDatabase existing = AssetDatabase.LoadAssetAtPath<SpriteDatabase>(DATABASE_PATH);
            if (existing != null)
            {
                if (!EditorUtility.DisplayDialog("Sprite Database Exists",
                    "A SpriteDatabase already exists. Do you want to overwrite it?",
                    "Yes", "No"))
                {
                    return;
                }
            }

            // Create the Data folder if it doesn't exist
            if (!AssetDatabase.IsValidFolder("Assets/Data"))
            {
                AssetDatabase.CreateFolder("Assets", "Data");
            }

            // Create new database
            SpriteDatabase database = ScriptableObject.CreateInstance<SpriteDatabase>();

            // Load machine sprites
            List<SpriteEntry> machineEntries = new List<SpriteEntry>();
            foreach (var kvp in MachineIconNumbers)
            {
                Sprite sprite = LoadIconSprite(kvp.Value);
                if (sprite != null)
                {
                    machineEntries.Add(new SpriteEntry { id = kvp.Key, sprite = sprite });
                    Debug.Log($"[SpriteDatabaseSetup] Loaded machine sprite: {kvp.Key} -> Icon {kvp.Value}");
                }
                else
                {
                    Debug.LogWarning($"[SpriteDatabaseSetup] Could not load sprite for machine: {kvp.Key} (Icon {kvp.Value})");
                }
            }
            database.machineSprites = machineEntries.ToArray();

            // Load item sprites
            List<SpriteEntry> itemEntries = new List<SpriteEntry>();
            foreach (var kvp in ItemIconNumbers)
            {
                Sprite sprite = LoadIconSprite(kvp.Value);
                if (sprite != null)
                {
                    itemEntries.Add(new SpriteEntry { id = kvp.Key, sprite = sprite });
                    Debug.Log($"[SpriteDatabaseSetup] Loaded item sprite: {kvp.Key} -> Icon {kvp.Value}");
                }
                else
                {
                    Debug.LogWarning($"[SpriteDatabaseSetup] Could not load sprite for item: {kvp.Key} (Icon {kvp.Value})");
                }
            }
            database.itemSprites = itemEntries.ToArray();

            // Save the asset
            if (existing != null)
            {
                EditorUtility.CopySerialized(database, existing);
                EditorUtility.SetDirty(existing);
            }
            else
            {
                AssetDatabase.CreateAsset(database, DATABASE_PATH);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log($"[SpriteDatabaseSetup] Created SpriteDatabase at {DATABASE_PATH}");
            Debug.Log($"[SpriteDatabaseSetup] Loaded {machineEntries.Count} machine sprites, {itemEntries.Count} item sprites");

            // Select the created asset
            Selection.activeObject = AssetDatabase.LoadAssetAtPath<SpriteDatabase>(DATABASE_PATH);
        }

        private static Sprite LoadIconSprite(int iconNumber)
        {
            string filename = $"Sci Fi Factory Icon ({iconNumber}).png";
            string fullPath = ICON_FOLDER + filename;

            // First, ensure the texture is set to Sprite mode
            TextureImporter importer = AssetImporter.GetAtPath(fullPath) as TextureImporter;
            if (importer != null && importer.textureType != TextureImporterType.Sprite)
            {
                importer.textureType = TextureImporterType.Sprite;
                importer.spriteImportMode = SpriteImportMode.Single;
                importer.SaveAndReimport();
            }

            // Load the sprite
            Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(fullPath);
            return sprite;
        }

        [MenuItem("Junkyard/Validate Sprite Database")]
        public static void ValidateSpriteDatabase()
        {
            SpriteDatabase database = AssetDatabase.LoadAssetAtPath<SpriteDatabase>(DATABASE_PATH);
            if (database == null)
            {
                Debug.LogError("[SpriteDatabaseSetup] No SpriteDatabase found at " + DATABASE_PATH);
                return;
            }

            int machineCount = database.machineSprites?.Length ?? 0;
            int itemCount = database.itemSprites?.Length ?? 0;

            Debug.Log($"[SpriteDatabaseSetup] SpriteDatabase contains {machineCount} machine sprites, {itemCount} item sprites");

            // Check for missing sprites
            if (database.machineSprites != null)
            {
                foreach (var entry in database.machineSprites)
                {
                    if (entry.sprite == null)
                    {
                        Debug.LogWarning($"[SpriteDatabaseSetup] Machine '{entry.id}' has null sprite!");
                    }
                }
            }

            if (database.itemSprites != null)
            {
                foreach (var entry in database.itemSprites)
                {
                    if (entry.sprite == null)
                    {
                        Debug.LogWarning($"[SpriteDatabaseSetup] Item '{entry.id}' has null sprite!");
                    }
                }
            }
        }
    }
}
