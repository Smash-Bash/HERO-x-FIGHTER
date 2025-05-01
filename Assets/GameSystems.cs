using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class GameSystems
{
    public static bool eventMode;
    public static Stage stageToLoad;
    public static Stage lastLoadedStage;
    public static List<Fighter> fighters = new List<Fighter>();
    public static List<int> inputIDs = new List<int>();

    public static void LoadScene(Stage stage)
    {
        Time.timeScale = 1;
        stageToLoad = stage;
        SceneManager.LoadScene("LoadingScreen");
    }
}
