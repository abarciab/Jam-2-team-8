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
            // if statement is a lie and evidence is evidence required
            StatementDropdown sDropdown = statementMenu.GetComponent<StatementDropdown>();
            EvidenceDropdown eDropdon = evidenceMenu.GetComponent<EvidenceDropdown>();
            if(sDropdown.statementsAreLies[statementMenu.value]) {
                UIManager.instance.hideUIElement("lying");
                CharacterResponseManager.instance.writeCharacterDialogue("test");
            }
        }
    }
}
