using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StatementDropdown : MonoBehaviour
{
    private TMP_Dropdown menu;
    private List<string> statements;     // holds back-end statements
    public List<bool> statementsAreLies { get; private set; }  // holds whether statements are lies or not

    private void Awake() {
        menu = GetComponent<TMP_Dropdown>();
        statements = new List<string>();
        statementsAreLies = new List<bool>();
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
        
        // clear menu and lists for remaking
        menu.options.Clear();
        statements.Clear();
        statementsAreLies.Clear();

        // remake menu and statements
        menu.options.Add(defaultOption);
        statements.Add("BUFFER");        // added to keep indices consistent
        statementsAreLies.Add(false);
        foreach(var line in JournalManager.instance.dialogueLog.characterLines) {
            if(line.speaker == CharacterResponseManager.instance.currentCharacterName) {
                foreach(var dialogue in line.lines) {
                    TMP_Dropdown.OptionData data = new TMP_Dropdown.OptionData();
                    data.text = dialogue.line;
                    menu.options.Add(data);
                    statements.Add(dialogue.line);
                    statementsAreLies.Add(true);   // this will be changed to log if a statement is a lie or not
                    // might need another list for the required evidence
                }
            }
        }
    }
}
