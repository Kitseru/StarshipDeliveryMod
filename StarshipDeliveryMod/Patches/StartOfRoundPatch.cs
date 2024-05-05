using System;
using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace StarshipDeliveryMod.Patches
{
    [HarmonyPatch(typeof(StartOfRound))]
    public class StartOfRoundPatch
    {
        public static Action<Camera>? onCameraChange;
        public static Action<bool>? onEnteringDungeon;
        public static Camera? CurrentCam {get; private set;}
        public static Dictionary<Camera, float> DefaultFarClipDict = [];
        private static List<string> scenesToExclude = new List<string> { "SampleSceneRelay", "ColdOpen1", "InitScene", "InitSceneLANMode", "InitSceneLaunchOptions", "MainMenu" };
        public static bool isALevelLoaded = false;
        public static bool isFarPlaneIncreased = false;

        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        public static void StartPatch(ref StartOfRound __instance)
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.sceneUnloaded += OnSceneUnloaded;
            CheckIfALevelIsLoaded();
            if(!isALevelLoaded)
            {
                IncreaseCameraFarClip(false);
            }
        }

        [HarmonyPatch("Update")]
        [HarmonyPostfix]
        public static void UpdatePatch(ref StartOfRound __instance)
        {
            if(!isALevelLoaded)
                return;

            if(CurrentCam?.transform.position.y < -100 && isFarPlaneIncreased)
            {
                IncreaseCameraFarClip(false);
                onEnteringDungeon?.Invoke(true);
            }
            else if(CurrentCam?.transform.position.y > -100 && !isFarPlaneIncreased)
            {
                IncreaseCameraFarClip(true);
                onEnteringDungeon?.Invoke(false);
            }
        }

        [HarmonyPatch("SwitchCamera")]
        [HarmonyPostfix]
        public static void SwitchCameraPatch(ref StartOfRound __instance, ref Camera ___activeCamera)
        {
            CurrentCam = ___activeCamera;
            onCameraChange?.Invoke(___activeCamera);
            
            if(!DefaultFarClipDict.ContainsKey(CurrentCam))
            {
                DefaultFarClipDict.Add(CurrentCam, CurrentCam.farClipPlane);
                StarshipDelivery.mls.LogInfo("Camera values stored in this entry : " + CurrentCam.name);
            }
        }

        public static void CheckIfALevelIsLoaded()
        {
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                Scene scene = SceneManager.GetSceneAt(i);

                if (!scenesToExclude.Contains(scene.name))
                {
                    isALevelLoaded = true;
                    return;
                }
            }

            isALevelLoaded = false;
        }

        public static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (!scenesToExclude.Contains(scene.name))
            {
                IncreaseCameraFarClip(true);
                isALevelLoaded = true;
            }
        }

        public static void OnSceneUnloaded(Scene scene)
        {
            if (!scenesToExclude.Contains(scene.name))
            {
                IncreaseCameraFarClip(false);
                isALevelLoaded = false;
            }
        }

        public static void IncreaseCameraFarClip(bool isIncreased)
        {
            if(CurrentCam == null)
                return;

            isFarPlaneIncreased = isIncreased;

            if(isIncreased)
            {
                CurrentCam.farClipPlane = 5000;
                StarshipDelivery.mls.LogInfo("Camera far plane increased to 5000");
            }
            else
            {
                if(!DefaultFarClipDict.ContainsKey(CurrentCam))
                {
                    CurrentCam.farClipPlane = 400;
                    StarshipDelivery.mls.LogInfo("Can't find entry in the Cam dictionnary, Camera far plane reset to default value : 400");
                    return;
                }

                CurrentCam.farClipPlane = DefaultFarClipDict[CurrentCam];
                StarshipDelivery.mls.LogInfo("Camera far plane reset to the original stored value : " + DefaultFarClipDict[CurrentCam]);
            }
        }
    }
}