using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SubmitAccusation : MonoBehaviour
{
    public TMP_Dropdown statementMenu;
    public TMP_Dropdown evidenceMenu;

    public void submit() {
        // if "[Select Statement]" and "[Select Evidence]" is not selected
        if(statementMenu.value != 0 && evidenceMenu.value != 0) {
            StatementDropdown sDropdown = statementMenu.GetComponent<StatementDropdown>();
            EvidenceDropdown eDropdown = evidenceMenu.GetComponent<EvidenceDropdown>();

            // if statement is a lie and evidence is evidence required
            if(sDropdown.lineData[statementMenu.value].lie && 
              (eDropdown.evidenceNames[evidenceMenu.value] == sDropdown.lineData[statementMenu.value].requiredEvidence)) {
                UIManager.instance.hideUIElement("lying");
                CharacterResponseManager.instance.writeCharacterDialogue("test");
            }
            else {
                UIManager.instance.hideUIElement("lying");
                // play some sort of error sound
            }
        }
    }
}
