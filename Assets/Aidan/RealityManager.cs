using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.Serialization;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TextCore.Text;

[ExecuteAlways]
public class RealityManager : MonoBehaviour
{
    [System.Serializable]
    public class characterData
    {
        public string characterName;
        public Sprite characterPortrait;
        public string pronoun = "he";
    }

    //classes
    [System.Serializable]
    public class CardDrawEvent
    {
        public string card;
        public string character;
        public string variant;
    }
    [System.Serializable]
    public class VariantEntry
    {
        public string name;
        public string variant;
    }
    [System.Serializable]
    public class CardRef
    {
        public string cardName;
        public string description;
        public Sprite sprite;
    }

    public static RealityManager instance;

    [Header("Realities")]
    public List<RealityData> allRealities = new List<RealityData>();
    public List<string> previousRealities = new List<string>();
    public RealityData baseReality = new RealityData();
    public RealityData currentReality = new RealityData();
    public static Action onRealityRefresh;

    [Header("character Data")]
    public List<VariantEntry> activeVariants = new List<VariantEntry>();
    public List<characterData> allCharacters = new List<characterData>();

    [Header("Card Data")]
    public List<CardData> allCards = new List<CardData>();
    public List<CardDrawEvent> cardHistory = new List<CardDrawEvent>();
    public List<CardRef> cards = new List<CardRef>();


    [Header("testing")]
    public string testRecipient;
    public string testCard;
    public bool testDrawCard;
    public bool testGetValidCards;

    private void Awake()
    {
        instance = this;
        JSONParser.onStoryDataRefresh += InitializeReality;
    }

