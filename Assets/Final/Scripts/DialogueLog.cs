using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class DialogueLog : MonoBehaviour
{
    [Header("prefabs")]
    public GameObject characterPortraitPrefab;
    public GameObject suspectDialoguePrefab;
    public GameObject detectivePrefab;

    [Header("referenes")]
    public GameObject leftPageParent;
    public GameObject rightPageParent;
    public GameObject nextPageButton;
    public GameObject previousPageButton;
    public GameObject personSelector;
    public GameObject contentGridPersonSelector;

    [Header("questions")]
    public string alibiQuestion;
    public string relationshipQuestion;

    [Header("misc")]
    public string currentlySelectedCharacter;
    public int currentPage;         //this is a counter for page flips, not for actual pages. so left side + right side is all page 0.
    public float currentTotalPrefferedHeight;  //this heigh gets reset every half-page, and is a sum of all the elements in each layout group so far
    public float totalHeightAllowed = 800;
    public float totalHeightAllowedpg0 = 800;
    public float rightPageHandicapp = 550;

    public int currentIndex;
    string currentCharacter;
    RecordedLines currentLines;
    public int currentPageIndexCount;
    Dictionary<int, int> pagesToIndex = new Dictionary<int, int>();
    //public int previousPageIndexCount;

    public static Action onDialogueLineAdded;

    [Header("testing")]
    public bool testCreateText;

    [System.Serializable]
    public class RecordedLines
    {
        public string speaker;
        public List<lineDetails> lines = new List<lineDetails>();

        [System.Serializable]
        public class lineDetails
        {
            public DialogueLineData lineData;
            public string question;
            public string line;
            public string dialogueType;
        }
    }

    public List<RecordedLines> characterLines = new List<RecordedLines>();

    private void Update()
    {
        if (testCreateText) {
            testCreateText = false;
            CreateText(characterLines[0]);
        }
    }

    private void OnEnable()
    {
        for (int i = 0; i < contentGridPersonSelector.transform.childCount; i++) {
            contentGridPersonSelector.transform.GetChild(i).gameObject.SetActive(false);
            for (int j = 0; j < characterLines.Count; j++) {
                if (contentGridPersonSelector.transform.GetChild(i).name == characterLines[j].speaker) {
                    contentGridPersonSelector.transform.GetChild(i).gameObject.SetActive(true);
                }
            }
        }
        personSelector.SetActive(true);
        previousPageButton.SetActive(false);
        nextPageButton.SetActive(false);
        //print("enanbled");
        clearLeftPage();
        clearRightPage();
    }

    public void SelectPerson(string name)
    {
        currentIndex = 0;
        currentPage = 0;
        for (int i = 0; i < characterLines.Count; i++) {
            if (characterLines[i].speaker == name) {
                CreateText(characterLines[i]);
            }
        }
    }

    void clearLeftPage()
    {
        for (int i = 0; i < leftPageParent.transform.childCount; i++) {
            Destroy(leftPageParent.transform.GetChild(i).gameObject);
        }
    }

    void clearRightPage()
    {
        for (int i = 0; i < rightPageParent.transform.childCount; i++) {
            Destroy(rightPageParent.transform.GetChild(i).gameObject);
        }
    }

    void CreateText(RecordedLines characterLines = null)
    {
        if (characterLines == null) {
            characterLines = currentLines;
        }
        currentLines = characterLines;

        if (characterLines.speaker != currentCharacter) {
            currentIndex = 0;
            currentPage = 0;
            nextPageButton.SetActive(false);
            previousPageButton.SetActive(false);
            pagesToIndex = new Dictionary<int, int>();
        }
        currentCharacter = characterLines.speaker;

        clearRightPage();
        clearLeftPage();

        currentTotalPrefferedHeight = 0;
        if (currentPage == 0) {
            var portrait = Instantiate(characterPortraitPrefab, leftPageParent.transform);
            portrait.GetComponent<Image>().sprite = RealityManager.instance.getCharacterPortraitByName(characterLines.speaker);
            portrait.GetComponentInChildren<TextMeshProUGUI>().text = characterLines.speaker;
            currentTotalPrefferedHeight += LayoutUtility.GetPreferredHeight(portrait.GetComponent<RectTransform>());
        }

        currentPageIndexCount = 0;
        GameObject currentPageParent = leftPageParent;
        float spacing = currentPageParent.GetComponent<VerticalLayoutGroup>().spacing;

        for (int i = currentIndex; i < characterLines.lines.Count; i++) {
            var line = characterLines.lines[i];

            var question = Instantiate(detectivePrefab, currentPageParent.transform);
            question.GetComponent<TextMeshProUGUI>().text = line.question;
            currentTotalPrefferedHeight += LayoutUtility.GetPreferredHeight(question.GetComponent<RectTransform>());

            var answer = Instantiate(suspectDialoguePrefab, currentPageParent.transform);
            answer.GetComponent<TextMeshProUGUI>().text = line.line;
            currentTotalPrefferedHeight += LayoutUtility.GetPreferredHeight(answer.GetComponent<RectTransform>());
            
            currentTotalPrefferedHeight += spacing * 2;
            LayoutRebuilder.ForceRebuildLayoutImmediate(currentPageParent.GetComponent<RectTransform>());
            currentPageIndexCount += 1;
            currentIndex += 1;

            if (currentTotalPrefferedHeight >= (currentPage == 0 ? totalHeightAllowedpg0 : totalHeightAllowed) && currentPageParent == leftPageParent) {
                currentPageParent = rightPageParent;
                currentTotalPrefferedHeight = (currentPage == 0) ? rightPageHandicapp : 0;
                nextPageButton.SetActive(false);
            }
            else if (currentTotalPrefferedHeight >= (currentPage == 0 ? totalHeightAllowedpg0 : totalHeightAllowed) && currentPageParent == rightPageParent) {
                break;
            }
        }

        if (!pagesToIndex.ContainsKey(currentPage))
            pagesToIndex.Add(currentPage, currentPageIndexCount);

        if (characterLines.lines.Count > currentIndex) {
            nextPageButton.SetActive(true);
        }
        else {
            nextPageButton.SetActive(false);
        }

        if (currentPage > 0) {
            previousPageButton.SetActive(true);
        }
        else {
            previousPageButton.SetActive(false);
        }
    }

    public void NextPage()
    {
        if (!gameObject.activeInHierarchy) { return; }
        clearLeftPage();
        clearRightPage();
        currentTotalPrefferedHeight = 0;
        currentPage += 1;
        CreateText();
    }

    public void previousPage()
    {
        if (!gameObject.activeInHierarchy) { return; }
        clearLeftPage();
        clearRightPage();
        currentTotalPrefferedHeight = 0;
        currentIndex -= (currentPageIndexCount + pagesToIndex[ (currentPage-1) ]);
        currentPage -= 1;
        CreateText();
    }



    public void RecordDialogue(string dialogueType, CharacterDialogueData character)
    {
        //print("added dialog");
        switch (dialogueType) {
            case "alibi":
                JournalManager.instance.dialogueLog.RecordDialogue(character.alibi, character.characterName, alibi: true);
                break;
            case "relationship":
                JournalManager.instance.dialogueLog.RecordDialogue(character.relationship, character.characterName, relationship: true);
                break;
            default:
                foreach (var response in character.evidenceResponses) {
                    if (response.item == dialogueType) {
                        JournalManager.instance.dialogueLog.RecordDialogue(response.line, character.characterName, evidence: dialogueType);
                    }
                }
                break;
        }
    }

    public void RecordDialogue(DialogueLineData line, string speaker, bool alibi = false, bool relationship = false, string evidence = null)
    {
        var newRecordedLine = new RecordedLines();
        foreach (var _speaker in characterLines) {
            if (_speaker.speaker == speaker) {
                
                for (int i = 0; i < _speaker.lines.Count; i++) {
                    if (_speaker.lines[i].question == alibiQuestion && alibi) {
                        _speaker.lines.RemoveAt(i);
                        break;
                    }
                    else if (_speaker.lines[i].question == relationshipQuestion && relationship) {
                        _speaker.lines.RemoveAt(i);
                        break;
                    }
                    else if (!string.IsNullOrEmpty(evidence) && _speaker.lines[i].question.Contains(evidence)) {
                        _speaker.lines.RemoveAt(i);
                        break;
                    }
                }
                newRecordedLine = _speaker;
            }
        }


        newRecordedLine.speaker = speaker;
        RecordedLines.lineDetails newLine = new RecordedLines.lineDetails();
        if (alibi) {
            newLine.question = alibiQuestion;
            newLine.dialogueType = "alibi";
        }
        else if (relationship) {
            newLine.question = relationshipQuestion;
            newLine.dialogueType = "relationship";
        }
        else if (!string.IsNullOrEmpty(evidence)) {
            newLine.question = "I " + "[VERB] " + speaker  + " ARTICLE " + evidence;
        }
        newLine.line = line.text;
        newLine.lineData = line;
        newRecordedLine.lines.Add(newLine);

        for (int i = 0; i < characterLines.Count; i++) {
            if (characterLines[i].speaker == speaker) {
                characterLines[i] = newRecordedLine;
                return;
            }
        }
        characterLines.Add(newRecordedLine);
    }

}
