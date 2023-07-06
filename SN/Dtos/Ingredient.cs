using System;
using System.Collections.Generic;
using System.Linq;

namespace SNModding.Nautilus.Dtos;

internal class Ingredient
{
    public Ingredient()
    {
    }

    public Ingredient(string name, int amount)
    {
        Name = name;
        Amount = amount;
    }

    public string Name { get; set; }
    public int Amount { get; set; }

    public (CraftData.Ingredient, List<string>) Validate()
    {
        var errors = new List<string>();
        if (!Enum.TryParse(Name, out TechType techType))
        {
            errors.Add($"\"{Name}\" is an invalid ingredient name");
        }

        if (errors.Any())
        {
            return (null, errors);
        }

        return (new CraftData.Ingredient(techType, Amount), errors);
    }

    public static (List<CraftData.Ingredient>, List<string>) Validate(List<Ingredient> ingredients)
    {
        var errors = new List<string>();
        return (ingredients
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
        errors);
    }
}
