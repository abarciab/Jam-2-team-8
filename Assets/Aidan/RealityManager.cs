using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.Serialization;
using Unity.VisualScripting;
using UnityEngine;

[ExecuteAlways]
public class RealityManager : MonoBehaviour
{
    [System.Serializable]
    public class CardDrawEvent
    {
        public string card;
        public string character;
    }
    [System.Serializable]
    public class VariantEntry
    {
        public string name;
        public string variant;
    }


    public static RealityManager instance;

    public List<CardDrawEvent> cardHistory = new List<CardDrawEvent>();
    public List<string> previousRealities = new List<string>();

    public List<RealityData> allRealities = new List<RealityData>();
    public RealityData baseReality = new RealityData();
    public RealityData currentReality = new RealityData();

    public List<VariantEntry> activeVariants = new List<VariantEntry>();
    public List<string> allCharacters = new List<string>();


    [Header("testing")]
    public string testRecipient;
    public bool testGetValidCards;

    private void Awake()
    {
        instance = this;
        JSONParser.onStoryDataRefresh += InitializeReality;
    }

    private void Start()
    {
        
    }

    RealityData getReality(string _name)
    {
        foreach (var reality in allRealities) {
            if (reality.name.ToLower() == _name.ToLower()) {
                return reality;
            }
        }
        print("error: tried to get reality, but none exist with that name: " + _name);
        return null;
    }

    void InitializeReality()
    {
        allRealities = JSONParser.instance.storyData;
        baseReality = getReality("core reality");
        currentReality = baseReality;
        updateListOfCharacters();
    }

    void updateListOfCharacters()
    {
        foreach (var character in currentReality.characters) {
            if (!allCharacters.Contains(character.characterName)) {
                allCharacters.Add(character.characterName);
            }
        }
        foreach (var character in baseReality.characters) {
            if (!allCharacters.Contains(character.characterName)) {
                allCharacters.Add(character.characterName);
            }
        }
    }

    //use this function to get all the lines for a certain character in a certain reality. if that character isn't present in the current reality, it returns that character from the base reality
    public CharacterDialogueData getCharacterDialogue(string characterName, RealityData realityToCheck = null)
    {
        if (realityToCheck == null) { realityToCheck = currentReality; }

        List<CharacterDialogueData> matches = new List<CharacterDialogueData>();
        foreach (var character in realityToCheck.characters) {
            if (character.characterName == characterName) {
                matches.Add(character);
            }
        }
        if (matches.Count == 1) {
            return matches[0];
        }
        else if (matches.Count > 1) {
            bool variantSelected = false;
            for (int i = 0; i < activeVariants.Count; i++) {
                if (activeVariants[i].name == characterName) {
                    variantSelected = true;
                    while (matches[0].variant != activeVariants[i].variant && matches.Count > 1) {
                        matches.RemoveAt(0);
                    }
                    if (matches.Count == 1) {
                        return matches[0];
                    }
                }
            }
            if (!variantSelected) {
                for (int i = 0; i < matches.Count; i++) {
                    if (matches[i].variant == "") {
                        return matches[i];
                    }
                }
            }
            print("there were initially multiple matches, but I eliminated all of them :(");
            return null;
        }
        else if (realityToCheck != baseReality) {
            return getCharacterDialogue(characterName, baseReality);
        }
        else {
            print("character dialogue was requested, but there's no one by that name: " + characterName);
            return null;
        }
    }

    public bool CardsAvalible(string character)
    {
        return false;
    }

    public void drawCard(string character)
    {
        //update cardhistory
    }

    void shiftToNewReality(string realityName)
    {
        //update currentReality
        //if a variant of the same name exists in the new universe, or there is no one by that name in the new universe and a variant by that name exists in the base universe, keep that variant data. othewise, delete it
        //update previousRealities
    }

    string getActiveVariantForCharacter(string characterName)
    {
        foreach (var variant in activeVariants) {
            if (variant.name == characterName) {
                return variant.variant;
            }
        }
        return default;
    }

    void Update()
    {

        if (testGetValidCards) {
            if (currentReality == default) { InitializeReality();  }

            testGetValidCards = false;
            List<CardData> validCards = getValidCards(testRecipient);
            string cardNames = "";
            foreach (var card in validCards) {
                cardNames += card.cardName + ", ";
            }
            print("valid cards: " + cardNames);
        }
    }

    CharacterDialogueData GetVariantFromOtherReality (string characterName, string variant) {
        //check through all realities and return a the variant that can exist in the current or the base reality
        foreach (var reality in allRealities) {
            foreach (var character in reality.characters) {
                if (character.characterName == characterName && character.variant == variant) {
                    if (character.validRealities.Contains(currentReality.name) || character.validRealities.Contains(baseReality.name)) {
                        return character;
                    }
                }
            }
        }
        return null;
    }

