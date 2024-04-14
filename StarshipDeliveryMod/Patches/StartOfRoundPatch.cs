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
    internal class StartOfRoundPatch
    {

        [HarmonyPatch("SwitchCamera")]
        [HarmonyPostfix]
        public static void SwitchCameraPatch(ref StartOfRound __instance, ref Camera ___activeCamera)
        {
            StarshipDelivery.mls.LogInfo("----------------------------------gameplay camera : " + ___activeCamera.name);
            ___activeCamera.farClipPlane = 2000;
        }
/* 
        static void ChangeCameraFarPlane()
        {
            StarshipDelivery.mls.LogInfo("----------------------------------gameplay camera : " + Camera.current);
            Camera.current.farClipPlane = 2000;
        } */

        /* [HarmonyPatch("Start")]
        [HarmonyPostfix]
        public static void StartPatch(ref StartOfRound __instance)
        {
            __instance.CameraSwitchEvent.AddListener(ChangeCameraFarPlane);
        }

        static void ChangeCameraFarPlane()
        {
            StarshipDelivery.mls.LogInfo("----------------------------------gameplay camera : " + Camera.current);
            Camera.current.farClipPlane = 2000;
        } */
    }
}