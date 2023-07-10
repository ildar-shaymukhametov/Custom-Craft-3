using Nautilus.Handlers;
using System;
using System.Collections.Generic;

namespace BZModding.CustomCraft3.Dtos;

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

    public (global::Ingredient, List<string>) Validate()
    {
        var errors = new List<string>();
        if (!Enum.TryParse(Name, out TechType techType) && !EnumHandler.TryGetValue(Name, out techType))
        {
            errors.Add($"\"{Name}\" is not a valid ingredient name");
            return (null, errors);
        }

        return (new global::Ingredient(techType, Amount), errors);
    }
}
