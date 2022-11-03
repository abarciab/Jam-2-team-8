using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StatementDropdown : MonoBehaviour
{
    private TMP_Dropdown menu;
    private List<string> statements;     // holds back-end statements
    public List<DialogueLineData> lineData { get; private set; }

    private void Awake() {
        menu = GetComponent<TMP_Dropdown>();
        statements = new List<string>();
        lineData = new List<DialogueLineData>();
    }

    private void OnEnable() {
        try {
            refreshDropdown();
            menu.value = 0;
        }
        catch(System.NullReferenceException err)  {
            ;
        }
    }

    public void refreshDropdown() {
        // make default option
        TMP_Dropdown.OptionData defaultOption = new TMP_Dropdown.OptionData();
        defaultOption.text = "[Select Statement]";
        
        // clear menu and lists for remaking
        menu.options.Clear();
        lineData.Clear();

        // remake menu and statements
        menu.options.Add(defaultOption);
        statements.Add("BUFFER");        // added to keep indices consistent
        lineData.Add(new DialogueLineData());
        foreach(var line in JournalManager.instance.dialogueLog.characterLines) {
            if(line.speaker == CharacterResponseManager.instance.currentCharacterName) {
                foreach(var dialogue in line.lines) {
                    TMP_Dropdown.OptionData data = new TMP_Dropdown.OptionData();
                    data.text = dialogue.line;
                    menu.options.Add(data);
                    statements.Add(dialogue.line);
                    lineData.Add(dialogue.lineData);
                }
            }
        }
    }

    private void Update() {
        if(Input.GetKeyDown(KeyCode.Alpha9)) {
            foreach(var line in lineData) {
                print(line.text);
            }
        }
    }
}
