using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

        void Update()
        {
            normalizedTimer = (int)Math.Floor(Math.Abs(itemDropship.shipTimer - 40));
        }

        void OnGUI()
        {
            GUI.Label(new Rect(100, 20, 100, 50), normalizedTimer.ToString());
        }
    }
}