    RealityData getRealityByName(string _name)
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
        allCards = JSONParser.instance.cardData;
        baseReality = getRealityByName("core reality");
        currentReality = baseReality;
        updateListOfCharacters();
    }

    bool characterInAllCharacters (string characterName)
    {
        foreach (var character in allCharacters) {
            if (character.characterName == characterName) {
                return true;
            }
        }
        return false;
    }

    public Sprite getCharacterPortraitByName(string characterName)
    {
        foreach (var character in allCharacters) {
            if (character.characterName == characterName) {
                return character.characterPortrait;
            }
        }
        print("no characters found with that name: " + characterName);
        return null;
    }

    public string getCharacterPronounByName(string characterName)
    {
        foreach (var character in allCharacters) {
            if (character.characterName == characterName) {
                return character.pronoun;
            }
        }
        print("no characters found with that name: " + characterName);
        return null;
    }

    void updateListOfCharacters()
    {
        //allCharacters.Clear();
        foreach (var character in currentReality.characters) {
            if (!characterInAllCharacters(character.characterName)) {
                var newCharacter = new characterData();
                newCharacter.characterName = character.characterName;
                allCharacters.Add(newCharacter);
            }
        }
        foreach (var character in baseReality.characters) {
            if (!characterInAllCharacters(character.characterName)) {
                var newCharacter = new characterData();
                newCharacter.characterName = character.characterName;
                allCharacters.Add(newCharacter);
            }
        }
    }

    //use this function to get all the lines for a certain character in a certain reality. if that character isn't present in the current reality, it returns that character from the base reality
    public CharacterDialogueData getCharacterDialogue(string characterName, RealityData realityToCheck = null)
    {
        print("trying to get data for character: " + characterName);
        bool showDebugInfo = true;

        if (currentReality == default) { InitializeReality();  }

        List<CharacterDialogueData> matches = new List<CharacterDialogueData>();
        foreach (var reality in allRealities) {
            foreach (var character in reality.characters) {
                if (character.characterName == characterName && (character.validRealities.Contains(currentReality.name) || character.validRealities.Contains(baseReality.name))) {
                    matches.Add(character);
                }
            }
        }
       
        if (matches.Count == 1) {
            return matches[0];
        }
        else if (matches.Count > 1) {
            if (showDebugInfo) {
                print("found multiple matches: ");
                foreach (var match in matches) {
                    print(match.characterName + ": " + match.variant);
                }
                print("\n");
            }

            bool variantSelected = false;
            for (int i = 0; i < activeVariants.Count; i++) {
                if (activeVariants[i].name == characterName) {
                    variantSelected = true;
                    while (matches[0].variant != activeVariants[i].variant && matches.Count > 1) {
                        print("we're looking for variant: " + activeVariants[i].variant + " for " + activeVariants[i].name + ", and this is variant " + matches[0].variant + " for " + matches[0].characterName + ", eliminating...");
                        matches.RemoveAt(0);
                    }
                    if (matches[0].variant == activeVariants[i].variant) {
                        print("found variant: " + activeVariants[i].variant + " for character: " + activeVariants[i].name);
                        return matches[0];
                    }
                }
            }
            if (!variantSelected) {
                for (int i = 0; i < matches.Count; i++) {
                    if (string.IsNullOrEmpty(matches[i].variant)) {
                        return matches[i];
                    }
                }
            }
            print("there were initially multiple matches, but I eliminated all of them.");
            return null;
        }
        else if (realityToCheck != baseReality) {
            print("does this ever happen?");
            return getCharacterDialogue(characterName, baseReality);
        }
        else {
            print("character dialogue was requested, but there's no one by that name: " + characterName);
            return null;
        }
    }

    public void ActivateVariant(string characterName, string variant)
    {
        
        if (GetVariantFromOtherReality(characterName, variant) != null) {
            VariantEntry newVariant = new VariantEntry();
            newVariant.name = characterName;
            newVariant.variant = variant;
            activeVariants.Add(newVariant);
            print("Activated " + variant + " variant of " + " " + characterName + " manually. be careful, as this method doesn't take into account conflicts with already existing variants, so dialogue might not align completely");
        }
        else {
            print("could not add " + variant + " variant of " + " " + characterName + ", as no variants by that name are allowed to exist in the current or base reality.");
        }
    }

    public void DeactivateVariant(string characterName, string variant)
    {
        List<VariantEntry> toRemove = new List<VariantEntry>();
        foreach (var _variant in activeVariants) {
            if (_variant.name == characterName && _variant.variant == variant) {
                toRemove.Add(_variant);
            }
        }
        foreach (var _variant in toRemove) {
            activeVariants.Remove(_variant);
            print("variant deactivated");
        }
    }

    public bool CardsAvalible(string characterName)
    {
        return getValidCards(characterName).Count != 0; 
    }

    //don't use this function just to check if there are any valid cards - use CardsAvaliable() to do that
    public int NumValidCards(string characterName)
    {
        if (characterInAllCharacters(characterName))
            return getValidCards(characterName).Count;
        else {
            return 0;
        }
    }

    public CardRef drawCard(string characterName)
    {
        //select the card from the valid option
        List<CardData> validCards = getValidCards(characterName);
        if (validCards.Count == 0) {
            print("Tried to draw a card without checking if there were valid cards avaliable first. next time check with the CardsAvalible function");
            return null;
        }
        CardData selectedCard = validCards[UnityEngine.Random.Range(0, validCards.Count)];

        return DrawCardManually(characterName, selectedCard.cardName);
    }

    CardRef DrawCardManually(string characterName, string cardName)
    {
        CardData selectedCard = null;
        foreach (var card in allCards) {
            if (card.cardName == cardName) {
                selectedCard = card;
            }
        }

        CardEffectData selectedCardEffect = null;
        string activeVariant = getActiveVariantForCharacter(characterName);
        foreach (var effect in selectedCard.cardEffects) {
            if (effect.recipient == characterName && (effect.variant == activeVariant || (string.IsNullOrEmpty(activeVariant) && string.IsNullOrEmpty(effect.variant)) || (!string.IsNullOrEmpty(effect.variant) && effect.variant.ToLower() == "any") )) {
                selectedCardEffect = effect;
            }
        }

        if (selectedCardEffect == null) {
            print("Tried to draw a card without checking if there were valid cards avaliable first. next time check with the CardsAvalible function");
            return null;
        }

        //shift reality, deactivate old variants, and activate valid new ones
        if (!string.IsNullOrEmpty(selectedCardEffect.reality)) {
            shiftToNewReality(selectedCardEffect.reality);
        }
        foreach (var characterEffect in selectedCardEffect.characterEffects) {
            if (CheckVariantExistsInReality(characterEffect.characterName, characterEffect.variant, currentReality.name)) {
                VariantEntry newVariantEntry = new VariantEntry();
                newVariantEntry.name = characterEffect.characterName;
                newVariantEntry.variant = characterEffect.variant;
                activeVariants.Add(newVariantEntry);
            }
        }

        //update card draw history
        CardDrawEvent newCardDraw = new CardDrawEvent();
        newCardDraw.card = selectedCard.cardName;
        newCardDraw.character = selectedCardEffect.recipient;
        newCardDraw.variant = selectedCardEffect.variant;
        cardHistory.Add(newCardDraw);

        return GetRefByCardName(selectedCard.cardName);
    }

    CardRef GetRefByCardName(string cardName)
    {
        foreach (var cardRef in cards) {
            if (cardRef.cardName.ToLower() == cardName.ToLower()) {
                return cardRef;
            }
            else {
                print(cardRef.cardName.ToLower() + " != " + cardName.ToLower());
            }
        }
        print("tried to get cardRef with invalid name: " + cardName);
        return null;
    }

    void shiftToNewReality(string newReality)
    {
        print("shifting to new reality: " + newReality);
        previousRealities.Add(currentReality.name);
        currentReality = getRealityByName(newReality);
        List<VariantEntry> variantsToDeactivate = new List<VariantEntry>();
        foreach (var variant in activeVariants) {
            if (GetVariantFromOtherReality(variant.name, variant.variant) == null) {
                variantsToDeactivate.Add(variant);
            }
        }
        activeVariants = activeVariants.Except(variantsToDeactivate).ToList();
        updateListOfCharacters();

        if (onRealityRefresh != null) {
            onRealityRefresh();
        }
    }

    bool CheckVariantExistsInReality(string characterName, string variant, string realityName, bool includeBase = true)
    {
        RealityData realityToCheck = getRealityByName(realityName);
        foreach (var character in realityToCheck.characters) {
            if (character.characterName.ToLower() == characterName.ToLower() && ( (string.IsNullOrEmpty(variant) && string.IsNullOrEmpty(variant)) || (!string.IsNullOrEmpty(character.variant) && !string.IsNullOrEmpty(variant) && (character.variant.ToLower() == variant.ToLower())) )) {
                return true;
            }
        }
        if (realityName != baseReality.name && includeBase) {
            return CheckVariantExistsInReality(characterName, variant, baseReality.name, false);
        }

        return false;
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
        if (currentReality == default) { InitializeReality(); }

        if (testDrawCard) {
            testDrawCard = false;
            DrawCardManually(testRecipient, testCard);
        }

        if (testGetValidCards) {
            testGetValidCards = false;
            print("there are " + NumValidCards(testRecipient) + " valid cards: ");
        }
    }

    CharacterDialogueData GetVariantFromOtherReality (string characterName, string variant) {
        //check through all realities and return a the variant that can exist in the current or the base reality
        foreach (var reality in allRealities) {
            foreach (var character in reality.characters) {
                if (character.characterName == characterName && (character.variant == variant|| (string.IsNullOrEmpty(variant) && string.IsNullOrEmpty(character.variant) )) ) {
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
        bool displayDebugInfo = false;
        //print("get valid cards");

        List<CardData> validCards = JSONParser.instance.cardData;
        List<CardData> invalidCards = new List<CardData>();

        //what is the currently active variant of this character
        string variant = getActiveVariantForCharacter(characterName);

        //go through all the cards we have loaded
        foreach (var cardData in validCards) {
            if (displayDebugInfo) { print("considering: " + cardData.cardName); }
            //make sure that this character can recieve this card
            CardEffectData effectUnderConsideration = null;
            foreach (var effect in cardData.cardEffects) {
                if (effect.recipient == characterName && ((string.IsNullOrEmpty(effect.variant) && string.IsNullOrEmpty(variant)) || (!string.IsNullOrEmpty(effect.variant) && effect.variant.ToLower() == "any") || effect.variant == variant )  ) {
                    effectUnderConsideration = effect;
                }
            }
            if (effectUnderConsideration == null) {
                if (displayDebugInfo) 
                    print("invalided card case 0: " + cardData.cardName);
                invalidCards.Add(cardData);
                if (cardData != validCards[validCards.Count - 1])
                    continue;
                else
                    break;

            }

            //if it would take us back to a previous reality, it's not valid
            if (previousRealities.Contains(effectUnderConsideration.reality) && currentReality.name != effectUnderConsideration.reality) {
                if (displayDebugInfo)
                    print("invalided card case 1: " + cardData.cardName);
                invalidCards.Add(cardData);
                if (cardData != validCards[validCards.Count - 1])
                    continue;
                else
                    break;
            }

            //if there's no entry on this card for this character (or if there's no entry for the active variant), it's not valid
            bool validForThisCharacter = false;
            foreach (var effect in cardData.cardEffects) {
                if (effect.recipient == characterName && ((string.IsNullOrEmpty(effect.variant) && string.IsNullOrEmpty(variant)) || (!string.IsNullOrEmpty(effect.variant) && effect.variant.ToLower() == "any") || effect.variant == variant)) {
                    validForThisCharacter = true; 
                    break;
                }
            }
            if (!validForThisCharacter) {
                if (displayDebugInfo)
                    print("invalided card case 2: " + cardData.cardName);
                invalidCards.Add(cardData);
                if (cardData != validCards[validCards.Count - 1])
                    continue;
                else
                    break;
            }

            //if there's a current variant that exists that has a conflict with a variant that this card would introduce, it's not valid
            foreach (var _character in allCharacters) {
                CharacterDialogueData characterData = getCharacterDialogue(_character.characterName);
                foreach (var conflict in characterData.conflicts) {
                    foreach (var effect in effectUnderConsideration.characterEffects) {
                        if (conflict.character == effect.characterName && conflict.variant == effect.variant) {
                            if (displayDebugInfo)
                                print("invalided card case 3: " + cardData.cardName);
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

            //if this card would have no effect on the character it's being drawn for, it's not valid
            string postCardVariant = null;  //the variant that the recipient will have after the card is drawn
            foreach (var characterEffect in effectUnderConsideration.characterEffects) {
                if (characterEffect.characterName == effectUnderConsideration.recipient) {
                    postCardVariant = characterEffect.variant;
                    if (postCardVariant == getActiveVariantForCharacter(effectUnderConsideration.recipient)) {
                        invalidCards.Add(cardData);
                    }
                }
            }
            if (postCardVariant == null) {
                if (effectUnderConsideration.reality == currentReality.name) {
                    invalidCards.Add(cardData);
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
            RealityData realityUnderConsideration = (effectUnderConsideration.reality == "") || (effectUnderConsideration.reality == null) ? currentReality : getRealityByName(effectUnderConsideration.reality);
            //print("reality under consideration: " + realityUnderConsideration.name);
            foreach (var character in realityUnderConsideration.characters) {
                if (character.characterName == characterName && ((string.IsNullOrEmpty(recipientVariant) && string.IsNullOrEmpty(character.variant)) || (!string.IsNullOrEmpty(recipientVariant) && recipientVariant.ToLower() == "any") || character.variant == recipientVariant)) {
                    recipientVariantExists = true;
                }
            }
            if (!recipientVariantExists) {
                if (displayDebugInfo)
                    print("invalided card case 4: " + cardData.cardName);
                invalidCards.Add(cardData);
                if (cardData != validCards[validCards.Count - 1])
                    continue;
                else
                    break;
            }

            //if the active variant for the target character doesn't match the variant that this card is for, it's not valid
            foreach (var activeVariant in activeVariants) {
                if (activeVariant.name == characterName && effectUnderConsideration.variant.ToLower() != "any") {
                    if (activeVariant.variant.ToLower() != effectUnderConsideration.variant.ToLower()) {
                        if (displayDebugInfo)
                            print("invalided card case 4.5: " + cardData.cardName);
                        invalidCards.Add(cardData);
                        if (cardData != validCards[validCards.Count - 1])
                            continue;
                        else
                            break;
                    }
                }
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
                if (character.characterName == characterName && ((string.IsNullOrEmpty(recipientVariant) && string.IsNullOrEmpty(character.variant)) || (!string.IsNullOrEmpty(recipientVariant) && recipientVariant.ToLower() == "any") || character.variant == recipientVariant) ) {
                    targetVariantExists = true;
                }
            }
            if (!targetVariantExists) {
                // * UNLESS the variant exists in another reality and is allowed to exist in the current or base one
                if (GetVariantFromOtherReality(characterName, recipientVariant) == null) {
                    if (displayDebugInfo)
                        print("invalided card case 5: " + cardData.cardName);
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
