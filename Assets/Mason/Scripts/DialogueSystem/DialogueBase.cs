using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DialogueBase : MonoBehaviour
{
    public static DialogueBase instance;
    public bool finished { get; private set; }
    private Text textHolder;

    private void Awake() {
        // if no duplicates
        if(instance == null) {
            instance = this;                    // makes instance a singleton
            textHolder = GetComponent<Text>();  // get the text component
            DontDestroyOnLoad(gameObject);      // prevents DialogueBase from being destroyed when new level loaded
        }
        // if duplicates exist
        else if(instance != null && instance != this) {
            Destroy(gameObject);                // get rid of duplicate
        }
    }

    // TODO: Disable dialogue buttons while writing dialogue
    public IEnumerator writeText(DialogueLine line) {
        // set text properties
        textHolder.color = line.textColor;
        textHolder.font = line.textFont;

        // reset text if line is a new line
        if(line.isNewLine)
            textHolder.text = "";

        // loop through each letter in text and add it to text
        finished = false;
        for(int i = 0; i < line.text.Length; ++i) {
            textHolder.text += line.text[i];
            AudioManager.instance.PlayGlobal(line.soundID, 1, true);
            yield return new WaitForSeconds(line.scrollDelay);
        }

        // wait for mouse click to move onto next line
        yield return new WaitUntil(() => Input.GetMouseButtonDown(0));
        finished = true;
    }
}
