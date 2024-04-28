using System;
using System.Collections.Generic;
using UnityEngine;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using StarshipDeliveryMod.Patches;
using System.IO;
using System.Reflection;

namespace StarshipDeliveryMod
{
    [BepInPlugin(modGUID, modName, modVersion)]
    public class StarshipDelivery : BaseUnityPlugin
    {
        private const string modGUID = "Laventin.StarshipDeliveryMod";
        private const string modName = "StarshipDelivery";
        private const string modVersion = "1.0.2";

        private readonly Harmony harmony = new(modGUID);

        internal static StarshipDelivery Instance = null!;

        internal static ManualLogSource mls = null!;

        public static AssetBundle Ressources = null!;

        public static string LevelDataConfig = null!;

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
    }
}