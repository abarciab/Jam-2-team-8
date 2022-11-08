using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEditor.Build.Content;
using UnityEngine.SceneManagement;

[ExecuteAlways]
public class tutorialCoordinator : MonoBehaviour
{
    [System.Serializable]
    public class TextLine
    {
        [TextArea(2, 6)]
        public string text;
        public int ID;
    }

    public bool AssignIDs;
    public List<TextLine> lines;

    public int tutorialID;
    int currentIndex;
    public float textSpeed = 0.01f;
    bool writingLine;
    bool loadedOtherScenes;

    [Header("references")]
    public GameObject skipTutorialOption;
    public TextMeshProUGUI mainText;
    public GameObject nextButton;

    [Header("Scenes")]
    public string coreSceneName;
    public string journalSceneName;
    public string managerSceneName;
    public string IntroSceneName;

    private void Start()
    {
        skipTutorialOption.SetActive(false);
        mainText.text = "";
        DisplayNextLine();

        if (Application.isPlaying) 
            AudioManager.instance.PlayMusic(0);
    }

    private void Update()
    {
        if (AssignIDs) {
            AssignIDs = false;
            for (int i = 0; i < lines.Count; i++) {
                lines[i].ID = i;
            }
        }
        if (!Application.isPlaying) {
            StopAllCoroutines();
            return;
        }

        nextButton.SetActive(currentIndex != tutorialID);

        if (!writingLine) {
            if (currentIndex == tutorialID) {
                skipTutorialOption.SetActive(true);
                nextButton.SetActive(false);
            }
        }
        if (loadedOtherScenes) {
            if (SceneManager.GetSceneByName(coreSceneName).isLoaded) {
                SceneManager.SetActiveScene(SceneManager.GetSceneByName(coreSceneName));
                SceneManager.UnloadSceneAsync(IntroSceneName);
            }
        }

    }

    public void DisplayNextLine()
    {
        StopAllCoroutines();
        if (writingLine) {
            mainText.text = lines[currentIndex == 0 ? 0 : currentIndex-1].text;
            writingLine = false;
            return;
        }
        if (currentIndex == lines.Count) {
            StartGame();
            return;
        }
        mainText.text = "";
        writingLine = true;
        StartCoroutine(writeText(lines[currentIndex].text));
        currentIndex += 1;
    }

    private IEnumerator writeText(string text)
    {
        for (int i = 0; i < text.Length; i++) {
            mainText.text += text[i];
            AudioManager.instance.PlayGlobal(1);
            yield return new WaitForSeconds(textSpeed);
        }
        writingLine = false;
    }

    public void StartGame()
    {
        Camera.main.GetComponent<AudioListener>().enabled = false;
        if (!SceneManager.GetSceneByName(managerSceneName).isLoaded)
            SceneManager.LoadScene(managerSceneName, LoadSceneMode.Additive);
        if (!SceneManager.GetSceneByName(coreSceneName).isLoaded)
            SceneManager.LoadScene(coreSceneName, LoadSceneMode.Additive);
        if (!SceneManager.GetSceneByName(journalSceneName).isLoaded)
            SceneManager.LoadScene(journalSceneName, LoadSceneMode.Additive);
        loadedOtherScenes = true;
        
    }
}
