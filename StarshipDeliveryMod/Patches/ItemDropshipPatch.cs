using System;
using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;

namespace StarshipDeliveryMod.Patches
{
    [HarmonyPatch(typeof(ItemDropship))]
    internal class ItemDropshipPatch
    { 

        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        public static void StartPatch(ref ItemDropship __instance)
        {
            //Change ship position to avoid penetrations with environment
            LevelData_Unity currentLevelDatas = LevelDataManager.GetLevelDatas(__instance.gameObject.scene.name);
            __instance.transform.parent.transform.localPosition = currentLevelDatas.landingPosition;
            __instance.transform.parent.transform.localRotation = Quaternion.Euler(currentLevelDatas.landingRotation);
            StarshipDelivery.mls.LogInfo("current level : " + currentLevelDatas.levelName + " ------------> changing ship position and rotation to fit Starship size at : " + currentLevelDatas.landingPosition + " - " + currentLevelDatas.landingRotation);

            StarshipReplacement.ReplaceStarshipModel(__instance.gameObject);
        }
    }
}