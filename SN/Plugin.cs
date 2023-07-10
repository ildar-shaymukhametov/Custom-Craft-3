using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using Nautilus.Handlers;
using Nautilus.Json.ExtensionMethods;
using SNModding.CustomCraft3.Dtos;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace SNModding.CustomCraft3;

[BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
[BepInDependency("com.snmodding.nautilus")]
public class Plugin : BaseUnityPlugin
{
    public new static ManualLogSource Logger { get; private set; }

    private static Assembly Assembly { get; } = Assembly.GetExecutingAssembly();

    private void Awake()
    {
        // set project-scoped logger instance
        Logger = base.Logger;

        LoadNewRecipes();
        LoadModifiedRecipes();
        LoadCustomSizes();
        GenerateSampleRecipes();
        GenerateSampleSizes();
        GenerateOthersReferences();
        GenerateTechTypeReference();

        // register harmony patches, if there are any
        Harmony.CreateAndPatchAll(Assembly, $"{PluginInfo.PLUGIN_GUID}");
        Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
    }

    private static void LoadNewRecipes()
    {
        var path = Path.Combine(Paths.PluginPath, Assembly.GetExecutingAssembly().GetName().Name, "WorkingFiles", "NewRecipes.json");
        var list = new List<NewRecipe>();
        list.LoadJson(path);

        foreach (var item in list)
        {
            var (itemData, errors) = item.Validate();
            if (itemData != null)
            {
                Item.Register(itemData);
            }

            if (errors.Any())
            {
                Logger.LogWarning($"New recipe ({item.Name}): {string.Join("; ", errors)}");
            }
        }
    }

    private static void LoadCustomSizes()
    {
        var path = Path.Combine(Paths.PluginPath, Assembly.GetExecutingAssembly().GetName().Name, "WorkingFiles", "CustomSizes.json");
        var list = new List<Size>();
        list.LoadJson(path);

        foreach (var item in list)
        {
            var (techType, size, errors) = item.Validate();
            if (size.HasValue)
            {
                CraftDataHandler.SetItemSize(techType, size.Value);
            }

            if (errors.Any())
            {
                Logger.LogWarning($"Custom size: {string.Join("; ", errors)}");
            }
        }
    }

    private static void LoadModifiedRecipes()
    {
        var path = Path.Combine(Paths.PluginPath, Assembly.GetExecutingAssembly().GetName().Name, "WorkingFiles", "ModifiedRecipes.json");
        var list = new List<Recipe>();
        list.LoadJson(path);

        foreach (var item in list)
        {
            var (techType, recipeData, errors) = item.Validate();
            if (recipeData != null)
            {
                CraftDataHandler.SetRecipeData(techType, recipeData);
            }

            if (errors.Any())
            {
                Logger.LogWarning($"Modified recipe ({item.Name}): {string.Join("; ", errors)}");
            }
        }
    }

    private static void GenerateSampleRecipes()
    {
        var type = typeof(TechType);
        var list = Enum.GetValues(type)
            .Cast<TechType>()
            .Where(x => type.GetField(x.ToString())?.GetCustomAttribute<ObsoleteAttribute>() is null)
            .Select(x => (TechType: x, RecipeData: CraftDataHandler.GetRecipeData(x)))
            .Where(x => x.RecipeData is not null && x.RecipeData.Ingredients.Any())
            .Select(x => new Recipe(x.TechType, x.RecipeData))
            .ToList();

        var path = Path.Combine(Paths.PluginPath, Assembly.GetExecutingAssembly().GetName().Name, "SampleFiles", "ModifiedRecipes.json");
        list.SaveJson(path);
    }

    private static void GenerateTechTypeReference()
    {
        var type = typeof(TechType);
        var list = Enum.GetValues(type)
            .Cast<TechType>()
            .Where(x => type.GetField(x.ToString())?.GetCustomAttribute<ObsoleteAttribute>() is null)
            .Select(x => x.ToString())
            .ToList();

        var path = Path.Combine(Paths.PluginPath, Assembly.GetExecutingAssembly().GetName().Name, "SampleFiles", "TechTypeReference.json");
        list.SaveJson(path);
    }

    private static void GenerateOthersReferences()
    {
        var result = new
        {
            FabricatorTypesAndPaths = Utils.GetFabricatorPaths(),
            TechGroups = Enum.GetNames(typeof(TechGroup)),
            TechCategories = Enum.GetNames(typeof(TechCategory))
        };

        var path = Path.Combine(Paths.PluginPath, Assembly.GetExecutingAssembly().GetName().Name, "SampleFiles", "OtherReferences.json");
        result.SaveJson(path);
    }

    private static void GenerateSampleSizes()
    {
        var list = new List<Size>
        {
            new Size { Height = 2, Width = 2, Name = TechType.Copper.ToString() },
            new Size { Height = 3, Width = 2, Name = TechType.PowerCell.ToString() }
        };

        var path = Path.Combine(Paths.PluginPath, Assembly.GetExecutingAssembly().GetName().Name, "SampleFiles", "CustomSizes.json");
        list.SaveJson(path);
    }
}