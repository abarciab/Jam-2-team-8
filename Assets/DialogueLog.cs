using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueLog : MonoBehaviour
{
    public Image characterPortrait;


    [System.Serializable]
    public class RecordedLine
    {
        public string speaker;

        [System.Serializable]
        public class lineDetails
        {   
            public string line;
            public string question;
            //public DialogueLineData lineData;
        }
    }

    public List<RecordedLine> recordedLines = new List<RecordedLine>();

    private void OnEnable()
    {
        
    }

    public void RecordDialogue(string dialogueType, CharacterDialogueData character)
    {
        switch (dialogueType) {
            case "alibi":
                JournalManager.instance.dialogueLog.RecordDialogue(character.alibi, character.characterName, alibi: true);
                break;
            case "relationship":
                JournalManager.instance.dialogueLog.RecordDialogue(character.relationship, character.characterName, relationship: true);
                break;
            default:
                foreach (var response in character.evidenceResponses) {
                    if (response.item == dialogueType) {
                        JournalManager.instance.dialogueLog.RecordDialogue(response.line, character.characterName, evidence: dialogueType);
                    }
                }
                break;
        }
    }

    public void RecordDialogue(DialogueLineData line, string speaker, bool alibi = false, bool relationship = false, string evidence = null)
    {
        var newRecordedLine = new RecordedLine();
        newRecordedLine.speaker = speaker;
        
    }

}
