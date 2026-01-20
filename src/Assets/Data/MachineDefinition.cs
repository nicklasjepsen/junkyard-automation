using System;
using UnityEngine;

namespace JunkyardAutomation.Data
{
    /// <summary>
    /// Definition for a machine type.
    /// Loaded from JSON at runtime.
    /// </summary>
    [Serializable]
    public class MachineDefinition
    {
        public string id;
        public string displayName;
        public string category;
        public SerializableVector2Int size;
        public int cost;
        public string color;

        // Conveyor-specific
        public int ticksPerMove;

        // Processor-specific
        public int processingTicks;
        public int inputSlots;
        public int outputSlots;

        // Storage-specific
        public int storageCapacity;

        /// <summary>
        /// Get size as Vector2Int.
        /// </summary>
        public Vector2Int GetSize()
        {
            return size != null ? new Vector2Int(size.x, size.y) : Vector2Int.one;
        }

        /// <summary>
        /// Parse the color string to a Unity Color.
        /// </summary>
        public Color GetColor()
        {
            if (ColorUtility.TryParseHtmlString(color, out Color c))
                return c;
            return Color.gray;
        }
    }

    /// <summary>
    /// Serializable Vector2Int for JSON parsing.
    /// </summary>
    [Serializable]
    public class SerializableVector2Int
    {
        public int x;
        public int y;

        public SerializableVector2Int() { x = 1; y = 1; }
        public SerializableVector2Int(int x, int y) { this.x = x; this.y = y; }

        public Vector2Int ToVector2Int() => new Vector2Int(x, y);
    }

    /// <summary>
    /// Wrapper for JSON array of machines.
    /// </summary>
    [Serializable]
    public class MachineDefinitionList
    {
        public MachineDefinition[] machines;
    }
}
