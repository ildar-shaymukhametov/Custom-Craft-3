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
        }

        if (errors.Any())
        {
            return (default, default, errors);
        }

        return (techType, new RecipeData
        {
            craftAmount = CraftAmount,
            Ingredients = Ingredients
                .Select(x =>
                {
                    var validationResult = x.Validate();
                    if (validationResult.Item1 == null)
                    {
                        errors.Add($"\"{x.Name}\" is an invalid ingredient name");
                    }

                    return validationResult;
                })
                .Where(x => x.Item1 != null)
                .Select(x => x.Item1)
                .ToList(),
            LinkedItems = LinkedItems
                .Select(x =>
                {
                    if (!Enum.TryParse(x, out TechType techType))
                    {
                        errors.Add($"\"{x}\" is an invalid linked item name");
                        return (Ok: false, default);
                    }

                    return (Ok: true, LinkedItem: techType);
                })
                .Where(x => x.Ok)
                .Select(x => x.LinkedItem)
                .ToList()
        }, errors);
    }
}
