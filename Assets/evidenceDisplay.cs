using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class evidenceDisplay : MonoBehaviour
{
    public GameObject evidenceEntry1;
    public GameObject evidenceEntry2;
    public GameObject evidenceEntry3;
    public GameObject evidenceEntry4;
    public GameObject evidenceEntry5;
    public GameObject evidenceEntry6;
    public GameObject previousPageButton;
    public GameObject nextPageButton;

    public int currentPage;
    public int currentEvidenceIndex;
    Dictionary<int, int> pageToEvidenceCount = new Dictionary<int, int>();   //how many pieces of evidence are on each page;

    private void OnEnable()
    {
        currentEvidenceIndex = 0;
        DisplayEvidence();
    }

    public void NextPage() {
        if (!isActiveAndEnabled) { return; }
        currentPage += 1;
        DisplayEvidence();
    }

    public void PreviousPage() {
        if (!isActiveAndEnabled || currentPage == 0) { return; }
        print("currentpage index count" + pageToEvidenceCount[currentPage]);
        currentEvidenceIndex -= (pageToEvidenceCount[currentPage] + 6);
        print("previous page. currentIndex: " + currentEvidenceIndex);
        currentPage -= 1;
        DisplayEvidence();
    }

    void clearPages() {
        evidenceEntry1.SetActive(false);
        evidenceEntry2.SetActive(false);
        evidenceEntry3.SetActive(false);
        evidenceEntry4.SetActive(false);
        evidenceEntry5.SetActive(false);
        evidenceEntry6.SetActive(false);
    }

    void DisplayEvidence(int evidenceIndex = -1)
    {
        print("displaying evidence. currentIndex: " + currentEvidenceIndex);
        clearPages();
        if (evidenceIndex == -1) {
            evidenceIndex = currentEvidenceIndex;
        }
        if (evidenceIndex >= EvidenceManager.instance.evidenceList.Count) {
            return;
        }

        int currentPos = 1;
        for (int i = currentEvidenceIndex; i < EvidenceManager.instance.evidenceList.Count; i++) {
            var evidence = EvidenceManager.instance.evidenceList[i];
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

            entryToModify.SetActive(true);
            for (int j = 0; j < entryToModify.transform.childCount; j++) {
                var child = entryToModify.transform.GetChild(j);
                if (child.GetComponent<Image>()) {
                    child.GetComponent<Image>().sprite = JSONParser.instance.getEvidenceSpriteByName(evidence.name);
                }
                if (child.gameObject.name.ToLower().Contains("name")) {
                    child.GetComponent<TextMeshProUGUI>().text = evidence.displayName;
                }
                if (child.gameObject.name.ToLower().Contains("description")){
                    child.GetComponent<TextMeshProUGUI>().text = evidence.description;
                }
            }
            currentPos += 1;
            evidenceIndex += 1;
            currentEvidenceIndex += 1;

            if (currentPos > 6) {
                print("breaking bc currentPos is 6");
                break;
            }
        }

        if (currentPage > 0) {
            previousPageButton.SetActive(true);
        }
        else {
            previousPageButton.SetActive(false);
        }
        if (EvidenceManager.instance.evidenceList.Count > currentEvidenceIndex && EvidenceManager.instance.evidenceList.Count > 0) {
            nextPageButton.SetActive(true);
            print("current latest index: " + currentEvidenceIndex + ", count: " + EvidenceManager.instance.evidenceList.Count);
        }
        else {
            nextPageButton.SetActive(false);
        }

        if (!pageToEvidenceCount.ContainsKey(currentPage)) {
            pageToEvidenceCount.Add(currentPage, currentPos-1   );
        }
    }
}
