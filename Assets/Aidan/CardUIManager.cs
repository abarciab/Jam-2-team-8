using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class CardUIManager : MonoBehaviour
{
    public Image drawnCard;
    public Image drawCardButton;
    public TextMeshProUGUI validCards;
    public TextMeshProUGUI reality;
    public TextMeshProUGUI characterName;
    public TextMeshProUGUI variant;
    public TextMeshProUGUI alibi;
    public TextMeshProUGUI relationship;
    public TextMeshProUGUI evidence1;
    public TextMeshProUGUI evidence2;
    public TextMeshProUGUI evidence3;

    public string currentCharacterName = "john";
    public int characterIndex = 0;

    private void Start()
    {
        UpdateUI();
    }

    void UpdateUI()
    {
        //print("updating UI. character: " + currentCharacterName);

        CharacterDialogueData character = RealityManager.instance.getCharacterDialogue(currentCharacterName);

        int numValidCards = RealityManager.instance.NumValidCards(currentCharacterName);
        validCards.text = "Valid cards to draw: " + numValidCards;
        if (numValidCards == 0) {
            drawCardButton.color = new Color(drawCardButton.color.r, drawCardButton.color.g, drawCardButton.color.b, 0.5f);
        }
        else {
            drawCardButton.color = new Color(drawCardButton.color.r, drawCardButton.color.g, drawCardButton.color.b, 1f);
        }

        reality.text = "Reality: " + RealityManager.instance.currentReality.name;
        
        if (string.IsNullOrEmpty(character.variant))
            variant.gameObject.SetActive(false);

        else
            variant.gameObject.SetActive(true);
        characterName.text = "Name: " + currentCharacterName;
        variant.text = "variant: " + character.variant;
        
        alibi.text = "alibi: " + character.alibi.text;
        relationship.text = "relationship to the victim: " + character.relationship.text;

        evidence1.gameObject.SetActive(false);
        evidence2.gameObject.SetActive(false);
        evidence3.gameObject.SetActive(false);
        if (character.evidenceResponses.Count > 0) {
            evidence1.gameObject.SetActive(true);
            evidence1.text = "when shown <" + character.evidenceResponses[0].item + ">: '" + character.evidenceResponses[0].line.text + "'";
        }
        if (character.evidenceResponses.Count > 1) {
            evidence2.gameObject.SetActive(true);
            evidence2.text = "when shown <" + character.evidenceResponses[1].item + ">: '" + character.evidenceResponses[1].line.text + "'";
        }
        if (character.evidenceResponses.Count > 2) {
            evidence3.gameObject.SetActive(true);
            evidence3.text = "when shown <" + character.evidenceResponses[2].item + ">: '" + character.evidenceResponses[2].line.text + "'";
        }
    }

    public void DrawCard()
    {
        if (RealityManager.instance.NumValidCards(currentCharacterName) == 0) { 
            return;  
        }

        drawnCard.gameObject.SetActive(false);
        RealityManager.CardRef newCard = RealityManager.instance.drawCard(currentCharacterName);
        UpdateUI();
        drawnCard.sprite = newCard.sprite;
        drawnCard.gameObject.SetActive(true);
    }

    public void NextCharacter()
    {
        characterIndex += 1;
        if (characterIndex >= RealityManager.instance.allCharacters.Count) {
            characterIndex = 0;
        }
        currentCharacterName = RealityManager.instance.allCharacters[characterIndex];
        UpdateUI();
        drawnCard.gameObject.SetActive(false);        
    }

    public void PreviousCharacter()
    {
        characterIndex -= 1;
        if (characterIndex <= 0) {
            characterIndex = RealityManager.instance.allCharacters.Count - 1;
        }
        currentCharacterName = RealityManager.instance.allCharacters[characterIndex];
        UpdateUI();
        drawnCard.gameObject.SetActive(false);
    }



}
