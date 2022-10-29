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
            DontDestroyOnLoad(gameObject);      // prevents UIManager from being destroyed when new level loaded
            hideDialogueUI();
        }
        // if duplicates exist
        else if(instance != null && instance != this) {
            Destroy(gameObject);                // get rid of duplicate
        }
    }

    public void hideDialogueUI() {
        foreach(Transform child in transform) {
            if(child.tag == "dialogue" || child.tag == "evidence")
                child.gameObject.SetActive(false);
            else
                child.gameObject.SetActive(true);
        }
    }

    public void showDialogueUI() {
        foreach(Transform child in transform) {
            if(child.tag == "dialogue" || child.tag == "journal") {
                if(child.tag == "dialogue")
                    child.Find("CharacterPortrait").GetComponent<Image>().sprite = CharacterResponseManager.instance.portraitSprite;
                child.gameObject.SetActive(true);
            }
        }
    }

    public void hideCharacters() {
        foreach(Transform child in transform) {
            if(child.tag == "character")
                child.gameObject.SetActive(false);
        }
    }

    public void hideEvidence() {
        foreach(Transform child in transform) {
            if(child.tag == "evidence")
                child.gameObject.SetActive(false);
        }
    }

    public void showEvidence() {
        foreach(Transform child in transform) {
            if(child.tag == "evidence")
                child.gameObject.SetActive(true);
        }
    }
}
