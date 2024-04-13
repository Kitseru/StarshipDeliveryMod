using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
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
            StarshipReplacement.ReplaceStarshipModel(__instance.gameObject);
            StarshipDelivery.CreateDeliveryUI(__instance);
        }
    }
}


//le timer commence à 20 si c'est le premier achat. Lorsque la partie commence le timer monte jursqu'à 40 puis reviens à 0.
//Le timer reprend et va jusqu'a 30 (c'est le temps que reste le droneship sur terre)
//Des que le timer a atteint 30, il s'arrete et le droneship repart dans l'espace.
//A partir de la chaque nouvel achat ne met que 10 seconde a arriver au lieu de 20 car le timer va de 30 à 40 au lieu de 20 à 40 comme en debut de partie