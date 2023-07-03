﻿using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using Nautilus.Handlers;
using System.Linq;
using Nautilus.Json.ExtensionMethods;
using System;
using System.IO;
using System.Collections.Generic;
using Nautilus.Crafting;

namespace BZModding.Nautilus;

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

        // register harmony patches, if there are any
        Harmony.CreateAndPatchAll(Assembly, $"{PluginInfo.PLUGIN_GUID}");
        Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
    }

    private static void LoadCustomSizes()
    {
        var path = Path.Combine(Paths.PluginPath, Assembly.GetExecutingAssembly().GetName().Name, "WorkingFiles", "CustomSizes.json");
        var list = new List<Size>();
        list.LoadJson(path);

        foreach (var item in list)
        {
            if (Enum.TryParse(item.Name, out TechType techType))
            {
                CraftDataHandler.SetItemSize(techType, item.Width, item.Height);
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
            var techType = (TechType)Enum.Parse(typeof(TechType), item.Name);
            CraftDataHandler.SetRecipeData(techType, new RecipeData
            {
                craftAmount = item.CraftAmount,
                Ingredients = item.Ingredients.Select(x => new global::Ingredient((TechType)Enum.Parse(typeof(TechType), x.Name), x.Amount)).ToList(),
                LinkedItems = item.LinkedItems.Select(x => (TechType)Enum.Parse(typeof(TechType), x)).ToList()
            });
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
}