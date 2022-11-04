using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueBase : MonoBehaviour
{
    public static DialogueBase instance;
    public bool finished { get; private set; }
    public TMP_FontAsset textFont = default;
    //private Text textHolder;
    private TextMeshProUGUI textHolder;

    private void Awake() {
        // if no duplicates
        if(instance == null) {
            instance = this;                    // makes instance a singleton
            //textHolder = GetComponent<Text>();  // get the text component
            textHolder = GetComponent<TextMeshProUGUI>();
            //DontDestroyOnLoad(gameObject);      // prevents DialogueBase from being destroyed when new level loaded
        }
        // if duplicates exist
        else if(instance != null && instance != this) {
            Destroy(gameObject);                // get rid of duplicate
        }
    }

    public IEnumerator writeText(DialogueLine line) {
        // set text properties
        textHolder.color = line.textColor;
        textHolder.font = textFont;
        CharacterResponseManager.instance.toggleButtons(false, Color.gray);

        // reset text if line is a new line
        if(line.isNewLine)
            textHolder.text = "";
        
        int soundIdNum;
        if(RealityManager.instance.getCharacterPronounByName(CharacterResponseManager.instance.currentCharacterName) == "he")
            soundIdNum = 4;
        else
            soundIdNum = 5;

        // loop through each letter in text and add it to text
        finished = false;
        for(int i = 0; i < line.text.Length; ++i) {
            textHolder.text += line.text[i];
            AudioManager.instance.PlayGlobal(soundIdNum, 1, restart:false);
            yield return new WaitForSeconds(line.scrollDelay);
        }

        // wait for mouse click to move onto next line
        CharacterResponseManager.instance.toggleButtons(true, Color.white);
        yield return new WaitUntil(() => Input.GetMouseButtonDown(0));
        finished = true;
    }
}
