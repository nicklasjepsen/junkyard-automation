using System;

namespace JunkyardAutomation.Data
{
    /// <summary>
    /// Definition for a processing recipe.
    /// Specifies what a machine can produce from inputs.
    /// </summary>
    [Serializable]
    public class RecipeDefinition
    {
        public string id;
        public string machineType;
        public RecipeSlot[] inputs;
        public RecipeSlot[] outputs;
        public int processingTicks;
    }

    /// <summary>
    /// An input or output slot in a recipe.
    /// </summary>
    [Serializable]
    public class RecipeSlot
    {
        public string itemId;
        public int count;

        public RecipeSlot() { count = 1; }
        public RecipeSlot(string itemId, int count = 1)
        {
            this.itemId = itemId;
            this.count = count;
        }
    }

    /// <summary>
    /// Wrapper for JSON array of recipes.
    /// </summary>
    [Serializable]
    public class RecipeDefinitionList
    {
        public RecipeDefinition[] recipes;
    }
}
