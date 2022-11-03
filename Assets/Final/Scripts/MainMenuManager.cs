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

    public Transform menuFade;

    public void startMenuTransition() {
        menuFade.gameObject.SetActive(true);
        menuFade.GetComponent<Animator>().SetTrigger("startFade");
    }

    public void startGame() {
        startImmediatly = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (startImmediatly) {
            startImmediatly = false;
            if (!SceneManager.GetSceneByName(managerSceneName).isLoaded)
                SceneManager.LoadScene(managerSceneName, LoadSceneMode.Additive);
            if (!SceneManager.GetSceneByName(coreSceneName).isLoaded)
                SceneManager.LoadScene(coreSceneName, LoadSceneMode.Additive);
            if (!SceneManager.GetSceneByName(journalSceneName).isLoaded)
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
