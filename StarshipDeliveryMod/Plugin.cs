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

        private readonly Harmony harmony = new Harmony(modGUID);

        internal static StarshipDelivery Instance = new StarshipDelivery();

        internal static ManualLogSource? mls;
        internal DeliveryGUI dGUI = new DeliveryGUI();

        public static AssetBundle? Ressources;


        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }

            mls = BepInEx.Logging.Logger.CreateLogSource(modGUID);
            mls.LogInfo("Starhip Delivery Mod has awaken OMG !!!!!!!!!!!");

            string currentAssemblyLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            Ressources = AssetBundle.LoadFromFile(Path.Combine(currentAssemblyLocation, "Ressources/starshipdelivery_assetbundle"));
            if (Ressources == null) {
                mls.LogError("Failed to load custom assets.");
                return;
            }
            else
            {
                mls.LogInfo("Asset Bundle : " + Ressources.ToString() + " Loaded Successifully");
            }

            //harmony.PatchAll(typeof(StarshipDeliveryModBase));
            harmony.PatchAll(typeof(ItemDropshipPatch));
            harmony.PatchAll(typeof(MenuManagerPatch));

            mls = Logger;

            var deliveryGUI_GO = new UnityEngine.GameObject("DeliveryGUI");
            UnityEngine.Object.DontDestroyOnLoad(deliveryGUI_GO);
            deliveryGUI_GO.hideFlags = HideFlags.HideAndDontSave;
            deliveryGUI_GO.AddComponent<DeliveryGUI>();
            dGUI = (DeliveryGUI)deliveryGUI_GO.GetComponent<DeliveryGUI>();
            mls.LogInfo("deliveryGUI Instance created : " + deliveryGUI_GO.name);
        }
    }

}