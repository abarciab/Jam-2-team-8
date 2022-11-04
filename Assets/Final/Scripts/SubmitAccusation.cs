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
                // switch the lie to the truth statement and write it out
                sDropdown.lineData[statementMenu.value].switchTextToTruth();
                CharacterResponseManager.instance.writeCharacterDialogue(sDropdown.dialogueTypes[statementMenu.value]);
                AudioManager.instance.PlayGlobal(6, 1, false);
            }
            else {
                // play some sort of error sound
                AudioManager.instance.PlayGlobal(7, 1, false);
            }
            UIManager.instance.hideUIElement("lying");
        }
    }
}
