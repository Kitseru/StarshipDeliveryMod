using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StarshipDeliveryMod
{
    internal class StarshipReplacement : MonoBehaviour
    {
        public static void ReplaceStarshipModel(GameObject _droneShip)
        {
            if (StarshipDelivery.Ressources == null)
            {
                StarshipDelivery.mls.LogError(" Ressources = null");
                return;
            }

            // Hide Original DroneShip
            Renderer[] childRenderers = _droneShip.transform.GetChild(0).GetComponentsInChildren<Renderer>();
            foreach(Renderer childRenderer in childRenderers)
            {
                childRenderer.enabled = false;
            }

            // Add Starship Prefab
            GameObject ressourcePrefab = StarshipDelivery.Ressources.LoadAsset<GameObject>("assets/prefabs/starshipmodel.prefab");
            GameObject newObject = UnityEngine.Object.Instantiate<GameObject>(ressourcePrefab, Vector3.zero, Quaternion.identity, _droneShip.gameObject.transform);
            newObject.transform.localPosition = new Vector3(0, 0, -1.4f);
            newObject.transform.localRotation = Quaternion.Euler(148, 90, 90);
            newObject.transform.localScale = new Vector3(1, 1, 1);

            StarshipDelivery.mls.LogError("New Object : " + newObject.name);
        }
    }
}