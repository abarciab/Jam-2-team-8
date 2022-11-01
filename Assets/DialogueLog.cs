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

    [Header("misc")]
    public string currentlySelectedCharacter;
    public int currentPage;         //this is a counter for page flips, not for actual pages. so left side + right side is all page 0.
    public float currentTotalPrefferedHeight;  //this heigh gets reset every half-page, and is a sum of all the elements in each layout group so far
    public float currentTotalPrefferedHeightv2;
    public float totalHeightAllowed = 800;
    int currentIndex;
    string currentCharacter;

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
            public string question;
            public string line;
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
        //CreateText(characterLines[0]);
        previousPageButton.SetActive(false);
        nextPageButton.SetActive(false);
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

    void CreateText(RecordedLines characterLines, int _currentPage = 0)
    {
        if (characterLines.speaker != currentCharacter) {
            currentIndex = 0;
            currentPage = 0;
            nextPageButton.SetActive(false);
            previousPageButton.SetActive(false);
        }
        currentCharacter = characterLines.speaker;

        clearRightPage();
        clearLeftPage();

        if (currentPage == 0) {
            var portrait = Instantiate(characterPortraitPrefab, leftPageParent.transform);
            portrait.GetComponent<Image>().sprite = RealityManager.instance.getCharacterPortraitByName(characterLines.speaker);
            portrait.GetComponentInChildren<TextMeshProUGUI>().text = characterLines.speaker;
            currentTotalPrefferedHeight += LayoutUtility.GetPreferredHeight(portrait.GetComponent<RectTransform>());
        }

        
        GameObject currentPageParent = leftPageParent;
        float spacing = currentPageParent.GetComponent<VerticalLayoutGroup>().spacing;
        //foreach (var line in characterLines.lines) {
        for (int i = currentIndex; i < characterLines.lines.Count; i++) {
            var line = characterLines.lines[i];
            currentIndex = i;
            if (currentTotalPrefferedHeight >= totalHeightAllowed && currentPageParent == leftPageParent) { 
                currentPageParent = rightPageParent;
                currentTotalPrefferedHeight = 0;
            }
            else if (currentTotalPrefferedHeight >= totalHeightAllowed && currentPageParent == rightPageParent) {
                nextPageButton.SetActive(true);
                break;
            }

            var question = Instantiate(detectivePrefab, currentPageParent.transform);
            question.GetComponent<TextMeshProUGUI>().text = line.question;
            currentTotalPrefferedHeight += LayoutUtility.GetPreferredHeight(question.GetComponent<RectTransform>());
            

            var answer = Instantiate(suspectDialoguePrefab, currentPageParent.transform);
            answer.GetComponent<TextMeshProUGUI>().text = line.line;
            currentTotalPrefferedHeight += LayoutUtility.GetPreferredHeight(answer.GetComponent<RectTransform>());
            currentTotalPrefferedHeight += spacing * 2;
            LayoutRebuilder.ForceRebuildLayoutImmediate(currentPageParent.GetComponent<RectTransform>());
        }
        

        //line by line, determine if the line belongs to the detective or the suspect
        //based on that, 
    }

    public void NextPage()
    {

    }

    public void previousPage()
    {

    }



    public void RecordDialogue(string dialogueType, CharacterDialogueData character)
    {
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
        newRecordedLine.speaker = speaker;
        
    }

}
