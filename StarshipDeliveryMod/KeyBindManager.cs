using System;
using System.Collections.Generic;
using HarmonyLib;
using StarshipDeliveryMod;
using UnityEngine;
using UnityEngine.InputSystem;

namespace StarshipDeliveryMod{   
    
delegate void SimpleMethodDelegate();
internal class KeyBindManager
{
    public static InputActionMap? customActionMap;
    public static void BindKeyToMethod(SimpleMethodDelegate _method, string _inputName, string _key)
    {
        if(customActionMap == null)
        {
            customActionMap = new InputActionMap("CustomStarshipDeliveryMod");
        }

        InputAction existingAction = customActionMap.FindAction(_inputName);
        if (existingAction != null)
        {
            Debug.LogWarning($"InputAction with the name {_inputName} already exists.");
            return;
        }
        customActionMap.Disable();

        InputAction newAction = customActionMap.AddAction(_inputName);
        newAction.AddBinding("<Keyboard>/" + _key);
        customActionMap.Enable();
        newAction.performed += ctx => _method();

        StarshipDelivery.mls.LogInfo($"New Keybind created, {_inputName} bind to {_key}");
    }

    public static InputAction CreateInputAction(string _inputName, string _binding, string _interactions)
    {
        if(customActionMap == null)
        {
            customActionMap = new InputActionMap("CustomStarshipDeliveryMod");
        }

        InputAction existingAction = customActionMap.FindAction(_inputName);
        if (existingAction != null)
        {
            Debug.LogWarning($"InputAction with the name {_inputName} already exists.");
            return existingAction;
        }


        customActionMap.Disable();

        InputAction newInputAction = customActionMap.AddAction(_inputName);
        newInputAction.AddBinding(_binding).WithInteraction(_interactions);
        customActionMap.Enable();

        return newInputAction;
    }
}


}
