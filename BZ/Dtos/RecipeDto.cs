using Nautilus.Crafting;
using Nautilus.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BZModding.CustomCraft3.Dtos;

internal class RecipeDto
{
    public RecipeDto()
    {
    }

    public RecipeDto(TechType techType, RecipeData recipeData)
    {
        Name = techType.ToString();
        CraftAmount = recipeData.craftAmount;
        Ingredients = recipeData.Ingredients.Select(x => new IngredientDto(x.techType.ToString(), x.amount)).ToArray();
        LinkedItems = recipeData.LinkedItems.Select(x => x.ToString()).ToArray();
    }

    public string Name { get; set; }
    public int CraftAmount { get; set; }
    public IngredientDto[] Ingredients { get; set; }
    public string[] LinkedItems { get; set; }

    public (TechType, RecipeData, List<string>) Validate()
    {
        var errors = new List<string>();
        if (!Enum.TryParse(Name, out TechType techType) && !EnumHandler.TryGetValue(Name, out techType))
        {
            errors.Add($"\"{Name}\" is not a valid recipe name");
            return (default, default, errors);
        }

        var recipeResult = Utils.ValidateRecipeData(CraftAmount, Ingredients, LinkedItems);
        if (recipeResult.Item2.Any())
        {
            errors.AddRange(recipeResult.Item2);
        }

        return (techType, recipeResult.Item1, errors);
    }
}
