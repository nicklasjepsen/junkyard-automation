using System;
using System.Collections.Generic;
using UnityEngine;

namespace JunkyardAutomation.Data
{
    /// <summary>
    /// ScriptableObject that holds sprite references for machines and items.
    /// Assign sprites in the Unity Inspector.
    /// </summary>
    [CreateAssetMenu(fileName = "SpriteDatabase", menuName = "Junkyard/Sprite Database")]
    public class SpriteDatabase : ScriptableObject
    {
        [Header("Machine Sprites")]
        public SpriteEntry[] machineSprites;

        [Header("Item Sprites")]
        public SpriteEntry[] itemSprites;

        private Dictionary<string, Sprite> machineSpriteDict;
        private Dictionary<string, Sprite> itemSpriteDict;

        public void Initialize()
        {
            machineSpriteDict = new Dictionary<string, Sprite>();
            itemSpriteDict = new Dictionary<string, Sprite>();

            if (machineSprites != null)
            {
                foreach (var entry in machineSprites)
                {
                    if (!string.IsNullOrEmpty(entry.id) && entry.sprite != null)
                    {
                        machineSpriteDict[entry.id] = entry.sprite;
                    }
                }
            }

            if (itemSprites != null)
            {
                foreach (var entry in itemSprites)
                {
                    if (!string.IsNullOrEmpty(entry.id) && entry.sprite != null)
                    {
                        itemSpriteDict[entry.id] = entry.sprite;
                    }
                }
            }

            Debug.Log($"[SpriteDatabase] Initialized with {machineSpriteDict.Count} machine sprites, {itemSpriteDict.Count} item sprites");
        }

        public Sprite GetMachineSprite(string machineTypeId)
        {
            if (machineSpriteDict == null) Initialize();
            return machineSpriteDict.TryGetValue(machineTypeId, out var sprite) ? sprite : null;
        }

        public Sprite GetItemSprite(string itemTypeId)
        {
            if (itemSpriteDict == null) Initialize();
            return itemSpriteDict.TryGetValue(itemTypeId, out var sprite) ? sprite : null;
        }
    }

    [Serializable]
    public class SpriteEntry
    {
        public string id;
        public Sprite sprite;
    }
}
