using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using StarshipDeliveryMod;

public class LevelDataManager
{
    public static Dictionary<string, LevelData_Unity> LevelDataDict = [];

    public static void InitializeLevelDatas(string _textJSON)
    {
        LevelDataList? levelDataList = JsonConvert.DeserializeObject<LevelDataList>(_textJSON);

        if(levelDataList == null)
        {
            StarshipDelivery.mls.LogInfo("Failed to read convert JsonFile to a class");
            return;
        }

        foreach(LevelData levelData in levelDataList.levelDatas)
        {
            Vector3 pos = new Vector3(levelData.landingPosition.x, levelData.landingPosition.y, levelData.landingPosition.z);
            Vector3 rot = new Vector3(levelData.landingRotation.x, levelData.landingRotation.y, levelData.landingRotation.z);

            LevelData_Unity levelData_Unity = new LevelData_Unity(levelData.levelName, pos, rot);

            LevelDataDict.Add(levelData_Unity.levelName, levelData_Unity);
        }
    }

    public static LevelData_Unity GetLevelDatas(string _levelName)
    {
        return LevelDataDict[_levelName];
    }

    public class LandingPosition
    {
        public float x { get; set; }
        public float y { get; set; }
        public float z { get; set; }
    }

    public class LandingRotation
    {
        public float x { get; set; }
        public float y { get; set; }
        public float z { get; set; }
    }

    public class LevelData
    {
        public string? levelName { get; set; }
        public LandingPosition? landingPosition { get; set; }
        public LandingRotation? landingRotation { get; set; }
    }

    public class LevelDataList
    {
        public List<LevelData>? levelDatas { get; set; }
    }
}

//Class that uses Vector3 instead of x y and z position as individual floats
public class LevelData_Unity
{
    public string levelName;
    public Vector3 landingPosition;
    public Vector3 landingRotation;

    public LevelData_Unity(string _levelName, Vector3 _landingPosition, Vector3 _landingRotation)
    {
        levelName = _levelName;
        landingPosition = _landingPosition;
        landingRotation = _landingRotation;
    }
}
