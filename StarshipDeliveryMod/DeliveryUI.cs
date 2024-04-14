using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

namespace StarshipDeliveryMod
{
    internal class DeliveryUI : MonoBehaviour
    {
        internal static DeliveryUI Instance = null!;
        internal ItemDropship itemDropship = null!;
        internal int normalizedTimer = 0;

        void Awake()
        {
            if(Instance == null)
            {
                Instance = this;
            }
        }

        void Start()
        {
            //var playerInput = GameObject.Find("PlayerSettingsObject").GetComponent<PlayerInput>();
            
            var customActionMap = new InputActionMap("CustomStarshipDeliveryMod");
            var speedUpTimerAction = customActionMap.AddAction("speedUpTimer");
            speedUpTimerAction.AddBinding("<Keyboard>/n");

            customActionMap.Enable();

            speedUpTimerAction.performed += ctx => SpeedUpTimer();
        }

        void Update()
        {
            normalizedTimer = (int)Math.Floor(Math.Abs(itemDropship.shipTimer - 40));
        }

        void OnGUI()
        {
            GUI.Label(new Rect(100, 20, 100, 50), normalizedTimer.ToString());
        }

        void SpeedUpTimer()
        {
            if(itemDropship == null)
            {
                StarshipDelivery.mls.LogError("No ItemDroneship instance");
                return;
            }
            
            itemDropship.shipTimer += 5f;
            StarshipDelivery.mls.LogInfo("Delivery timer advanced by 5 seconds");
        }
    }
}