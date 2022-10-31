using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueLog : MonoBehaviour
{
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

    public void RecordDialogue(DialogueLineData line, string speaker, bool alibi = false, bool relationship = false, string evidence = null)
    {
        var newRecordedLine = new RecordedLine();
        newRecordedLine.speaker = speaker;
        
    }

}
