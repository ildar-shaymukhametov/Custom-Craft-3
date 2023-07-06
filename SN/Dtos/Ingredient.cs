using System;
using System.Collections.Generic;

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
            return (null, errors);
        }

        return (new CraftData.Ingredient(techType, Amount), errors);
    }
}
