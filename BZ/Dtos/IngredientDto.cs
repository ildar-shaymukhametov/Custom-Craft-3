using Nautilus.Handlers;
using System;
using System.Collections.Generic;

namespace BZModding.CustomCraft3.Dtos;

internal class IngredientDto
{
    public IngredientDto()
    {
    }

    public IngredientDto(string name, int amount)
    {
        Name = name;
        Amount = amount;
    }

    public string Name { get; set; }
    public int Amount { get; set; }

    public (Ingredient, List<string>) Validate()
    {
        var errors = new List<string>();
        if (!Enum.TryParse(Name, out TechType techType) && !EnumHandler.TryGetValue(Name, out techType))
        {
            errors.Add($"\"{Name}\" is not a valid ingredient name");
            return (null, errors);
        }

        return (new Ingredient(techType, Amount), errors);
    }
}
