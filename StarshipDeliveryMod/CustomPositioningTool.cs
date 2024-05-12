using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using GameNetcodeStuff;
using Newtonsoft.Json;
using StarshipDeliveryMod;
using StarshipDeliveryMod.Patches;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace StarshipDeliveryMod;
internal class CustomPositioningTool : MonoBehaviour
{
    private bool isToolActive = false;
    private bool isShipPlaced = false;
    private bool isTweakMenuOpen = false;
    private bool isPopupOpen = false;
    private LayerMask layerMask;
    private GameObject? dummyShipPrefab;
    private GameObject? dummyShip = null;
    private Camera? cam = null;
    private PlayerControllerB? playerController;
    private GameObject? canevasPrefab;
    private Transform? canvas;
    private Transform? tweakMenu;
    private Transform? tipsTexts;
    private Transform? choicePopup;
    private Transform? confirmPopup;
    private List<Transform> allPopups = new();

    private Text? lockText;
    private Text? rotateText;
    private Text? tweakOpenText;
    private Text? processText;
    private Text? confirmInfosText;

    private InputField? posXField;
    private InputField? posYField;
    private InputField? posZField;

    private InputField? rotXField;
    private InputField? rotYField;
    private InputField? rotZField;
    private static List<string> scenesToExclude = new List<string> { "SampleSceneRelay", "ColdOpen1", "InitScene", "InitSceneLANMode", "InitSceneLaunchOptions", "MainMenu" };
    public LevelDataList? storedLevelDataList = null;

    public InputAction? activateToolAction;
    public InputAction? exitPopupAction;
    public InputAction? lockDummyAction;
    public InputAction? unlockDummyAction;
    public InputAction? rotateAction;
    public InputAction? openTweakMenuAction;
    public InputAction? processAction;

    #pragma warning disable CS8602 // Dereference of a possibly null reference.
    
    void Start()
    {
        //Input Init
        activateToolAction = KeyBindManager.CreateInputAction("ActivateCustomPositionTool", "<Keyboard>/p", "press");
        exitPopupAction = KeyBindManager.CreateInputAction("ExitPopup", "<Keyboard>/escape", "press");
        lockDummyAction = KeyBindManager.CreateInputAction("LockDummy", "<Mouse>/leftButton", "press");
        unlockDummyAction = KeyBindManager.CreateInputAction("UnlockDummy", "<Keyboard>/l", "press");
        rotateAction = KeyBindManager.CreateInputAction("RotateDummy", "<Keyboard>/o", "hold");
        openTweakMenuAction = KeyBindManager.CreateInputAction("OpenTweakMenu", "<Keyboard>/i", "press");
        processAction = KeyBindManager.CreateInputAction("ProcessDummy", "<Keyboard>/k", "press");

        //References Init
        dummyShipPrefab = StarshipDelivery.Ressources.LoadAsset<GameObject>("assets/prefabs/dummyshiptransform.prefab");
        canevasPrefab = StarshipDelivery.Ressources.LoadAsset<GameObject>("assets/prefabs/custompositioningtoolcanvas.prefab");
        cam = StartOfRoundPatch.CurrentCam;
        playerController = StartOfRound.Instance.localPlayerController;

        //Add Default and Room LayerMask
        layerMask |= (1 << 0);
        layerMask |= (1 << 8);

        StarshipDelivery.mls.LogInfo(">>> Custom Positioning Tool initialized");
    }

    void Update()
    {
        if (activateToolAction.triggered)
        {
            ActivateTool();
        }

        if (!isToolActive || cam == null || dummyShip == null) return;

        if(isPopupOpen)
        {
            if(exitPopupAction.triggered)
            {
                ExitPopup();
            }
            return;
        }

        if(!isShipPlaced && !isTweakMenuOpen)
        {
            RaycastHit hit;
            Ray ray = new Ray(cam.transform.position, cam.transform.forward);
            
            if (Physics.Raycast(ray, out hit, 100f, layerMask))
            {
                dummyShip.transform.position = hit.point;
                dummyShip.transform.up = hit.normal;
            }
        }

        if(lockDummyAction.triggered)
        {
            isShipPlaced = true;
            UpdateUI();
        }

        if(unlockDummyAction.triggered && !isTweakMenuOpen)
        {
            isShipPlaced = false;
            UpdateUI();
        }

        if(rotateAction.IsPressed() && isShipPlaced && !isTweakMenuOpen)
        {
            dummyShip.transform.Rotate(Vector3.up * 100 * Time.deltaTime);
        }

        if(openTweakMenuAction.triggered && isShipPlaced)
        {
            OpenTweakMenu();
            UpdateUI();
        }

        if(processAction.triggered && isShipPlaced && !isTweakMenuOpen)
        {
            isPopupOpen = !isPopupOpen;
            OpenPopup(choicePopup.gameObject, isPopupOpen);
        }
    }

    private void OpenTweakMenu()
    {
        isTweakMenuOpen = !isTweakMenuOpen;

        if(isTweakMenuOpen)
        {
            InitTweakValues();
            LockPlayerCamera(true);
        }
        else
        {
            LockPlayerCamera(false);
        }
    }

