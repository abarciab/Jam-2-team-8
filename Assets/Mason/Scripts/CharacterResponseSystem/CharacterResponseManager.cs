using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterResponseManager : MonoBehaviour
{
    public static CharacterResponseManager instance;

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
        StartCoroutine(DialogueBase.instance.writeText(convertToLine(text)));
    }

    // this will eventuall be replaced when default parameters are set up for other override
    private DialogueLine convertToLine(string text) {
        DialogueLine line = new DialogueLine(text, Color.black, 
                                             Resources.GetBuiltinResource<Font>("Arial.ttf"), true, 0.01f, 0);
        return line;
    }

    // https://stackoverflow.com/questions/2804395/c-sharp-4-0-can-i-use-a-color-as-an-optional-parameter-with-a-default-value
    private DialogueLine convertToLine(string text, Color textColor, Font textFont, bool isNewLine, float scrollDelay, int soundID) {
        DialogueLine line = new DialogueLine(text, textColor, textFont, isNewLine, scrollDelay, soundID);
        return line;
    }
}
