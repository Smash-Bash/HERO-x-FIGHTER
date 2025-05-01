using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class LoadingScreen : MonoBehaviour
{
    public bool loading;
    public Image progressBar;
    public Image thumbnail;
    public TMP_Text stageName;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void OnPostRender()
    {
        if (!loading)
        {
            Time.timeScale = 1;
            loading = true;
            if (GameSystems.stageToLoad != null)
            {
                StartCoroutine(LoadAsynchronously(GameSystems.stageToLoad.sceneName));
                GameSystems.lastLoadedStage = GameSystems.stageToLoad;
                stageName.text = GameSystems.stageToLoad.stageName;
                thumbnail.sprite = GameSystems.stageToLoad.thumbnail;
            }
            else
            {
                StartCoroutine(LoadAsynchronously("MainMenu"));
            }
        }
    }

    IEnumerator LoadAsynchronously(string sceneName)
    {
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(sceneName);
        loadOperation.allowSceneActivation = true;

        while (!loadOperation.isDone)
        {
            progressBar.fillAmount = loadOperation.progress / 0.9f;
            yield return null;
        }
    }
}
