using System.Collections.Generic;
using System;

namespace SNModding.Nautilus.Dtos;

internal class Size
{
    public string Name { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }

    public (TechType, Vector2int?, List<string>) Validate()
    {
        var errors = new List<string>();
        if (!Enum.TryParse(Name, out TechType techType))
        {
            errors.Add($"\"{Name}\" is an invalid item name");
            return (default, default, errors);
        }

        return (techType, new Vector2int(Width, Height), errors);
    }
}
