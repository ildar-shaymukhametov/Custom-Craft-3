using Nautilus.Crafting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SNModding.Nautilus.Dtos;

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

    public (TechType, RecipeData, List<string>) Validate()
    {
        var errors = new List<string>();
        if (!Enum.TryParse(Name, out TechType techType))
        {
            errors.Add($"\"{Name}\" is an invalid recipe name");
            return (default, default, errors);
        }

        var recipeResult = Utils.CreateRecipeData(CraftAmount, Ingredients, LinkedItems);
        if (recipeResult.Item2.Any())
        {
            errors.AddRange(recipeResult.Item2);
        }

        return (techType, recipeResult.Item1, errors);
    }
}
