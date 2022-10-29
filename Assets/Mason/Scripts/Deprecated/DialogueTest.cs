using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTest : MonoBehaviour
{
    public List<DialogueLine> lines = new List<DialogueLine>();

    private void Start() {
        StartCoroutine(displayLines());
    }

    private IEnumerator displayLines() {
        // loop through each line and use DialogueBase instance to write it
        for(int i = 0; i < lines.Count; ++i) {
            DialogueLine line = lines[i];
            StartCoroutine(DialogueBase.instance.writeText(line));
            yield return new WaitUntil(() => DialogueBase.instance.finished);
        }
    }
}
