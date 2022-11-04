using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditsScreenManager : MonoBehaviour
{
    public void loadCreditsScene() {
        SceneManager.LoadScene("Credits");
    }

    public void loadMainMenu() {
        SceneManager.LoadScene("Main Menu");
    }
}
