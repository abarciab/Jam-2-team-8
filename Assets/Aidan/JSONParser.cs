using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using Newtonsoft.Json;
using Unity.VisualScripting;
using System.IO;
using System;
using System.Data;
using JetBrains.Annotations;
using UnityEngine.Rendering;

//classes for card data
[System.Serializable]
public class CardData {
    public string cardName;         //what card this is
    public List<CardEffectData> cardEffects = new List<CardEffectData>();
}
[System.Serializable]
public class CardEffectData {
    public string recipient;        //this data is about the situation in which this card is shown to this character
    public string variant;          //if not blank, the character must be in this variant to recieve this effect
    public string reality;         //which reality to shift to. NOTE: this shift is performed before character variants are selected
    public List<CharacterCardEffectData> characterEffects = new List<CharacterCardEffectData>();        //which variants are active after the shift
}
[System.Serializable]
public class CharacterCardEffectData {
    public string characterName;
    public string variant;
}


//classes for dialogue data
[System.Serializable]
public class DialogueLineData  {
    public string text;
    public bool lie;
    public string evidenceGained;
    public string requiredEvidence;
    public string truth;
}
[System.Serializable]
public class EvidenceResponseData {
    public string item;
    public DialogueLineData line;
}
[System.Serializable]
public class variantConflict {
    public string character;
    public string variant;
}
[System.Serializable]
public class CharacterDialogueData {

    public string characterName;
    public string variant;
    public List<variantConflict> conflicts = new List<variantConflict>();         //a variant conflict is a rule about card drawing. When one of these conflicts is active, and a drawn card wouldn't change that, but it would activate this variant, that card cannot be drawn
    public DialogueLineData defaultResponse;
    public DialogueLineData alibi;
    public DialogueLineData relationship;
    public List<EvidenceResponseData> evidenceResponses = new List<EvidenceResponseData>();
}
[System.Serializable]
public class RealityData {
    public string name;
    public List<CharacterDialogueData> characters = new List<CharacterDialogueData>();
}

[ExecuteAlways]
public class JSONParser : MonoBehaviour
{
    public static JSONParser instance;
    
    public bool parseDialogue = false;
    public bool parseCards = false;
    public string JSONDialogueFilePath = "Assets/Aidan/testJSON.txt";
    public string JSONCardDataFilePath = "Assets/Aidan/cardJSON.txt";

    public List<RealityData> storyData = new List<RealityData>();
    public List<CardData> cardData = new List<CardData>();

