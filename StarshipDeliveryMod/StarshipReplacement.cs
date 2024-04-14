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
            GameObject newShipObject = Instantiate(StarshipDelivery.Ressources.LoadAsset<GameObject>("assets/prefabs/starshipmodel.prefab"), Vector3.zero, Quaternion.identity, _droneShip.gameObject.transform);
            newShipObject.name = "StarshipModel";
            newShipObject.transform.localPosition = new Vector3(0, 0, -1.4f);
            newShipObject.transform.localRotation = Quaternion.Euler(180, 90, 90);
            newShipObject.transform.localScale = new Vector3(2, 2, 2);

            //Change Animation Clips
            ReplaceStarshipAnimations(_droneShip);
            Animator shipAnimator = _droneShip.GetComponent<Animator>();
            RuntimeAnimatorController animatorController = shipAnimator.runtimeAnimatorController;

            //Move Item Spawn Positions
            Transform[] spawnPositions = new Transform[_droneShip.transform.Find("ItemSpawnPositions").childCount];
            for (int i = 0; i < spawnPositions.Length; i++)
                spawnPositions[i] = _droneShip.transform.Find("ItemSpawnPositions").transform.GetChild(i);

            spawnPositions[0].localPosition = new Vector3(4.960f,1.185f,-0.141f);
            spawnPositions[1].localPosition = new Vector3(5.140f,0.509f,-0.141f);
            spawnPositions[2].localPosition = new Vector3(5.177f,-0.374f,-0.141f);
            spawnPositions[3].localPosition = new Vector3(5.058f,-1.069f,-0.141f);

            //Resize Triggers Colliders
            _droneShip.transform.Find("ItemShip/Trigger").GetComponent<BoxCollider>().size = new Vector3(3f, 3f, 1f);
            _droneShip.transform.Find("ItemShip/KillTrigger").GetComponent<BoxCollider>().size = new Vector3(2.8f, 2.8f, 1f);

            //Set shipBody "Collider" layer to work with Items Falling detection
            newShipObject.transform.Find("ShipBody").gameObject.layer = LayerMask.NameToLayer("Default");

        }

        private static void ReplaceStarshipAnimations(GameObject _droneShip)
        {
            Animator shipAnimator = _droneShip.GetComponent<Animator>();
            AnimatorOverrideController animatorOverrideController = new AnimatorOverrideController(shipAnimator.runtimeAnimatorController);
            shipAnimator.runtimeAnimatorController = animatorOverrideController;

            animatorOverrideController["ItemShipIdleGone"] = StarshipDelivery.Ressources.LoadAsset<AnimationClip>("assets/animationclip/itemshipidlegone.anim");
            animatorOverrideController["ItemShipLand"] = StarshipDelivery.Ressources.LoadAsset<AnimationClip>("assets/animationclip/itemshipland.anim");
            animatorOverrideController["ItemShipLeave"] = StarshipDelivery.Ressources.LoadAsset<AnimationClip>("assets/animationclip/itemshipleave.anim");
            animatorOverrideController["ItemShipOpenDoors"] = StarshipDelivery.Ressources.LoadAsset<AnimationClip>("assets/animationclip/itemshipopendoors.anim");

            shipAnimator.Rebind();
        }
    }
}