using System;
using System.Collections.Generic;
using System.Linq;

namespace SNModding.Nautilus.Dtos
{
    internal class NewRecipe
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool UnlockAtStart { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public string Icon { get; set; }
        public string Model { get; set; }
        public List<Ingredient> Ingredients { get; set; }
        public List<string> LinkedItems { get; set; }
        public int CraftAmount { get; set; }

        public (ItemData, List<string>) Validate()
        {
            var errors = new List<string>();
            if (!Enum.TryParse(Model, out TechType model))
            {
                errors.Add($"\"{Model}\" is an invalid model name. This recipe will be skipped");
                return (null, errors);
            }

            var icon = SpriteManager.defaultSprite;
            if (Enum.TryParse(Icon, out TechType iconTechType))
            {
                icon = SpriteManager.Get(iconTechType);
            }

            var recipeResult = Utils.CreateRecipeData(CraftAmount, Ingredients, LinkedItems);
            if (recipeResult.Item2.Any())
            {
                errors.AddRange(recipeResult.Item2);
            }

            var item = new ItemData
            {
                Name = Name,
                Description = Description,
                UnlockAtStart = UnlockAtStart,
                Size = new Vector2int(Width, Height),
                Icon = icon,
                Model = model,
                RecipeData = recipeResult.Item1
            };

            return (item, errors);
        }
    }
}
