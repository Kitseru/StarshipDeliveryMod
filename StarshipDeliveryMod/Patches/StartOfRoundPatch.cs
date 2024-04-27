using System;
using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;

namespace StarshipDeliveryMod.Patches
{
    [HarmonyPatch(typeof(StartOfRound))]
    public class StartOfRoundPatch
    {
        public static Action<Camera>? onCameraChange;
        public static Camera? CurrentCam {get; private set;}

        [HarmonyPatch("SwitchCamera")]
        [HarmonyPostfix]
        public static void SwitchCameraPatch(ref StartOfRound __instance, ref Camera ___activeCamera)
        {
            CurrentCam = ___activeCamera;
            onCameraChange?.Invoke(___activeCamera);
            ___activeCamera.farClipPlane = 5000;
        }
    }
}