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
        [HarmonyPatch("Update")]
        [HarmonyPostfix]
        public static void UpdateDeliveryGUI(ref float ___shipTimer, ref Animator ___shipAnimator)
        {
            StarshipDelivery.Instance.dGUI.org_shipTimer = ___shipTimer;

            /* if(___shipTimer > 5f)
            {
                ___shipTimer = 5f;
            } */
            //StarshipDelivery.mls.LogInfo("GameObject Name = " + ___shipAnimator.gameObject.name.ToString());
            //StarshipDelivery.mls.LogInfo("Mesh Filter Name = " + ___shipAnimator.gameObject.GetComponent<MeshFilter>().ToString());
            //StarshipDelivery.mls.LogInfo("Mesh Name = " + ___shipAnimator.gameObject.GetComponent<MeshFilter>().mesh.ToString());
            //StarshipDelivery.mls.LogInfo("Collider Type = " + ___shipAnimator.gameObject.GetComponent<Collider>().GetType().ToString());
            //StarshipDelivery.mls.LogInfo("Texture Name = " + ___shipAnimator.gameObject.GetComponent<Renderer>().material.mainTexture.name.ToString());
            //le timer commence à 20 si c'est le premier achat. Lorsque la partie commence le timer monte jursqu'à 40 puis reviens à 0.
            //Le timer reprend et va jusqu'a 30 (c'est le temps que reste le droneship sur terre)
            //Des que le timer a atteint 30, il s'arrete et le droneship repart dans l'espace.
            //A partir de la chaque nouvel achat ne met que 10 seconde a arriver au lieu de 20 car le timer va de 30 à 40 au lieu de 20 à 40 comme en debut de partie
        }

        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        public static void SpawnStarshipModel(ref ItemDropship __instance)
        {

            if (StarshipDelivery.Ressources == null)
            {
                StarshipDelivery.mls?.LogError(" Ressources = null");
                return;
            }

            // Hide Original DroneShip
            Renderer[] childRenderers = __instance.transform.GetChild(0).GetComponentsInChildren<Renderer>();
            foreach(Renderer childRenderer in childRenderers)
            {
                childRenderer.enabled = false;
            }

            // Add Starship Prefab
            //GameObject ressourcePrefab = StarshipDelivery.Ressources.LoadAsset<GameObject>("assets/models/starshipmodel_game.blend");
            GameObject ressourcePrefab = StarshipDelivery.Ressources.LoadAsset<GameObject>("assets/prefabs/starshipmodel.prefab");
            GameObject newObject = UnityEngine.Object.Instantiate<GameObject>(ressourcePrefab, Vector3.zero, Quaternion.identity, __instance.gameObject.transform);
            newObject.transform.localPosition = new Vector3(0, 0, -1.4f);
            newObject.transform.localRotation = Quaternion.Euler(148, 90, 90);
            newObject.transform.localScale = new Vector3(1, 1, 1);

            StarshipDelivery.mls?.LogError("New Object : " + newObject.name);
        }
    }

    [HarmonyPatch(typeof(MenuManager))]
    internal class MenuManagerPatch
    {
        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        public static void SpawnShip()
        {
            StarshipDelivery.mls?.LogInfo("Spawn Ship");
            GameObject ressourcePrefab = StarshipDelivery.Ressources.LoadAsset<GameObject>("assets/prefabs/starshipmodel.prefab");
            ressourcePrefab.transform.position = new Vector3(1090.5f, 552.3f, -81f);
            ressourcePrefab.transform.rotation = Quaternion.Euler(0, 0, 0);
            ressourcePrefab.transform.localScale = new Vector3(6, 6, 6);
            StarshipDelivery.mls?.LogInfo("Ship : " + ressourcePrefab.ToString());
        }
    }
}


//le timer commence à 20 si c'est le premier achat. Lorsque la partie commence le timer monte jursqu'à 40 puis reviens à 0.
//Le timer reprend et va jusqu'a 30 (c'est le temps que reste le droneship sur terre)
//Des que le timer a atteint 30, il s'arrete et le droneship repart dans l'espace.
//A partir de la chaque nouvel achat ne met que 10 seconde a arriver au lieu de 20 car le timer va de 30 à 40 au lieu de 20 à 40 comme en debut de partie