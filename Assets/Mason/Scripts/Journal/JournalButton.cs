using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class JournalButton : MonoBehaviour
{
    private Button button;
    private bool journalOpen = false;

    private void Start() {
        button = GetComponent<Button>();
        button.onClick.AddListener(toggleJournal);
    }

    private void toggleJournal() {
        if(journalOpen) {
            journalOpen = false;
            print("Journal is now closed");
            // switch sprite to journal button
            JournalManager.instance.CloseJournal();
            GetComponentInChildren<TextMeshProUGUI>().SetText("Journal");
        }
        else {
            journalOpen = true;
            print("Journal is now open");
            // switch sprite to the an exit button
            JournalManager.instance.OpenJournal();
            GetComponentInChildren<TextMeshProUGUI>().SetText("Exit");
        }
    }
}
