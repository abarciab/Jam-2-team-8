using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public string IntroSceneName;

    public Transform menuFade;

    private void Start()
    {
        AudioManager.instance.PlayMusic(0);
    }

    public void startMenuTransition() {
        menuFade.gameObject.SetActive(true);
        menuFade.GetComponent<Animator>().SetTrigger("startFade");
        GetComponent<Animator>().SetTrigger("fade");
    }

    public void startGame() {
        SceneManager.LoadScene(IntroSceneName);
    }
    public void DisableAudioListener()
    {
        Camera.main.GetComponent<AudioListener>().enabled = false;
    }

    public void PlayHoverSound()
    {
        AudioManager.instance.PlayGlobal(1, restart:true);
    }

    public void PlayClickSound()
    {
        AudioManager.instance.PlayGlobal(2, restart: true);
    }
}
