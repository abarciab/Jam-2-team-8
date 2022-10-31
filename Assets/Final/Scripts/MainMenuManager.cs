using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public bool startImmediatly;
    bool loadedOtherScenes;

    public string coreSceneName;
    public string journalSceneName;
    public string managerSceneName;
    public string mainMenuSceneName;

    // Update is called once per frame
    void Update()
    {
        if (startImmediatly) {
            startImmediatly = false;
            SceneManager.LoadScene(managerSceneName, LoadSceneMode.Additive);
            SceneManager.LoadScene(coreSceneName, LoadSceneMode.Additive);
            SceneManager.LoadScene(journalSceneName, LoadSceneMode.Additive);
            loadedOtherScenes = true; 
        }
        if (loadedOtherScenes) {
            if (SceneManager.GetSceneByName(coreSceneName).isLoaded) {
                SceneManager.SetActiveScene(SceneManager.GetSceneByName(coreSceneName));
                SceneManager.UnloadSceneAsync(mainMenuSceneName);
            }
        }
    }
}
