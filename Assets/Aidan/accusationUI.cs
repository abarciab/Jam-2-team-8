using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor;

public class accusationUI : MonoBehaviour
{
    public Image murdererSprite;
    public TextMeshProUGUI murdererText;
    public Image meansSprite;
    public TextMeshProUGUI meansText;
    public Image motiveSprite;
    public TextMeshProUGUI motiveText;

    public string selectedMurderer = "sarah";
    public string selectedMeans = "GUN USED TO KILL JOE";
    string meansEvidence;
    public string selectedMotive = "GAMBLING DEBTS";
    string motiveEvidence;

    public GameObject murdererSelectionMenu;
    public GameObject murdererGridContent;
    public GameObject meansSelectionMenu;
    public GameObject meansGridContent;
    public GameObject motiveSelectionMenu;
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
        murdererSprite.sprite = RealityManager.instance.getCharacterPortraitByName(selectedMurderer);

        meansText.text = selectedMeans;
        meansSprite.sprite = JSONParser.instance.getEvidenceSpriteByName(meansEvidence);

        motiveText.text = selectedMotive.Replace("{THEY}", RealityManager.instance.getCharacterPronounByName(selectedMurderer));
        motiveSprite.sprite = JSONParser.instance.getEvidenceSpriteByName(motiveEvidence);
    }

    public void OpenMurdererSelection()
    {
        for (int i = 0; i < murdererGridContent.transform.childCount; i++) {
            Destroy(murdererGridContent.transform.GetChild(i).gameObject);
        }

        foreach (var character in RealityManager.instance.allCharacters) {
            var newListItem = Instantiate(listElementPrefab, murdererGridContent.transform);
            var selectorScript = newListItem.GetComponent<AccusationEvidenceSelector>();
            selectorScript.SelectedName = character.characterName;
            selectorScript.evidenceSprite = RealityManager.instance.getCharacterPortraitByName(character.characterName);
            selectorScript.UIScript = this;
        }

        murdererSelectionMenu.SetActive(true);
    }

    public void OpenMeansSelection()
    {
        for (int i = 0; i < meansGridContent.transform.childCount; i++) {
            Destroy(meansGridContent.transform.GetChild(i).gameObject);
        }

        foreach (var evidence in JSONParser.instance.evidenceData) {
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
        meansSelectionMenu.SetActive(true);
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
        motiveSelectionMenu.SetActive(true);
    }

    public void SelectMurderer(string characterName)
    {
        selectedMurderer = characterName;
        murdererSelectionMenu.SetActive(false);
        UpdateUI();
    }

    public void SelectMeans(string evidence, string means)
    {
        selectedMeans = means;
        meansEvidence = evidence;
        meansSelectionMenu.SetActive(false);
        UpdateUI();
    }

    public void SelectMotive(string evidence, string motive)
    {
        motiveEvidence = evidence;
        selectedMotive = motive;
        motiveSelectionMenu.SetActive(false);
        UpdateUI();
    }

}

