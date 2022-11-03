using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    private void Awake() {
        // if no duplicates
        if(instance == null) {
            instance = this;                    // makes instance a singleton
            //DontDestroyOnLoad(gameObject);      // prevents UIManager from being destroyed when new level loaded
            hideDialogueUI(true);
        }
        // if duplicates exist
        else if(instance != null && instance != this) {
            Destroy(gameObject);                // get rid of duplicate
        }
    }

    // reset screen back to normal gameplay
    public void hideDialogueUI(bool showFade) {
        foreach(Transform child in transform) {
            if(child.tag == "dialogue" || child.tag == "evidence" || child.tag == "lying" || (!showFade && child.tag == "fade" || child.gameObject.name == "card"))
                child.gameObject.SetActive(false);
            else
                child.gameObject.SetActive(true);
        }
    }

    // start dialogue with character
    public void showDialogueUI() {
        foreach(Transform child in transform) {
            if(child.tag == "dialogue" || child.tag == "journal") {
                if(child.tag == "dialogue")
                    child.Find("CharacterPortrait").GetComponent<Image>().sprite = CharacterResponseManager.instance.portraitSprite;
                child.gameObject.SetActive(true);
            }
        }
    }

    // hide a specific UI element
    public void hideUIElement(string itemTag) {
        foreach(Transform child in transform) {
            if(child.tag == itemTag)
                child.gameObject.SetActive(false);
        }
    }

    // show a specific UI element
    public void showUIElement(string itemTag) {
        foreach(Transform child in transform) {
            if(child.tag == itemTag)
                child.gameObject.SetActive(true);
        }
    }
}
