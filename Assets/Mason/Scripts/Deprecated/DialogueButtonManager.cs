using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueButtonManager : MonoBehaviour
{
    public static DialogueButtonManager instance;
    public string characterName = "sarah";

    private void Awake() {
        // if no duplicates
        if(instance == null) {
            instance = this;                    // makes instance a singleton
            setCharacter(characterName);        // set initial character
            DontDestroyOnLoad(gameObject);      // prevents DialogueButtonManager from being destroyed when new level loaded
        }
        // if duplicates exist
        else if(instance != null && instance != this) {
            Destroy(gameObject);                // get rid of duplicate
        }
    }

    public void setCharacter(string newCharacterName) {
        // loop through each button and set the name of the new character
        characterName = newCharacterName;
        foreach(Transform child in transform) {
            DialogueButtons button = child.GetComponent<DialogueButtons>();
            if(button != null)
                button.characterName = characterName;
        }
    }
}
