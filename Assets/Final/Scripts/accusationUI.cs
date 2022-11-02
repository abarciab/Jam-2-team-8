using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor;

public class accusationUI : MonoBehaviour
{
    //public Image murdererSprite;
    public TextMeshProUGUI murdererText;
    //public Image meansSprite;
    public TextMeshProUGUI meansText;
    //public Image motiveSprite;
    public TextMeshProUGUI motiveText;

    public string selectedMurderer = "sarah";
    public string selectedMeans = "GUN USED TO KILL JOE";
    string meansEvidence;
    public string selectedMotive = "GAMBLING DEBTS";
    string motiveEvidence;

    

    [Header("line")]
    public GameObject lineStart;
    public GameObject lineEnd;
    public Vector3 murdererLineStart;
    public Vector3 meansLineStart;
    public Vector3 motiveLineStart; 
    [Header("offsets")]
    public Vector3 murdererPinOffset;
    public Vector3 meansPinOffset;
    public Vector3 motivePinOffset;

    [Header("references")]
    public GameObject suspectSelectionPage;
    //public GameObject murdererGridContent;
    public GameObject meansSelectionPage;
    public GameObject meansGridContent;
    public GameObject motiveSelectionPage;
    public GameObject motiveGridContent;
    public GameObject listElementPrefab;

    [Header("testing")]
    public bool updateUI;

    // Start is called before the first frame update
    void Start()
    {
        UpdateUI();
    }

    // Update is called once per frame
    void Update()
    {
        if (updateUI) {
            updateUI = false;
            UpdateUI();
        }
    }

    void UpdateUI()
    {
        murdererText.text = selectedMurderer;
        //murdererSprite.sprite = RealityManager.instance.getCharacterPortraitByName(selectedMurderer);

        meansText.text = selectedMeans;
        //meansSprite.sprite = JSONParser.instance.getEvidenceSpriteByName(meansEvidence);

        //motiveText.text = selectedMotive.Replace("{THEY}", RealityManager.instance.getCharacterPronounByName(selectedMurderer));
        //motiveSprite.sprite = JSONParser.instance.getEvidenceSpriteByName(motiveEvidence);
    }

    public void OpenMurdererSelection()
    {
        for (int i = 0; i < suspectSelectionPage.transform.childCount; i++) {
            for (int j = 0; j < EvidenceManager.instance.charactersInteractedWith.Count; j++) {
                if (suspectSelectionPage.transform.GetChild(i).GetComponent<TextMeshProUGUI>().text.ToLower().Contains(EvidenceManager.instance.charactersInteractedWith[j].ToLower())) {
                    suspectSelectionPage.transform.GetChild(i).gameObject.SetActive(true);
                }
            }
        }
        lineStart.GetComponent<RectTransform>().localPosition = murdererLineStart; 
        lineEnd.GetComponent<RectTransform>().localPosition = murdererLineStart; 

        meansSelectionPage.SetActive(false);
        motiveSelectionPage.SetActive(false);
        suspectSelectionPage.SetActive(true);
    }

    public void OpenMeansSelection()
    {
        for (int i = 0; i < meansGridContent.transform.childCount; i++) {
            Destroy(meansGridContent.transform.GetChild(i).gameObject);
        }

        foreach (var evidence in EvidenceManager.instance.evidenceList) {
            if (evidence.means.Count > 0) {
                foreach (var means in evidence.means) {
                    if (means.applicableCharacters.Contains("ANY") || means.applicableCharacters.Contains(selectedMurderer.ToUpper())) {
                        
                        var newListItem = Instantiate(listElementPrefab, meansGridContent.transform);
                        var selectorScript = newListItem.GetComponent<AccusationEvidenceSelector>();
                        selectorScript.accusationComponent = AccusationEvidenceSelector.AccusationComponent.means;
                        selectorScript.UIScript = this;
                        selectorScript.evidenceName = evidence.name;
                        selectorScript.nameInSelectorList = evidence.displayName;
                        selectorScript.SelectedName = means.name;
                        selectorScript.evidenceSprite = JSONParser.instance.getEvidenceSpriteByName(evidence.name);
                        break;
                    }
                }
            }
        }

        lineStart.GetComponent<RectTransform>().localPosition = meansLineStart;
        lineEnd.GetComponent<RectTransform>().localPosition = meansLineStart;

        suspectSelectionPage.SetActive(false);
        motiveSelectionPage.SetActive(false);
        meansSelectionPage.SetActive(true);
    }
    public void OpenMotiveSelection()
    {
        for (int i = 0; i < motiveGridContent.transform.childCount; i++) {
            Destroy(motiveGridContent.transform.GetChild(i).gameObject);
        }

        foreach (var evidence in JSONParser.instance.evidenceData) {
            if (evidence.motive.Count > 0) {
                foreach (var motive in evidence.motive) {
                    if (motive.applicableCharacters.Contains("any") || motive.applicableCharacters.Contains(selectedMurderer)) {
                        var newListItem = Instantiate(listElementPrefab, motiveGridContent.transform);
                        var selectorScript = newListItem.GetComponent<AccusationEvidenceSelector>();
                        selectorScript.accusationComponent = AccusationEvidenceSelector.AccusationComponent.motive;
                        selectorScript.UIScript = this;
                        selectorScript.evidenceName = evidence.name;
                        selectorScript.nameInSelectorList = evidence.displayName;
                        selectorScript.SelectedName = motive.name;
                        selectorScript.evidenceSprite = JSONParser.instance.getEvidenceSpriteByName(evidence.name);
                    }
                }
            }
        }
        suspectSelectionPage.SetActive(false);
        meansSelectionPage.SetActive(false);
        motiveSelectionPage.SetActive(true);
    }

    public void moveEndPoint(GameObject newEnd)
    {
        Vector3 offset = Vector3.zero;
        if (suspectSelectionPage.activeInHierarchy) {
            offset = murdererPinOffset;
        }
        else if (meansSelectionPage.activeInHierarchy) {
            offset = meansPinOffset;
        }
        else if (motiveSelectionPage.activeInHierarchy) {
            offset = motivePinOffset;
        }
        lineEnd.transform.position = newEnd.transform.position + offset;
    }

    public void SelectMurderer(string characterName)
    {
        selectedMurderer = characterName;
        //suspectSelectionPage.SetActive(false);
        UpdateUI();
    }

    public void SelectMeans(string evidence, string means)
    {
        selectedMeans = means;
        meansEvidence = evidence;
        //meansSelectionPage.SetActive(false);
        UpdateUI();
    }

    public void SelectMotive(string evidence, string motive)
    {
        motiveEvidence = evidence;
        selectedMotive = motive;
        //motiveSelectionPage.SetActive(false);
        UpdateUI();
    }

}