    void ActivateTool()
    {
        if(dummyShipPrefab == null || canevasPrefab == null) return;
        isToolActive = !isToolActive;

        if(isToolActive)
        {
            StarshipDelivery.mls.LogInfo(">>> Custom Positioning Tool on");
            dummyShip = Instantiate(dummyShipPrefab, Vector3.zero, Quaternion.identity);
            canvas = Instantiate(canevasPrefab, Vector3.zero, Quaternion.identity).transform;
            canvas.gameObject.AddComponent<CustomPositioningToolEvents>();
            InitUI();
            UpdateUI();
            canvas.GetComponent<CustomPositioningToolEvents>().onFieldUpdate += UpdateTweakValues;
            canvas.GetComponent<CustomPositioningToolEvents>().onCopyToClipboard += CopyJsonToClipboard;
            canvas.GetComponent<CustomPositioningToolEvents>().onOverwriteFile += CheckForDuplicatesLevelNames;
            canvas.GetComponent<CustomPositioningToolEvents>().onCancel += ExitPopup;
            canvas.GetComponent<CustomPositioningToolEvents>().onConfirm += SaveToFile;
        }
        else
        {
            StarshipDelivery.mls.LogInfo(">>> Custom Positioning Tool off");
            isShipPlaced = false;
            isTweakMenuOpen = false;
            isPopupOpen = false;
            LockPlayerCamera(false);
            allPopups.Clear();
            UpdateUI();
            if(canvas != null)
            {
                canvas.GetComponent<CustomPositioningToolEvents>().onFieldUpdate -= UpdateTweakValues;
                canvas.GetComponent<CustomPositioningToolEvents>().onCopyToClipboard -= CopyJsonToClipboard;
                canvas.GetComponent<CustomPositioningToolEvents>().onOverwriteFile -= CheckForDuplicatesLevelNames;
                canvas.GetComponent<CustomPositioningToolEvents>().onCancel -= ExitPopup;
                canvas.GetComponent<CustomPositioningToolEvents>().onConfirm -= SaveToFile;
            }
            if(dummyShip != null)
                Destroy(dummyShip);
            if(canvas != null)
                Destroy(canvas.gameObject);
        }
    }

    private void InitUI()
    {
        if(canvas == null) return;
        
        tweakMenu = canvas.Find("TweakMenu");
        tipsTexts = canvas.Find("TipsTexts");
        choicePopup = canvas.Find("ChoicePopup");
        confirmPopup = canvas.Find("ConfirmPopup");

        tweakMenu.gameObject.SetActive(true);
        tipsTexts.gameObject.SetActive(true);
        choicePopup.gameObject.SetActive(true);
        confirmPopup.gameObject.SetActive(true);

        allPopups.Add(choicePopup);
        allPopups.Add(confirmPopup);

        lockText = tipsTexts.Find("Lock/Text").GetComponent<Text>();
        rotateText = tipsTexts.Find("Rotate/Text").GetComponent<Text>();
        tweakOpenText = tipsTexts.Find("TweakOpen/Text").GetComponent<Text>();
        processText = tipsTexts.Find("Process/Text").GetComponent<Text>();
        confirmInfosText = confirmPopup.Find("ConfirmText").GetComponent<Text>();

        posXField = tweakMenu.Find("Pos_Layout/PosX_Layout/PosX_InputField").GetComponent<InputField>();
        posYField = tweakMenu.Find("Pos_Layout/PosY_Layout/PosY_InputField").GetComponent<InputField>();
        posZField = tweakMenu.Find("Pos_Layout/PosZ_Layout/PosZ_InputField").GetComponent<InputField>();

        rotXField = tweakMenu.Find("Rot_Layout/RotX_Layout/RotX_InputField").GetComponent<InputField>();
        rotYField = tweakMenu.Find("Rot_Layout/RotY_Layout/RotY_InputField").GetComponent<InputField>();
        rotZField = tweakMenu.Find("Rot_Layout/RotZ_Layout/RotZ_InputField").GetComponent<InputField>();
        
        tweakMenu.gameObject.SetActive(false);
        choicePopup.gameObject.SetActive(false);
        confirmPopup.gameObject.SetActive(false);
    }

    void UpdateUI()
    {
        if(!isTweakMenuOpen)
        {
            tipsTexts.gameObject.SetActive(true);
            tweakMenu.gameObject.SetActive(false);

            if(isShipPlaced)
            {
                lockText.text = "[L] to Unlock";
                rotateText.text = "[O] to Rotate";
                tweakOpenText.text = "[ I ] to open Tweak Menu";
                processText.text = "[K] to copy Json snippet to clipboard";
            }
            else
            {
                lockText.text = "[RMB] to Lock in place";
                rotateText.text = "-";
                tweakOpenText.text = "-";
                processText.text = "-";
            }
        }
        else
        {
            tipsTexts.gameObject.SetActive(false);
            tweakMenu.gameObject.SetActive(true);
        }
    }

