using Nautilus.Crafting;
using System.Collections.Generic;
using System.Linq;

namespace SNModding.Nautilus;

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
    }

    public string Name { get; set; }
    public int CraftAmount { get; set; }
    public List<Ingredient> Ingredients { get; set; }
}
