using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EvidenceDropdown : MonoBehaviour
{
    private TMP_Dropdown menu;
    private List<string> evidenceNames;     // holds back-end names of evidence

    private void Awake() {
        menu = GetComponent<TMP_Dropdown>();
        evidenceNames = new List<string>();
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
        
        // clear menu and evidenceNames for remaking
        menu.options.Clear();
        evidenceNames.Clear();

        // remake menu and evidenceNames
        menu.options.Add(defaultOption);
        evidenceNames.Add("BUFFER");        // added to keep indices consistent
        foreach(EvidenceData evidence in EvidenceManager.instance.evidenceList) {
            TMP_Dropdown.OptionData data = new TMP_Dropdown.OptionData();
            data.text = evidence.displayName;
            menu.options.Add(data);
            evidenceNames.Add(evidence.name);
        }
    }

    public void submitEvidence() {
        // if "[Select Evidence]" is not selected
        if(menu.value != 0) {
            // write evidence response and hide evidence screen
            CharacterResponseManager.instance.writeCharacterDialogue(evidenceNames[menu.value]);
            UIManager.instance.hideUIElement("evidence");
        }        
    }
}
