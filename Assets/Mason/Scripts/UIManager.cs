using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    private void Awake() {
        // if no duplicates
        if(instance == null) {
            instance = this;                    // makes instance a singleton
            DontDestroyOnLoad(gameObject);      // prevents UIManager from being destroyed when new level loaded
            hideUI();
        }
        // if duplicates exist
        else if(instance != null && instance != this) {
            Destroy(gameObject);                // get rid of duplicate
        }
    }

    public void hideUI() {
        foreach(Transform child in transform) {
            if(child.tag != "character") {
                child.gameObject.SetActive(false);
            }
        }
    }

    public void showUI() {
        foreach(Transform child in transform) {
            child.gameObject.SetActive(true);
        }
    }
}
