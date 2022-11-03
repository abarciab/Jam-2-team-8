using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class statementDropdown : MonoBehaviour
{
    private TMP_Dropdown menu;
    private List<string> statements;     // holds back-end names of evidence

    private void Awake() {
        menu = GetComponent<TMP_Dropdown>();
        statements = new List<string>();
    }

    private void OnEnable() {
        try {
            print("start running");
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
        
        // clear menu and statements for remaking
        menu.options.Clear();
        statements.Clear();

        // remake menu and statements
        menu.options.Add(defaultOption);
        statements.Add("BUFFER");        // added to keep indices consistent
        foreach(var line in JournalManager.instance.dialogueLog.characterLines) {
            if(line.speaker == CharacterResponseManager.instance.currentCharacterName) {
                foreach(var dialogue in line.lines) {
                    TMP_Dropdown.OptionData data = new TMP_Dropdown.OptionData();
                    //data.text = dialogue.line.Length < 15 ? dialogue.line : dialogue.line.Substring(0, 15) + "...";
                    data.text = dialogue.line;
                    menu.options.Add(data);
                    statements.Add(dialogue.line);
                }
            }
        }
    }

    public void submitEvidence() {
        // if "[Select Evidence]" is not selected
        if(menu.value != 0) {
            // write evidence response and hide evidence screen
            CharacterResponseManager.instance.writeCharacterDialogue(statements[menu.value]);
            UIManager.instance.hideUIElement("evidence");
        }        
    }
}
