using Nautilus.Crafting;
using SNModding.Nautilus.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SNModding.Nautilus
{
    internal static class Utils
    {
        public static (RecipeData, List<string>) CreateRecipeData(int craftAmount, List<Ingredient> ingredients, List<string> linkedItems)
        {
            var errors = new List<string>();
            var data = new RecipeData
            {
                craftAmount = craftAmount,
                Ingredients = ingredients
                    .Select(x =>
                    {
                        var result = x.Validate();
                        if (result.Item1 == null)
                        {
                            errors.Add($"\"{x.Name}\" is an invalid ingredient name");
                        }

                        return result;
                    })
                    .Where(x => x.Item1 != null)
                    .Select(x => x.Item1)
                    .ToList(),
                LinkedItems = linkedItems
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
            };

            return (data, errors);
        }
    }
}
