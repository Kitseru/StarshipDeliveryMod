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

namespace StarshipDeliveryMod
{
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

        public static bool AutoReplace = true;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }

            mls = BepInEx.Logging.Logger.CreateLogSource(modGUID);
            mls.LogInfo("Starship Delivery Mod loaded");

            string currentAssemblyLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            Ressources = AssetBundle.LoadFromFile(Path.Combine(currentAssemblyLocation, "starshipdelivery"));
            
            ConfigSettings.BindConfigSettings();

            if (Ressources == null) {
                mls.LogError("Failed to load custom assets.");
                return;
            }

            try
            {
                LevelDataConfig = File.ReadAllText(Path.Combine(currentAssemblyLocation, "ShipPositionConfig.json"));
            }
            catch
            {
                mls.LogError("Failed to load ShipPositionConfig.json");
                return;
            }

            LevelDataManager.InitializeLevelDatas(LevelDataConfig);

            harmony.PatchAll(typeof(ItemDropshipPatch));
            harmony.PatchAll(typeof(StartOfRoundPatch));

            mls = Logger;
        }

        public static void InitStarshipReplacement(ItemDropship _itemDropShip)
        {
            //Change ship position to avoid penetrations with environment
            LevelData_Unity currentLevelDatas = LevelDataManager.GetLevelDatas(_itemDropShip.gameObject.scene.name);
            if(currentLevelDatas != null)
            {
                _itemDropShip.transform.parent.transform.localPosition = currentLevelDatas.landingPosition;
                _itemDropShip.transform.parent.transform.localRotation = Quaternion.Euler(currentLevelDatas.landingRotation);
                mls.LogInfo("current level : " + currentLevelDatas.levelName + " -> changing ship position and rotation to fit Starship size at : " + currentLevelDatas.landingPosition + " - " + currentLevelDatas.landingRotation);
            }
            else
            {
                mls.LogInfo("ShipPositionConfig.json don't contain datas for this level, default ship position will be used");
            }
            
            StarshipReplacement.ReplaceStarshipModel(_itemDropShip.gameObject);
        }
    }

    public static class ConfigSettings
    {
        public static ConfigEntry<bool>? enableMusicEffects;
        public static ConfigEntry<bool>? enableSonicBoom;

        public static void BindConfigSettings()
        {
            enableMusicEffects = ((BaseUnityPlugin)StarshipDelivery.Instance).Config.Bind<bool>("StarshipDeliveryMod Config", "Enable Music Effects", true, "Enable Effects on the Dropship Music such as Pitch Shift, Reverb, Distortion, etc.");
            enableSonicBoom = ((BaseUnityPlugin)StarshipDelivery.Instance).Config.Bind<bool>("StarshipDeliveryMod Config", "Enable Sonic Boom Sound", true, "Enable the sonic boom sound when the Starship is entering the atmosphere");
        }
    }
}