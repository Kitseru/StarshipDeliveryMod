using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        private const string modVersion = "0.0.1";

        private readonly Harmony harmony = new(modGUID);

        internal static StarshipDelivery Instance = null!;

        internal static ManualLogSource mls = null!;
        internal DeliveryUI deliveryUI = null!;

        public static AssetBundle Ressources = null!;


        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }

            mls = BepInEx.Logging.Logger.CreateLogSource(modGUID);
            mls.LogInfo("Starhip Delivery Mod loaded");

            string currentAssemblyLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            Ressources = AssetBundle.LoadFromFile(Path.Combine(currentAssemblyLocation, "Ressources/starshipdelivery_assetbundle"));
            if (Ressources == null) {
                mls.LogError("Failed to load custom assets.");
                return;
            }

            harmony.PatchAll(typeof(ItemDropshipPatch));

            mls = Logger;
        }

        public static void CreateDeliveryUI(ItemDropship _itemDropship)
        {
            var deliveryUI_GO = new GameObject("DeliveryUI");
            deliveryUI_GO.hideFlags = HideFlags.HideAndDontSave;
            DeliveryUI deliveryUI = deliveryUI_GO.AddComponent<DeliveryUI>();
            deliveryUI.itemDropship = _itemDropship;
            mls.LogInfo("DeliveryUI Instance created : " + deliveryUI_GO.name);
        }
    }

}