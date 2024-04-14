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
            StarshipReplacement.ReplaceStarshipModel(__instance.gameObject);

            Transform[] spawnPositions = __instance.transform.Find("ItemSpawnPositions").GetComponentsInChildren<Transform>();

            CreateDeliveryUI(__instance);
        }

        [HarmonyPatch("OpenShipDoorsOnServer")]
        [HarmonyPrefix]
        public static void OpenShipDoorsOnServerPatch(ref ItemDropship __instance, ref bool ___shipLanded, ref bool ___shipDoorsOpened, ref List<int> ___itemsToDeliver, ref Terminal ___terminalScript, ref StartOfRound ___playersManager, ref Transform[] ___itemSpawnPositions)
        {
            if (___shipLanded && !___shipDoorsOpened)
            {
                int num = 0;
                for (int i = 0; i < ___itemsToDeliver.Count; i++)
                {
                    GameObject obj = UnityEngine.Object.Instantiate(___terminalScript.buyableItemsList[___itemsToDeliver[i]].spawnPrefab, ___itemSpawnPositions[num].position, Quaternion.identity, ___playersManager.propsContainer);
                    GrabbableObject grabbableObject = obj.GetComponent<GrabbableObject>();
                    grabbableObject.fallTime = 1f;
                    grabbableObject.hasHitGround = true;
                    grabbableObject.reachedFloorTarget = true;
			        grabbableObject.targetFloorPosition = grabbableObject.transform.localPosition;
                    obj.GetComponent<NetworkObject>().Spawn();
                    num = ((num < 3) ? (num + 1) : 0);
                }
                ___itemsToDeliver.Clear();
                __instance.OpenShipClientRpc();
            }
        }

        static void CreateDeliveryUI(ItemDropship _itemDropship)
        {
            GameObject deliveryUI_GO = new GameObject("DeliveryUI");
            deliveryUI_GO.hideFlags = HideFlags.HideAndDontSave;
            DeliveryUI deliveryUI = deliveryUI_GO.AddComponent<DeliveryUI>();
            deliveryUI.itemDropship = _itemDropship;
            StarshipDelivery.mls.LogInfo("DeliveryUI Instance created : " + deliveryUI_GO.name);
        }
    }
}


//le timer commence à 20 si c'est le premier achat. Lorsque la partie commence le timer monte jursqu'à 40 puis reviens à 0.
//Le timer reprend et va jusqu'a 30 (c'est le temps que reste le droneship sur terre)
//Des que le timer a atteint 30, il s'arrete et le droneship repart dans l'espace.
//A partir de la chaque nouvel achat ne met que 10 seconde a arriver au lieu de 20 car le timer va de 30 à 40 au lieu de 20 à 40 comme en debut de partie