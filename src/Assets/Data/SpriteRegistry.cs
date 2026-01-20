using UnityEngine;

namespace JunkyardAutomation.Data
{
    /// <summary>
    /// Runtime singleton that provides access to sprite assets.
    /// Uses a SpriteDatabase ScriptableObject for sprite references.
    /// </summary>
    public class SpriteRegistry : MonoBehaviour
    {
        public static SpriteRegistry Instance { get; private set; }

        [SerializeField] private SpriteDatabase spriteDatabase;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            if (spriteDatabase != null)
            {
                spriteDatabase.Initialize();
                Debug.Log("[SpriteRegistry] Initialized with SpriteDatabase");
            }
            else
            {
                Debug.LogWarning("[SpriteRegistry] No SpriteDatabase assigned!");
            }
        }

        /// <summary>
        /// Get sprite for a machine type.
        /// </summary>
        public Sprite GetMachineSprite(string machineTypeId)
        {
            return spriteDatabase?.GetMachineSprite(machineTypeId);
        }

        /// <summary>
        /// Get sprite for an item type.
        /// </summary>
        public Sprite GetItemSprite(string itemTypeId)
        {
            return spriteDatabase?.GetItemSprite(itemTypeId);
        }

        /// <summary>
        /// Check if sprites are available.
        /// </summary>
        public bool IsReady => spriteDatabase != null;
    }
}
