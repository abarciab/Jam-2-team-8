using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class evidenceDisplay : MonoBehaviour
{
    public GameObject evidenceEntry1;
    public GameObject evidenceEntry2;
    public GameObject evidenceEntry3;
    public GameObject evidenceEntry4;
    public GameObject evidenceEntry5;
    public GameObject evidenceEntry6;

    public int currentPage;
    public int currentIndex;

    private void OnEnable()
    {
        currentIndex = 0;
        
    }

    void DisplayEvidence(int startingIndex = -1)
    {
        if (startingIndex == -1) {
            startingIndex = currentIndex;
        }

        int currentPos = currentIndex % 6;
        foreach (var evidence in EvidenceManager.instance.evidenceList) {
            GameObject entryToModify = evidenceEntry1;
            switch (currentPos) {
                case 1:
                    entryToModify = evidenceEntry1;
                    break;
                case 2:
                    entryToModify = evidenceEntry2;
                    break;
                case 3:
                    entryToModify = evidenceEntry3;
                    break;
                case 4:
                    entryToModify = evidenceEntry4;
                    break;
                case 5:
                    entryToModify = evidenceEntry5;
                    break;
                case 6:
                    entryToModify = evidenceEntry6;
                    break;
            }
            for (int i = 0; i < entryToModify.transform.childCount; i++) {

            }

        }
    }
}
