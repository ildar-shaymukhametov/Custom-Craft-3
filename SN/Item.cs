using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Assets.PrefabTemplates;
using Nautilus.Crafting;

namespace SNModding.Nautilus
{
    internal class Item
    {
        public static PrefabInfo Info { get; private set; }

        public static void Register(ItemData data)
        {
            Info = PrefabInfo.WithTechType(data.Name, data.Name, data.Description, unlockAtStart: data.UnlockAtStart)
                .WithSizeInInventory(data.Size)
                .WithIcon(data.Icon);

            var prefab = new CustomPrefab(Info);
            prefab.SetGameObject(new CloneTemplate(Info, data.Model));
            prefab.SetRecipe(data.RecipeData)
                .WithFabricatorType(data.FabricatorType)
                .WithStepsToFabricatorTab(data.FabricatorPath)
                .WithCraftingTime(data.CraftTimeSeconds);

            if (data.TechGroup.HasValue && data.TechCategory.HasValue)
            {
                prefab.SetPdaGroupCategory(data.TechGroup.Value, data.TechCategory.Value);
            }

            prefab.Register();
        }
    }

    internal class ItemData
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool UnlockAtStart { get; set; }
        public Atlas.Sprite Icon { get; set; }
        public TechType Model { get; set; }
        public Vector2int Size { get; set; }
        public RecipeData RecipeData { get; set; }
        public CraftTree.Type FabricatorType { get; set; }
        public string[] FabricatorPath { get; set; }
        public float CraftTimeSeconds { get; set; }
        public TechCategory? TechCategory { get; set; }
        public TechGroup? TechGroup { get; set; }
    }
}
