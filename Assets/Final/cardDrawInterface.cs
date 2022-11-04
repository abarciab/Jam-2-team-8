using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class cardDrawInterface : MonoBehaviour
{
    public GameObject drawCardButtonObj;
    public GameObject drawCardText;
    public GameObject cardsunavaliableText;
    public GameObject cardHolder;
    public GameObject cardParent;
    public Image cardFront;
    string currentCharacter;

    public void DrawCard()
    {
        
        RealityManager.CardRef drawnCard = RealityManager.instance.drawCard(CharacterResponseManager.instance.currentCharacterName);
        //print("drawcard");
        if (drawnCard == null) {
            print("null??");
            return;
        }
        cardFront.sprite = drawnCard.sprite;
        cardFront.GetComponentInChildren<TextMeshProUGUI>().text = drawnCard.cardName;
        cardParent.SetActive(true);
        cardHolder.SetActive(false);
    }

    private void Update()
    {
        //print("valid cards: " + RealityManager.instance.NumValidCards(CharacterResponseManager.instance.currentCharacterName));

        cardsunavaliableText.SetActive(true);
        drawCardText.SetActive(false);
        drawCardButtonObj.SetActive(false);
        if (RealityManager.instance.NumValidCards(CharacterResponseManager.instance.currentCharacterName) > 0) {
            cardsunavaliableText.SetActive(false);
            drawCardText.SetActive(true);
            drawCardButtonObj.SetActive(true);
        }      
    }

}
