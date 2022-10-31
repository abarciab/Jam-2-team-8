using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EvidenceDropdown : MonoBehaviour
{
    //private Dropdown menu;
    private TMP_Dropdown menu;

    private void Awake() {
        menu = GetComponent<TMP_Dropdown>();
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
        defaultOption.text = "[Select Evidence]";
        
        // clear menu and remake it
        menu.options.Clear();
        menu.options.Add(defaultOption);
        foreach(EvidenceData evidence in EvidenceManager.instance.evidenceList) {
            TMP_Dropdown.OptionData data = new TMP_Dropdown.OptionData();
            data.text = evidence.displayName;
            menu.options.Add(data);
        }
    }

    public void submitEvidence() {
        if(menu.value != 0) {
            // this will be good for later on when we need to check if this evidence
            // means anything to the character
            string currentCharacterName = CharacterResponseManager.instance.currentCharacterName;
            CharacterDialogueData character = RealityManager.instance.getCharacterDialogue(currentCharacterName);
            

            CharacterResponseManager.instance.writeCharacterDialogue("default");
            UIManager.instance.hideUIElement("evidence");
        }        
    }
}
