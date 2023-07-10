using System.Collections.Generic;
using System;
using Nautilus.Handlers;

namespace SNModding.CustomCraft3.Dtos;

internal class Size
{
    public string Name { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }

    public (TechType, Vector2int?, List<string>) Validate()
    {
        var errors = new List<string>();
        if (!Enum.TryParse(Name, out TechType techType) && !EnumHandler.TryGetValue(Name, out techType))
        {
            errors.Add($"\"{Name}\" is not a valid item name");
            return (default, default, errors);
        }

        return (techType, new Vector2int(Width, Height), errors);
    }
}
