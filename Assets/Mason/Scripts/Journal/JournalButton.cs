using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class JournalButton : MonoBehaviour
{
    [SerializeField] private Sprite journalSprite;
    [SerializeField] private Sprite exitSprite;
    private Button button;
    private bool journalOpen = false;

    private void Start() {
        button = GetComponent<Button>();
        button.onClick.AddListener(toggleJournal);
    }

    private void toggleJournal() {
        // close journal if open
        if(journalOpen) {
            print("Journal is now closed");
            journalOpen = false;
            JournalManager.instance.CloseJournal();
            button.image.sprite = journalSprite;
        }
        // open journal if closed
        else {
            print("Journal is now open");
            journalOpen = true;
            JournalManager.instance.OpenJournal();
            button.image.sprite = exitSprite;
        }
    }
}