    List<CardData> getValidCards(string characterName)
    {
        //print("get valid cards");

        List<CardData> validCards = JSONParser.instance.cardData;
        List<CardData> invalidCards = new List<CardData>();

        //what is the currently active variant of this character
        string variant = getActiveVariantForCharacter(characterName);

        //go through all the cards we have loaded
        foreach (var cardData in validCards) {
            //print("considering: " + cardData.cardName);
            //make sure that this character can recieve this card
            CardEffectData effectUnderConsideration = null;
            foreach (var effect in cardData.cardEffects) {
                if (effect.recipient == characterName && effect.variant == variant) {
                    effectUnderConsideration = effect;
                }
            }
            if (effectUnderConsideration == null) {
                //print("invalided card case 0: " + cardData.cardName);
                invalidCards.Add(cardData);
                if (cardData != validCards[validCards.Count - 1])
                    continue;
                else
                    break;

            }

            //if it would take us back to a previous reality, it's not valid
            if (previousRealities.Contains(effectUnderConsideration.reality) && currentReality.name != effectUnderConsideration.reality) {
                //print("invalided card case 1: " + cardData.cardName);
                invalidCards.Add(cardData);
                if (cardData != validCards[validCards.Count - 1])
                    continue;
                else
                    break;
            }

            //if there's no entry on this card for this character (or if there's no entry for the active variant), it's not valid
            bool validForThisCharacter = false;
            foreach (var effect in cardData.cardEffects) {
                if (effect.recipient == characterName && effect.variant == variant) {
                    validForThisCharacter = true; 
                    break;
                }
            }
            if (!validForThisCharacter) {
                //print("invalided card case 2: " + cardData.cardName);
                invalidCards.Add(cardData);
                if (cardData != validCards[validCards.Count - 1])
                    continue;
                else
                    break;
            }

            //if there's a current variant that exists that has a conflict with a variant that this card would introduce, it's not valid
            foreach (var _characterName in allCharacters) {
                CharacterDialogueData characterData = getCharacterDialogue(_characterName);
                foreach (var conflict in characterData.conflicts) {
                    foreach (var effect in effectUnderConsideration.characterEffects) {
                        if (conflict.character == effect.characterName && conflict.variant == effect.variant) {
                            //print("invalided card case 3: " + cardData.cardName);
                            invalidCards.Add(cardData);
                        }
                    }
                }
            }
            if (invalidCards.Contains(cardData)) {
                if (cardData != validCards[validCards.Count - 1])
                    continue;
                else
                    break;
            }

            //if the recipient variant for the target character doesn't exist in this reality, it's not valid
            bool recipientVariantExists = false;
            string recipientVariant = "";
            if (effectUnderConsideration.recipient == characterName) {
                recipientVariant = effectUnderConsideration.variant;
            }
            //print("considerationReality: " + effectUnderConsideration.reality);
            RealityData realityUnderConsideration = (effectUnderConsideration.reality == "") || (effectUnderConsideration.reality == null) ? currentReality : getReality(effectUnderConsideration.reality);
            //print("reality under consideration: " + realityUnderConsideration.name);
            foreach (var character in realityUnderConsideration.characters) {
                if (character.characterName == characterName && (character.variant == recipientVariant || recipientVariant == null)) {
                    recipientVariantExists = true;
                }
            }
            if (!recipientVariantExists) {
                //print("invalided card case 4: " + cardData.cardName);
                invalidCards.Add(cardData);
                if (cardData != validCards[validCards.Count - 1])
                    continue;
                else
                    break;
            }

            //if the target variant for the target character doesn't exist in this reality, it's not valid*
            bool targetVariantExists = false;
            string targetVariant = "";
            foreach (var character in effectUnderConsideration.characterEffects) {
                if (character.characterName == characterName) {
                    targetVariant = character.variant;
                }
            }
            foreach (var character in realityUnderConsideration.characters) {
                if (character.characterName == characterName && (character.variant == recipientVariant || recipientVariant == null)) {
                    targetVariantExists = true;
                }
            }
            if (!targetVariantExists) {
                // * UNLESS the variant exists in another reality and is allowed to exist in the current or base one
                if (GetVariantFromOtherReality(characterName, recipientVariant) == null) {
                    //print("invalided card case 5: " + cardData.cardName);
                    invalidCards.Add(cardData);
                    if (cardData != validCards[validCards.Count - 1])
                        continue;
                    else
                        break;
                }
            }
        }
        /*string _invalidCards = "";
        foreach (var card in invalidCards) {
            _invalidCards += (card.cardName + ", ");
        }*/
        //print("invalid cards: " + _invalidCards);

        return validCards.Except(invalidCards).ToList();
    }

    
}
