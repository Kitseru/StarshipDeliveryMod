using System;
using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;
using UnityEngine.SceneManagement;
using GameNetcodeStuff;
using Unity.Netcode;

namespace StarshipDeliveryMod.Patches;

[HarmonyPatch(typeof(PlayerControllerB))]
internal class PlayerControllerBPatch
{
    [HarmonyPatch("ConnectClientToPlayerObject")]
    [HarmonyPostfix]
    public static void ConnectClientToPlayerObjectPatch(ref PlayerControllerB __instance)
    {
        if(ConfigSettings.customPositioningTool?.Value == true && __instance.IsOwner)
        {
            __instance.gameObject.AddComponent<CustomPositioningTool>();
        }
    }
}