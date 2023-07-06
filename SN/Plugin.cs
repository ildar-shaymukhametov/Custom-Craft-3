using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using Nautilus.Handlers;
using Nautilus.Json.ExtensionMethods;
using SNModding.Nautilus.Dtos;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace SNModding.Nautilus;

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

        GenerateSampleRecipes();
        LoadModifiedRecipes();
        LoadCustomSizes();
        GenerateTechTypeReference();
        LoadNewRecipes();

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
            var result = item.Validate();
            if (result.Item1 != null)
            {
                Item.Register(result.Item1);
            }

            if (result.Item2.Any())
            {
                Logger.LogWarning($"New recipe: {string.Join(", ", result.Item2)}");
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
            var result = item.Validate();
            if (result.Item2.HasValue)
            {
                CraftDataHandler.SetItemSize(result.Item1, result.Item2.Value);
            }

            if (result.Item3.Any())
            {
                Logger.LogWarning($"Custom size: {string.Join(", ", result.Item3)}");
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
            var result = item.Validate();
            if (result.Item2 != null)
            {
                CraftDataHandler.SetRecipeData(result.Item1, result.Item2);
            }

            if (result.Item3.Any())
            {
                Logger.LogWarning($"Modified recipe ({item.Name}): {string.Join(", ", result.Item3)}");
            }
        }
    }

    private static void GenerateSampleRecipes()
    {
        var type = typeof(TechType);
        var list = Enum.GetValues(type)
            .Cast<TechType>()
            .Where(x => type.GetField(x.ToString()).GetCustomAttribute<ObsoleteAttribute>() is null)
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
            .Where(x => type.GetField(x.ToString()).GetCustomAttribute<ObsoleteAttribute>() is null)
            .Select(x => x.ToString())
            .ToList();

        var path = Path.Combine(Paths.PluginPath, Assembly.GetExecutingAssembly().GetName().Name, "SampleFiles", "TechTypeReference.json");
        list.SaveJson(path);
    }
}