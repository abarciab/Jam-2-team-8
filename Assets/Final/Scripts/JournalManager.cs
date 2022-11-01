using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JournalManager : MonoBehaviour
{
    public static JournalManager instance;

    public GameObject bookmarks;
    public GameObject book;

    public GameObject mapTab;
    public GameObject peopleTab;
    public GameObject EvidenceTab;
    public GameObject AccusationTab;

    public DialogueLog dialogueLog;

    private void Awake()
    {
        instance = this;
    }

    public void OpenJournal()
    {
        book.SetActive(true);
        bookmarks.SetActive(true);
    }

    public void CloseJournal()
    {
        book.SetActive(false);
        bookmarks.SetActive(false);
    }

    public void OpenPeopleTab()
    {
        SwitchTabs(0);
    }

    public void OpenEvidenceTab()
    {
        SwitchTabs(1);
    }

    public void OpenMapTab()
    {
        SwitchTabs(2);
    }

    public void OpenAccusationTab()
    {
        SwitchTabs(3);
    }

    void SwitchTabs(int index)
    {
        peopleTab.SetActive(index == 0 ? true : false);
        peopleTab.GetComponent<DialogueLog>().personSelector.SetActive(index == 0 ? true : false);
        EvidenceTab.SetActive(index == 1 ? true : false);
        mapTab.SetActive(index == 2 ? true : false);
        AccusationTab.SetActive(index == 3 ? true : false);
        
    }
}
