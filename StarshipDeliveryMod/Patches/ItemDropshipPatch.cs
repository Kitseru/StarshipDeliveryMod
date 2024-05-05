using System;
using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;
using StarshipDeliveryMod;

namespace StarshipDeliveryMod.Patches
{
    [HarmonyPatch(typeof(ItemDropship))]
    internal class ItemDropshipPatch
    { 

        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        public static void StartPatch(ref ItemDropship __instance)
        {
            if(StarshipDelivery.AutoReplace)
            {
                StarshipDelivery.InitStarshipReplacement(__instance);            
            }
        }
    }
}