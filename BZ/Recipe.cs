using Nautilus.Crafting;
using System.Collections.Generic;
using System.Linq;

namespace BZModding.Nautilus;

internal class Recipe
{
    public Recipe()
    {
    }

    public Recipe(TechType techType, RecipeData recipeData)
    {
        Name = techType.ToString();
        CraftAmount = recipeData.craftAmount;
        Ingredients = recipeData.Ingredients.Select(x => new Ingredient(x.techType.ToString(), x.amount)).ToList();
        LinkedItems = recipeData.LinkedItems.Select(x => x.ToString()).ToList();
    }

    public string Name { get; set; }
    public int CraftAmount { get; set; }
    public List<Ingredient> Ingredients { get; set; }
    public List<string> LinkedItems { get; set; }
}
