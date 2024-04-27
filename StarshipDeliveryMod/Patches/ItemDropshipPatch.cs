using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;

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
            Transform[] spawnPositions = __instance.transform.Find("ItemSpawnPositions").GetComponentsInChildren<Transform>();
        }
    }
}