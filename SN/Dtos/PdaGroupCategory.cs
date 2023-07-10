using System;
using System.Collections.Generic;

namespace SNModding.CustomCraft3.Dtos
{
    internal class PdaGroupCategory
    {
        public string TechCategory { get; set; }
        public string TechGroup { get; set; }

        public (TechGroup?, TechCategory?, List<string>) Validate()
        {
            var errors = new List<string>();
            if (!Enum.TryParse(TechCategory, out TechCategory category))
            {
                errors.Add($"\"{TechCategory}\" is not a valid tech category");
                return (default, default, errors);
            }

            if (!Enum.TryParse(TechGroup, out TechGroup group))
            {
                errors.Add($"\"{TechGroup}\" is not a valid tech group");
                return (default, default, errors);
            }

            return (group, category, errors);
        }
    }
}
