﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class CrossSceneController 
{
    public static string previousScene;
    public static string recordingToLoad = "";
    public static int recordingToLoadID;
    public static AudioClip clipToLoad;

    public static bool isCampaign;
    public static int currentCampaignLevel = 1; //1-3
    public static string campaignDifficulty = "";

    public static float mainThemeTime; //Used by UIs to continue the main theme where the previous menu left off

    public static bool isOnline; //If true, online features (leaderboards, workshop, etc) enabled. If false they are disabled; Likely due to choosing "continue offline" instead of logging in

    public static void SceneToGame(string recordingPath, AudioClip clip, int id) //Triggered from other scene, sends current track to game
    {
        previousScene = SceneManager.GetActiveScene().name;
        recordingToLoad = recordingPath;
        if (recordingToLoad.Substring(recordingToLoad.Length - 4) != ".xml")
            recordingToLoad += ".xml";
        clipToLoad = clip;
        recordingToLoadID = id;
    }

    public static void SceneToGame(string recordingPath, AudioClip clip) //Triggered from other scene, sends current track to game
    {
        previousScene = SceneManager.GetActiveScene().name;
        recordingToLoad = recordingPath;
        if (recordingToLoad.Substring(recordingToLoad.Length - 4) != ".xml")
            recordingToLoad += ".xml";
        clipToLoad = clip;
    }

    public static void GameToMaker(string recordingName) //Triggered from game, sends current track to maker
    {
        recordingToLoad = recordingName;
        if (recordingToLoad.Substring(recordingToLoad.Length - 4) != ".xml")
            recordingToLoad += ".xml";
    }
}
