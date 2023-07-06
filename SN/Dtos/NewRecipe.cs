using Nautilus.Crafting;
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
            }

            if (errors.Any())
            {
                return (null, errors);
            }

            var icon = SpriteManager.defaultSprite;
            if (Enum.TryParse(Icon, out TechType iconTechType))
            {
                icon = SpriteManager.Get(iconTechType);
            }

            var ingredientsResult = Ingredient.Validate(Ingredients);
            if (ingredientsResult.Item2.Any())
            {
                errors.AddRange(ingredientsResult.Item2);
            }

            return (new ItemData
            {
                Name = Name,
                Description = Description,
                UnlockAtStart = UnlockAtStart,
                Size = new Vector2int(Width, Height),
                Icon = icon,
                Model = model,
                RecipeData = new RecipeData
                {
                    craftAmount = CraftAmount,
                    Ingredients = ingredientsResult.Item1,
                    LinkedItems = LinkedItems
                        .Select(x =>
                        {
                            if (!Enum.TryParse(x, out TechType techType))
                            {
                                errors.Add($"\"{x}\" is an invalid linked item name");
                                return (Ok: false, default);
                            }

                            return (Ok: true, LinkedItem: techType);
                        })
                        .Where(x => x.Ok)
                        .Select(x => x.LinkedItem)
                        .ToList()
                }
            }, errors);
        }
    }
}
