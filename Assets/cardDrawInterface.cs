using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class cardDrawInterface : MonoBehaviour
{
    public GameObject drawCardButtonObj;
    public Button drawCardButton;
    public GameObject cardParent;
    public Image cardFront;
    string currentCharacter;

    public void DrawCard()
    {
        RealityManager.CardRef drawnCard = new RealityManager.CardRef();
        if (RealityManager.instance.CardsAvalible(CharacterResponseManager.instance.currentCharacterName)) {
            drawnCard = RealityManager.instance.drawCard(CharacterResponseManager.instance.currentCharacterName);
        }
        else {
            return;
        }
        cardFront.sprite = drawnCard.sprite;
        cardParent.SetActive(true);
    }

    private void Update()
    {
        if (string.IsNullOrEmpty(CharacterResponseManager.instance.currentCharacterName) || CharacterResponseManager.instance.currentCharacterName != currentCharacter) {
            currentCharacter = CharacterResponseManager.instance.currentCharacterName;

            if (string.IsNullOrEmpty(CharacterResponseManager.instance.currentCharacterName) || RealityManager.instance.CardsAvalible(CharacterResponseManager.instance.currentCharacterName)) {
                drawCardButton.enabled = false;
            }
            else {
                drawCardButton.enabled = true;
            }


        }
        
    }

}
