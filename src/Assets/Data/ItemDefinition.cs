using System;
using UnityEngine;

namespace JunkyardAutomation.Data
{
    /// <summary>
    /// Definition for an item type (scrap, product, intermediate).
    /// Loaded from JSON at runtime.
    /// </summary>
    [Serializable]
    public class ItemDefinition
    {
        public string id;
        public string displayName;
        public string category;
        public string color;
        public int sellPrice;

        /// <summary>
        /// Parse the color string to a Unity Color.
        /// </summary>
        public Color GetColor()
        {
            if (ColorUtility.TryParseHtmlString(color, out Color c))
                return c;
            return Color.white;
        }
    }

    /// <summary>
    /// Wrapper for JSON array of items.
    /// </summary>
    [Serializable]
    public class ItemDefinitionList
    {
        public ItemDefinition[] items;
    }
}
