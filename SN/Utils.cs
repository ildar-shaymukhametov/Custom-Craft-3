using Nautilus.Crafting;
using Nautilus.Handlers;
using SNModding.CustomCraft3.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SNModding.CustomCraft3
{
    internal static class Utils
    {
        public static (RecipeData, List<string>) CreateRecipeData(int craftAmount, Ingredient[] ingredients, string[] linkedItems)
        {
            var errors = new List<string>();
            var data = new RecipeData
            {
                craftAmount = craftAmount,
                Ingredients = ingredients
                    .Select(x =>
                    {
                        var result = x.Validate();
                        if (result.Item1 == null)
                        {
                            errors.Add($"\"{x.Name}\" is not a valid ingredient name");
                        }

                        return result;
                    })
                    .Where(x => x.Item1 != null)
                    .Select(x => x.Item1)
                    .ToList(),
                LinkedItems = linkedItems
                    .Select(x =>
                    {
                        if (!Enum.TryParse(x, out TechType techType))
                        {
                            errors.Add($"\"{x}\" is not a valid linked item name");
                            return (Ok: false, default);
                        }

                        return (Ok: true, LinkedItem: techType);
                    })
                    .Where(x => x.Ok)
                    .Select(x => x.LinkedItem)
                    .ToList()
            };

            return (data, errors);
        }

        public static Dictionary<CraftTree.Type, string[][]> GetFabricatorPaths()
        {
            return new Dictionary<CraftTree.Type, string[][]>
            {
                [CraftTree.Type.Constructor] = new[]
                {
                    CraftTreeHandler.Paths.ConstructorRocket,
                    CraftTreeHandler.Paths.ConstructorVehicles
                },
                [CraftTree.Type.Fabricator] = new[]
                {
                    CraftTreeHandler.Paths.FabricatorsBasicMaterials,
                    CraftTreeHandler.Paths.FabricatorsAdvancedMaterials,
                    CraftTreeHandler.Paths.FabricatorsElectronics,
                    CraftTreeHandler.Paths.FabricatorWater,
                    CraftTreeHandler.Paths.FabricatorCookedFood,
                    CraftTreeHandler.Paths.FabricatorCuredFood,
                    CraftTreeHandler.Paths.FabricatorEquipment,
                    CraftTreeHandler.Paths.FabricatorTools,
                    CraftTreeHandler.Paths.FabricatorMachines
                },
                [CraftTree.Type.SeamothUpgrades] = new[]
                {
                    CraftTreeHandler.Paths.VehicleUpgradesCommonModules,
                    CraftTreeHandler.Paths.VehicleUpgradesExosuitModules,
                    CraftTreeHandler.Paths.VehicleUpgradesSeamothModules,
                    CraftTreeHandler.Paths.VehicleUpgradesTorpedoes
                },
                [CraftTree.Type.Workbench] = new string[0][],
                [CraftTree.Type.Centrifuge] = new string[0][],
                [CraftTree.Type.CyclopsFabricator] = new string[0][],
                [CraftTree.Type.MapRoom] = new string[0][],
                [CraftTree.Type.Rocket] = new string[0][]
            };
        }
    }
}
