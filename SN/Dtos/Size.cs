using System.Collections.Generic;
using System;
using Nautilus.Handlers;

namespace SNModding.Nautilus.Dtos;

internal class Size
{
    public string Name { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }

    public (TechType, Vector2int?, List<string>) Validate()
    {
        var errors = new List<string>();
        TechType techType;
        if (!Enum.TryParse(Name, out techType) && !EnumHandler.TryGetValue(Name, out techType))
        {
            errors.Add($"\"{Name}\" is not a valid item name");
            return (default, default, errors);
        }

        return (techType, new Vector2int(Width, Height), errors);
    }
}
