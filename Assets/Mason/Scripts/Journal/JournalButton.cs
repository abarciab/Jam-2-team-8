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
            GetComponentInChildren<TextMeshProUGUI>().SetText("Journal");
        }
        else {
            journalOpen = true;
            print("Journal is now open");
            // switch sprite to the an exit button
            GetComponentInChildren<TextMeshProUGUI>().SetText("Exit");
        }
    }
}
