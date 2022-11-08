using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class JournalButton : MonoBehaviour
{
    [SerializeField] private Sprite journalSprite;
    [SerializeField] private Sprite exitSprite;
    [SerializeField] private GameObject sparkles;
    private Button button;
    private bool journalOpen = false;

    private void Start() {
        button = GetComponent<Button>();
        button.onClick.AddListener(toggleJournal);
    }

    public void TurnOnSparkles()
    {
        sparkles.SetActive(true);
        GetComponent<Animator>().SetBool("wiggle", true);
    }

    public void toggleJournal() {
        GetComponent<Animator>().SetBool("wiggle", false);
        sparkles.SetActive(false);
        // close journal if open
        if (journalOpen) {
            
            //print("Journal is now closed");
            journalOpen = false;
            JournalManager.instance.CloseJournal();
            transform.GetChild(0).gameObject.SetActive(false);
        }
        // open journal if closed
        else {
            journalOpen = true;
            JournalManager.instance.OpenJournal();
            //button.image.sprite = exitSprite;
            transform.GetChild(0).gameObject.SetActive(true);
        }
        AudioManager.instance.PlayGlobal(9, restart: false);
    }
}
