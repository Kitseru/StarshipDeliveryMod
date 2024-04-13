using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StarshipDeliveryMod
{
    internal class DeliveryGUI : MonoBehaviour
    {
        internal static DeliveryGUI? Instance;
        internal float org_shipTimer = 0;
        internal int normalizedTimer = 0;

        void Awake()
        {
            if(Instance == null)
            {
                Instance = this;
            }
        }

        void Update()
        {
            normalizedTimer = (int)Math.Floor(Math.Abs(org_shipTimer - 40));
        }

        void OnGUI()
        {
            GUI.Label(new Rect(100, 20, 100, 50), normalizedTimer.ToString());
            //StarshipDelivery.mls.LogInfo(deliveryTimer.ToString());
        }
    }
}