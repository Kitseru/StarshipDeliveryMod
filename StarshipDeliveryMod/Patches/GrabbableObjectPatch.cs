using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using GameNetcodeStuff;
using UnityEngine;

namespace StarshipDeliveryMod.Patches
{
    [HarmonyPatch(typeof(GrabbableObject))]
    internal class GrabbableObjectPatch
    {

        [HarmonyPatch("FallToGround")]
        [HarmonyPrefix]
        public static void FallToGroundPatch(ref GrabbableObject __instance, ref float ___fallTime, ref Vector3 ___targetFloorPosition, ref Item ___itemProperties, bool randomizePosition = false)
        {
            int originalLayerMask = 268437760; // LayerMask compilé
            LayerMask newLayerMask = originalLayerMask | (1 << 0); //Ajout du LayerMask Default

            StarshipDelivery.mls.LogInfo("----------------------------------new Layer created");

            ___fallTime = 0f;

            if (Physics.Raycast(__instance.transform.position, Vector3.down, out var hitInfo, 80f, newLayerMask, QueryTriggerInteraction.Ignore))
            {
                Debug.Log("HitInfo detected: " + LayerMask.LayerToName(hitInfo.collider.gameObject.layer));

                ___targetFloorPosition = hitInfo.point + ___itemProperties.verticalOffset * Vector3.up;
                if (__instance.transform.parent != null)
                {
                    ___targetFloorPosition = __instance.transform.parent.InverseTransformPoint(___targetFloorPosition);
                }
            }
            else
            {
                Debug.Log("dropping item did not get raycast : " + __instance.gameObject.name);
                ___targetFloorPosition = __instance.transform.localPosition;
            }
            if (randomizePosition)
            {
                ___targetFloorPosition += new Vector3(UnityEngine.Random.Range(-0.5f, 0.5f), 0f, UnityEngine.Random.Range(-0.5f, 0.5f));
            }

            StarshipDelivery.mls.LogInfo("----------------------------------everything executed correctly");
        }
    }
}