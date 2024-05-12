using System;
using System.Collections.Generic;
using UnityEngine;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using StarshipDeliveryMod.Patches;
using System.IO;
using System.Reflection;
using BepInEx.Configuration;

namespace StarshipDeliveryMod;

[BepInPlugin(modGUID, modName, modVersion)]
public class StarshipDelivery : BaseUnityPlugin
{
    private const string modGUID = "Laventin.StarshipDeliveryMod";
    private const string modName = "StarshipDelivery";
    private const string modVersion = "1.0.3";

    private readonly Harmony harmony = new(modGUID);

    internal static StarshipDelivery Instance = null!;

    internal static ManualLogSource mls = null!;

    public static AssetBundle Ressources = null!;

    public static string LevelDataConfig = null!;
    public static string LevelDataConfigPath = null!;

    public static bool AutoReplace = true;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        mls = BepInEx.Logging.Logger.CreateLogSource(modGUID);
        mls.LogInfo(">>> Starship Delivery Mod loaded");

        string currentAssemblyLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        Ressources = AssetBundle.LoadFromFile(Path.Combine(currentAssemblyLocation, "starshipdelivery"));
        
        ConfigSettings.BindConfigSettings();

        if (Ressources == null) {
            mls.LogError(">>> Failed to load custom assets.");
            return;
        }

        LevelDataConfigPath = Path.Combine(currentAssemblyLocation, "ShipPositionConfig.json");

        try
        {
            LevelDataConfig = File.ReadAllText(LevelDataConfigPath);
        }
        catch
        {
            mls.LogError(">>> Failed to load ShipPositionConfig.json");
            return;
        }

        LevelDataManager.InitializeLevelDatas(LevelDataConfig);

        harmony.PatchAll(typeof(ItemDropshipPatch));
        harmony.PatchAll(typeof(StartOfRoundPatch));
        harmony.PatchAll(typeof(PlayerControllerBPatch));

        mls = Logger;
    }

    public static void InitStarshipReplacement(ItemDropship _itemDropShip)
    {
        //Change ship position to avoid penetrations with environment
        LevelData_Unity currentLevelDatas = LevelDataManager.GetLevelDatas(_itemDropShip.gameObject.scene.name);
        if(currentLevelDatas != null)
        {
            _itemDropShip.transform.parent.transform.position = currentLevelDatas.landingPosition;
            _itemDropShip.transform.parent.transform.rotation = Quaternion.Euler(currentLevelDatas.landingRotation);
            mls.LogInfo(">>> current level : " + currentLevelDatas.levelName + " -> changing ship position and rotation to fit Starship size at : " + currentLevelDatas.landingPosition + " - " + currentLevelDatas.landingRotation);
        }
        else
        {
            mls.LogInfo(">>> ShipPositionConfig.json don't contain datas for this level, default ship position will be used");
        }
        
        StarshipReplacement.ReplaceStarshipModel(_itemDropShip.gameObject);
    }
}

public static class ConfigSettings
{
    public static ConfigEntry<bool>? enableMusicEffects;
    public static ConfigEntry<bool>? enableSonicBoom;
    public static ConfigEntry<float>? sfxVolume;
    public static ConfigEntry<float>? starshipSize;
    public static ConfigEntry<bool>? customPositioningTool;

    public static void BindConfigSettings()
    {
        enableMusicEffects = ((BaseUnityPlugin)StarshipDelivery.Instance).Config.Bind<bool>("StarshipDeliveryMod Config", "Enable Music Effects", true, "Enable Effects on the Dropship Music such as Pitch Shift, Reverb, Distortion, etc.");
        enableSonicBoom = ((BaseUnityPlugin)StarshipDelivery.Instance).Config.Bind<bool>("StarshipDeliveryMod Config", "Enable Sonic Boom Sound", true, "Enable the sonic boom sound when the Starship is entering the atmosphere");
        sfxVolume = ((BaseUnityPlugin)StarshipDelivery.Instance).Config.Bind<float>("StarshipDeliveryMod Config", "SFX Volume", 100, "(Range : 0 - 150 %) Change the volume of landing and liftoff SFX");
        sfxVolume.Value = Mathf.Clamp(sfxVolume.Value, 0, 150);
        starshipSize = ((BaseUnityPlugin)StarshipDelivery.Instance).Config.Bind<float>("StarshipDeliveryMod Config", "Starship Size", 100, "(Range : 50 - 150 %) Change the size of the Starship");
        starshipSize.Value = Mathf.Clamp(starshipSize.Value, 50, 150);
        customPositioningTool = ((BaseUnityPlugin)StarshipDelivery.Instance).Config.Bind<bool>("StarshipDeliveryMod Debug", "Enable Custom Positioning Tool", false, "A built-in tool that can let you preview and change starship postion and rotation on custom moons, and allow to overwrite ShipPositionConfig.json file to save your changes. To open it in game, press [P]");
    }
}