    public static Action onStoryDataRefresh;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        ParseDialogueData();
        ParseCardData();
    }

    private void Update()
    {
        if (parseDialogue) {
            parseDialogue = false;
            ParseDialogueData();
        }
        if (parseCards) {
            parseCards = false;
            ParseCardData();
        }
    }

    public string GetSubstringByString(string a, string b, string c)
    {
        if (a.Length <= 0 || b.Length <= 0 || c.Length <= 0 || !c.Contains(a) || !c.Contains(b) ) { return ""; }
        return c.Substring((c.IndexOf(a) + a.Length), (c.IndexOf(b) - c.IndexOf(a) - a.Length));
    }

    void ParseCardData()
    {
        //read the file
        string JSONtext = File.ReadAllText(JSONCardDataFilePath);
        var cardDict = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, Dictionary<string, string>>>>(JSONtext);

        List<CardData> newCards = new List<CardData>();
        //first, there are different cards
        foreach (var Card in cardDict) {
            CardData newCard = new CardData();
            newCard.cardName = Card.Key;

            foreach (var cardEffect in Card.Value) {
                CardEffectData newCardEffect = new CardEffectData();
                string label = cardEffect.Key;
                if (label.Contains("[")) {
                    string variant = GetSubstringByString("[", "]", label);
                    label = label.Replace("[" + variant + "]", "");
                    newCardEffect.variant = variant;
                }

                newCardEffect.recipient = label;

                foreach (var characterEffect in cardEffect.Value) {
                    if (characterEffect.Key.ToLower() == "reality") {
                        newCardEffect.reality = characterEffect.Value;
                    }
                    else {
                        CharacterCardEffectData newCharacterEffect = new CharacterCardEffectData();
                        newCharacterEffect.characterName = characterEffect.Key;
                        newCharacterEffect.variant = characterEffect.Value;
                        newCardEffect.characterEffects.Add(newCharacterEffect);
                    }
                }
                newCard.cardEffects.Add(newCardEffect);
            }
            newCards.Add(newCard);
        }
        cardData = newCards;

        //broadcast that the card data has been loaded/reloaded
        if (onStoryDataRefresh != null) {
            onStoryDataRefresh();
        }
    }

    void ParseDialogueData()
    {
        //print("parsing dialogue data");

        List<RealityData> newData = new List<RealityData>();
        string JSONtext = File.ReadAllText(JSONDialogueFilePath);

        //first, there are the different realities.
        var realityDict = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, Dictionary<string, string>>>>(JSONtext);
        foreach (var reality in realityDict) {

            RealityData newReality = new RealityData();
            newReality.name = reality.Key;

            //each reality has characters
            var characterDict = reality.Value;
            foreach (var character in characterDict) {
                CharacterDialogueData newCharacter = new CharacterDialogueData();

                string characterName = character.Key;
                if (character.Key.Contains("[")) {
                    string variant = GetSubstringByString("[", "]", characterName);
                    characterName = characterName.Replace("[" + variant + "]", "");
                    newCharacter.variant = variant;
                }
                newCharacter.characterName = characterName;

                //each character has lines
                foreach (var line in character.Value) {
                    DialogueLineData newLine = new DialogueLineData();
                    newLine.text = line.Value;
                    string label = line.Key;

                    //process lies
                    if (line.Key.Contains("[LIE]")) {
                        label = label.Replace("[LIE]", "");
                        string requiredEvidence = GetSubstringByString("[", "]", newLine.text);
                        newLine.text = newLine.text.Replace("[" + requiredEvidence + "]", "");
                        newLine.requiredEvidence = requiredEvidence;
                        newLine.lie = true;
                        newLine.truth = character.Value[label + "[TRUTH]"];

                        //if there's evidence in the true version of the statement, add it in there
                        string evidenceGained = GetSubstringByString("{", "}", newLine.truth);
                        if (evidenceGained.Length > 0) {
                            newLine.truth = newLine.truth.Replace("{" + evidenceGained + "}", "");
                            newLine.evidenceGained = evidenceGained;
                        }
                    }

                    //proccess variant conflicts
                    if (line.Key.Contains("conflict")) {
                        variantConflict newVariantConflict = new variantConflict();
                        newVariantConflict.character = GetSubstringByString("[", "]", line.Key);
                        newVariantConflict.variant = line.Value;
                        newCharacter.conflicts.Add(newVariantConflict);
                        continue;
                    }

                    //mark if a line of dialogue uncovers new evidence in the case
                    string _evidenceGained = GetSubstringByString("{", "}", line.Value);
                    if (_evidenceGained.Length > 0) {
                        newLine.text.Replace("{" + _evidenceGained + "}", "");
                        newLine.evidenceGained = _evidenceGained;
                    }

                    //sort the data into the correct place
                    if (label == "where") {
                        newCharacter.alibi = newLine;
                    }
                    else if (label == "relationship") {
                        newCharacter.relationship = newLine;
                    }
                    else if (label == "default") {
                        newCharacter.defaultResponse = newLine;
                    }
                    else if (!label.Contains("where") && !label.Contains("default") && !label.Contains("relationship") && !label.Contains("[TRUTH]")) {
                        EvidenceResponseData newEvidenceResponse = new EvidenceResponseData();
                        newEvidenceResponse.line = newLine;
                        newEvidenceResponse.item = label;
                        newCharacter.evidenceResponses.Add(newEvidenceResponse);
                    }
                }
                newReality.characters.Add(newCharacter);
            }
            newData.Add(newReality);
        }
        storyData = newData;

        //broadcast that the story data has been loaded/reloaded
        if (onStoryDataRefresh != null) {
            onStoryDataRefresh();
        }
    }
}
