using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class menuFade : MonoBehaviour
{
    public Transform mainMenuManager;

    public void loadGame()  {
        mainMenuManager.GetComponent<MainMenuManager>().startGame();
    }
}
