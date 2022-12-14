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

    AudioSource source;

    private void Awake() {
        // if no duplicates
        if(instance == null) {
            instance = this;                    // makes instance a singleton
            //DontDestroyOnLoad(gameObject);      // prevents CharacterResponse from being destroyed when new level loaded
        }
        // if duplicates exist
        else if(instance != null && instance != this) {
            Destroy(gameObject);                // get rid of duplicate
        }
        if (source == null) {
            source = gameObject.AddComponent<AudioSource>();
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
    // get the dialogue type from the character and display the text for it
    public void writeCharacterDialogue(string dialogueType) {
        //print("writing dialogue. type: " + dialogueType);
        //this is really unclean for now
        CharacterDialogueData character = RealityManager.instance.getCharacterDialogue(currentCharacterName);
        string text = "";
        string evidenceGained = "";
        if(dialogueType == "alibi") {
            text = character.alibi.text;
            evidenceGained = character.alibi.evidenceGained;
        }
        else if(dialogueType == "relationship") {
            text = character.relationship.text;
            evidenceGained = character.relationship.evidenceGained;
        }
        else if(dialogueType == "default") {
            text = character.defaultResponse.text;
            evidenceGained = character.defaultResponse.evidenceGained;
        }
        else {
            bool found = false;
            foreach(EvidenceResponseData response in character.evidenceResponses) {
                if(response.item.ToUpper() == dialogueType.ToUpper()) {
                    text = response.line.text;
                    evidenceGained = response.line.evidenceGained;
                    found = true;
                    break;
                }
            }
            if(!found) {
                text = character.defaultResponse.text;
                evidenceGained = character.defaultResponse.evidenceGained;
            }
        }

        // log evidence and character name in evidence manager
        if(!string.IsNullOrEmpty(evidenceGained)) {
            if (!EvidenceManager.instance.IsAlreadyCollected(evidenceGained)) {
                EvidenceManager.instance.addEvidence(evidenceGained);
                AudioManager.instance.PlayHere(12, source: source);
                FindObjectOfType<JournalButton>().TurnOnSparkles();
            }
        }
        EvidenceManager.instance.addCharacter(currentCharacterName);

        StartCoroutine(DialogueBase.instance.writeText(new DialogueLine(text)));

        //record dialogue lines in the journal:
        JournalManager.instance.dialogueLog.RecordDialogue(dialogueType, character);
    }

    public void characterGreeting(DialogueLine greeting) {
        StartCoroutine(DialogueBase.instance.writeText(greeting));
    }

    // enables or disables buttons
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
