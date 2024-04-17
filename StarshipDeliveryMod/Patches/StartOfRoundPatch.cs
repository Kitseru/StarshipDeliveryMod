using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using GameNetcodeStuff;
using HarmonyLib;
using UnityEngine;

namespace StarshipDeliveryMod.Patches
{
    [HarmonyPatch(typeof(StartOfRound))]
    public class StartOfRoundPatch
    {
        public static Action<Camera>? onCameraChange;
        public static Camera? currentCam;

/*         [HarmonyPatch("Awake")]
        [HarmonyPostfix]
        public static void AwakePatch(ref StartOfRound __instance)
        {
            __instance.gameObject.AddComponent<CustomRoundManager>();
        } */

        [HarmonyPatch("SwitchCamera")]
        [HarmonyPostfix]
        public static void SwitchCameraPatch(ref StartOfRound __instance, ref Camera ___activeCamera)
        {
            StarshipDelivery.mls.LogInfo("----------------------------------gameplay camera : " + ___activeCamera.name);
            currentCam = ___activeCamera;
            onCameraChange?.Invoke(___activeCamera);
            ___activeCamera.farClipPlane = 5000;
        }
    }
    
}