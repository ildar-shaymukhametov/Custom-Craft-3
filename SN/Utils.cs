using Nautilus.Crafting;
using SNModding.Nautilus.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SNModding.Nautilus
{
    internal static class Utils
    {
        public static (RecipeData, List<string>) CreateRecipeData(int craftAmount, Ingredient[] ingredients, string[] linkedItems)
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
                            errors.Add($"\"{x.Name}\" is not a valid ingredient name");
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
                            errors.Add($"\"{x}\" is not a valid linked item name");
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
