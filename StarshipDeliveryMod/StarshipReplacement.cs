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
            GameObject newObject = Instantiate(StarshipDelivery.Ressources.LoadAsset<GameObject>("assets/prefabs/starshipmodel.prefab"), Vector3.zero, Quaternion.identity, _droneShip.gameObject.transform);
            newObject.name = "StarshipModel";
            newObject.transform.localPosition = new Vector3(0, 0, -1.4f);
            newObject.transform.localRotation = Quaternion.Euler(180, 90, 90);
            newObject.transform.localScale = new Vector3(1, 1, 1);

            StarshipDelivery.mls.LogError("New Object : " + newObject.name);

            //Change Animation Clips
            ReplaceStarshipAnimations(_droneShip);
            Animator shipAnimator = _droneShip.GetComponent<Animator>();
            RuntimeAnimatorController animatorController = shipAnimator.runtimeAnimatorController;

        }

        private static void ReplaceStarshipAnimations(GameObject _droneShip)
        {
            Animator shipAnimator = _droneShip.GetComponent<Animator>();
            AnimatorOverrideController animatorOverrideController = new AnimatorOverrideController(shipAnimator.runtimeAnimatorController);
            shipAnimator.runtimeAnimatorController = animatorOverrideController;

            animatorOverrideController["ItemShipIdleGone"] = StarshipDelivery.Ressources.LoadAsset<AnimationClip>("assets/animationclip/itemshipidlegone.anim");
            StarshipDelivery.mls.LogInfo("Clip Updated with : " + animatorOverrideController["ItemShipIdleGone"]);
            animatorOverrideController["ItemShipLand"] = StarshipDelivery.Ressources.LoadAsset<AnimationClip>("assets/animationclip/itemshipland.anim");
            StarshipDelivery.mls.LogInfo("Clip Updated with : " + animatorOverrideController["ItemShipLand"]);
            animatorOverrideController["ItemShipLeave"] = StarshipDelivery.Ressources.LoadAsset<AnimationClip>("assets/animationclip/itemshipleave.anim");
            StarshipDelivery.mls.LogInfo("Clip Updated with : " + animatorOverrideController["ItemShipLeave"]);
            animatorOverrideController["ItemShipOpenDoors"] = StarshipDelivery.Ressources.LoadAsset<AnimationClip>("assets/animationclip/itemshipopendoors.anim");
            StarshipDelivery.mls.LogInfo("Clip Updated with : " + animatorOverrideController["ItemShipOpenDoors"]);

            shipAnimator.Rebind();
        }
    }
}