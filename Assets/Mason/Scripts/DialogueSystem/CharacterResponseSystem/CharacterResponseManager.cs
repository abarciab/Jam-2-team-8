using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterResponseManager : MonoBehaviour
{
    [SerializeField] private Transform buttonHolder;
    public static CharacterResponseManager instance;
    public string currentCharacterName;
    public Sprite portraitSprite;

    private void Awake() {
        // if no duplicates
        if(instance == null) {
            instance = this;                    // makes instance a singleton
            DontDestroyOnLoad(gameObject);      // prevents CharacterResponse from being destroyed when new level loaded
        }
        // if duplicates exist
        else if(instance != null && instance != this) {
            Destroy(gameObject);                // get rid of duplicate
        }
    }
/*
    public void writeCharacterDialogue(string characterName, string dialogueType) {
        //this is really unclean for now
        CharacterDialogueData character = RealityManager.instance.getCharacterDialogue(characterName);
        string text;
        if(dialogueType == "alibi")
            text = character.alibi.text;
        else if(dialogueType == "relationship")
            text = character.relationship.text;
        else if(dialogueType == "default")
            text = character.defaultResponse.text;
        else {
            Debug.LogError("dialogueType " + dialogueType + " not found");
            return;
        }
        StartCoroutine(DialogueBase.instance.writeText(new DialogueLine(text)));
    }
*/
    public void writeCharacterDialogue(string dialogueType) {
        //this is really unclean for now
        CharacterDialogueData character = RealityManager.instance.getCharacterDialogue(currentCharacterName);
        string text;
        if(dialogueType == "alibi")
            text = character.alibi.text;
        else if(dialogueType == "relationship")
            text = character.relationship.text;
        else if(dialogueType == "default")
            text = character.defaultResponse.text;
        else {
            Debug.LogError("dialogueType " + dialogueType + " not found");
            return;
        }
        StartCoroutine(DialogueBase.instance.writeText(new DialogueLine(text)));
    }

    public void characterGreeting(DialogueLine greeting) {
        StartCoroutine(DialogueBase.instance.writeText(greeting));
    }

    public void toggleButtons(bool interactable, Color buttonColor) {
        foreach(Transform dialogueButton in buttonHolder) {
            Button button = dialogueButton.GetComponent<Button>();
            button.interactable = interactable;
            changeButtonColor(button, buttonColor);
        }
    }

    private void changeButtonColor(Button button, Color color) {
        ColorBlock cb = button.colors;
        cb.normalColor = color;
        button.colors = cb;
    }
}