    void InitTweakValues()
    {
        posXField.text = dummyShip.transform.position.x.ToString();
        posYField.text = dummyShip.transform.position.y.ToString();
        posZField.text = dummyShip.transform.position.z.ToString();

        rotXField.text = dummyShip.transform.eulerAngles.x.ToString();
        rotYField.text = dummyShip.transform.eulerAngles.y.ToString();
        rotZField.text = dummyShip.transform.eulerAngles.z.ToString();

        Debug.Log(dummyShip.transform.eulerAngles.x);
        Debug.Log(dummyShip.transform.eulerAngles.y);
        Debug.Log(dummyShip.transform.eulerAngles.z);
    }

    void UpdateTweakValues()
    {
        dummyShip.transform.position = new Vector3(GetCorrectFloat(posXField.text), GetCorrectFloat(posYField.text), GetCorrectFloat(posZField.text));
        dummyShip.transform.rotation = Quaternion.Euler(GetCorrectFloat(rotXField.text), GetCorrectFloat(rotYField.text), GetCorrectFloat(rotZField.text));
    }

    private void CopyJsonToClipboard()
    {
        LevelData newLevelData = GenerateLevelDataForTheDummy();
        string jsonString = JsonConvert.SerializeObject(newLevelData, Formatting.Indented);
        GUIUtility.systemCopyBuffer = jsonString;
        StarshipDelivery.mls.LogInfo(">>> Json Snippet copied to Clipboard");
        ExitPopup();
    }

    private string? GetCurrentSceneName()
    {
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene scene = SceneManager.GetSceneAt(i);

            if (!scenesToExclude.Contains(scene.name))
            {
                return scene.name;
            }
        }

        return null;
    }
    
    private LevelData GenerateLevelDataForTheDummy()
    {   
        string? currentSceneName = GetCurrentSceneName();

        if(currentSceneName == null)
        {
            throw new InvalidOperationException(">>> Scene Name is not Valid");
        }

        Vector3 currentShipPosition = dummyShip.transform.Find("DummyShip").transform.position;
        Vector3 currentShipRotation = dummyShip.transform.Find("DummyShip").transform.eulerAngles;

        Debug.Log(currentShipPosition);

        LevelData currentLevelData = new()
        {
            levelName = currentSceneName,
            landingPosition = new()
            {
                x = currentShipPosition.x,
                y = currentShipPosition.y,
                z = currentShipPosition.z
            },
            landingRotation = new()
            {
                x = currentShipRotation.x,
                y = currentShipRotation.y,
                z = currentShipRotation.z
            }
        };

        return currentLevelData;
    }

    void CheckForDuplicatesLevelNames()
    {
        storedLevelDataList = LevelDataManager.GetStoredLevelDataList(StarshipDelivery.LevelDataConfig);

        if(storedLevelDataList == null || storedLevelDataList.levelDatas == null) return;

        LevelData newLevelData = GenerateLevelDataForTheDummy();

        foreach(LevelData levelData in storedLevelDataList.levelDatas)
        {
            if(levelData.levelName == newLevelData.levelName)
            {
                OpenPopup(confirmPopup.gameObject, true);
                string newConfirmText = confirmInfosText.text.Replace("[]", levelData.levelName);
                confirmInfosText.text = newConfirmText;
                return;
            }
        }

        SaveToFile();
    }

    string GenerateJsonFile()
    {
        LevelData newLevelData = GenerateLevelDataForTheDummy();
                
        LevelData? levelDataToRemove = null;
        foreach(LevelData levelData in storedLevelDataList.levelDatas)
        {
            if(levelData.levelName == newLevelData.levelName)
            {
                levelDataToRemove = levelData;
            }
        }
        
        if(levelDataToRemove != null)
        {
            storedLevelDataList.levelDatas.Remove(levelDataToRemove);
        }

        storedLevelDataList.levelDatas.Add(newLevelData);

        string jsonString = JsonConvert.SerializeObject(storedLevelDataList, Formatting.Indented);
        return jsonString;
    }

    void SaveToFile()
    {
        string _jsonString = GenerateJsonFile();
        System.IO.File.WriteAllText(StarshipDelivery.LevelDataConfigPath, _jsonString);
        StarshipDelivery.mls.LogInfo(">>> ShipPositionConfig.json overwrited successifully, reopen the game to see changes !");
        ExitPopup();
    }

    void OpenPopup(GameObject _popup, bool _open)
    {
        if(_popup == null) return;

        isPopupOpen = _open;
        _popup.SetActive(_open);
        LockPlayerCamera(_open);
    }
    private void ExitPopup()
    {
        foreach(Transform popup in allPopups)
        {
            popup.gameObject.SetActive(false);
        }

        isPopupOpen = false;
        LockPlayerCamera(false);
    }

    void LockPlayerCamera(bool _lock)
    {
        if(playerController == null) return;

        if(_lock)
        {
            playerController.disableLookInput = true;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            playerController.disableLookInput = false;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    private float GetCorrectFloat(string _text)
    {
        float result;
        if (float.TryParse(_text, out result))
        {
            return result;
        }
        else
        {
            return 0.0f;
        }
    }

    #pragma warning restore CS8602 // Dereference of a possibly null reference.
}