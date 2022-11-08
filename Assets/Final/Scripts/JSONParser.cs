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
using System.Linq;
using UnityEditor;
using Unity.VisualScripting.FullSerializer;

[System.Serializable]
public class AccusationData {
    public string reality;
    public string murderer;
    public List<EvidenceData> means = new List<EvidenceData>();
    public List<EvidenceData> motive = new List<EvidenceData>();
}

//classes for evidence data
[System.Serializable]
public class EvidenceData {
    public string name;
    public string displayName;
    public string description;
    public List<string> allowedRealities = new List<string>();
    public List<StringPairData> means = new List<StringPairData>();        //"with poison", or "by pushing him off a roof", for example
    public List <StringPairData> motive = new List<StringPairData>();       //"owed him money", or "discovered he was having an affair", for example
}
[System.Serializable]
public class StringPairData {
    public string name;
    public List<string> applicableCharacters = new List<string>();
}

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
    public string characterName;        //the character this would effect
    public string variant;              //the variant that will be activated
}


//classes for dialogue data
[System.Serializable]
public class DialogueLineData  {
    public string text;
    public bool lie;
    public string evidenceGained;
    public string requiredEvidence;
    public string truth;

    public void switchTextToTruth() {
        string temp = text;
        text = truth;
        truth = temp;
        lie = !lie;
    }
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
    public List<string> validRealities = new List<string>();            //all the realities that this character can exist in

    public DialogueLineData getEvidenceResponse(string evidenceName)
    {
        foreach (var response in evidenceResponses) {
            if ((!string.IsNullOrEmpty(response.item) && !string.IsNullOrEmpty(evidenceName)) && response.item.ToLower() == evidenceName.ToLower()) {
                return response.line;
            }
        }
        return new DialogueLineData();
    }
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
    
    
    public string JSONDialogueFilePath = "Assets/Aidan/testJSON.txt";
    public string JSONCardDataFilePath = "Assets/Aidan/cardJSON.txt";
    public string JSONAccusationDataFilePath = "Assets/Aidan/JSON/evidence.json";
    public string JSONEvidenceDataFilePath = "Assets/Aidan/JSON/accusations.json";

    public List<RealityData> storyData = new List<RealityData>();
    public List<CardData> cardData = new List<CardData>();
    public List<EvidenceData> evidenceData = new List<EvidenceData>();
    public List<RealityManager.characterData> evidenceSprites = new List<RealityManager.characterData>();
    public List<AccusationData> accusationData = new List<AccusationData>();
    public static Action onStoryDataRefresh;

    [Header("testing")]
    public bool parseAll;
    public bool parseDialogue;
    public bool parseCards;
    public bool parseAccusations;
    public bool parseEvidence;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        if (Application.isPlaying) {
            parseAll = true;
        }
    }

    private void Update()
    {
        if (parseAll) {
            parseAll = false;
            ParseDialogueData();
            ParseCardData();
            ParseEvidenceData();
            ParseAccusationData();
        }

        if (parseDialogue) {
            parseDialogue = false;
            ParseDialogueData();
        }
        if (parseCards) {
            parseCards = false;
            ParseCardData();
        }
        if (parseEvidence) {
            parseEvidence = false;
            ParseEvidenceData();
        }
        if (parseAccusations) {
            parseAccusations = false;
            ParseAccusationData();
        }
    }

    public EvidenceData GetEvidenceByName(string name)
    {
        for (int i = 0; i < evidenceData.Count; i++) {
            if (evidenceData[i].name == name) {
                return evidenceData[i];
            }
        }

        print(name + " is not listed in evidence database");
        return null;
    }

    string GetSubstringByString(string a, string b, string c)
    {
        if (a.Length <= 0 || b.Length <= 0 || c.Length <= 0 || !c.Contains(a) || !c.Contains(b) ) { return ""; }
        return c.Substring((c.IndexOf(a) + a.Length), (c.IndexOf(b) - c.IndexOf(a) - a.Length));
    }

    void ParseAccusationData()
    {
        //read the file
        string JSONtext = File.ReadAllText(JSONAccusationDataFilePath);
        var accusationDict = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(JSONtext);

        List<AccusationData> newAccusationList = new List<AccusationData>();
        foreach (var accusation in accusationDict) {
            AccusationData newEvidence = new AccusationData();
            newEvidence.reality = accusation.Key;

            foreach (var detail in accusation.Value) {
                if (detail.Key == "murderer") {
                    newEvidence.murderer = detail.Value;
                }
                if (detail.Key == "means") {
                    var meansList = detail.Value.Split(", ").ToList();
                    foreach (var means in meansList) {
                        var evidence = GetEvidenceByName(means);
                        if (evidence != null) 
                            newEvidence.means.Add(evidence);
                    }
                }
                if (detail.Key == "motive") {
                    var motivesList = detail.Value.Split(", ").ToList();
                    foreach (var motive in motivesList) {
                        var evidence = GetEvidenceByName(motive);
                        if (evidence != null)
                            newEvidence.motive.Add(evidence);
                    }
                }
            }
            newAccusationList.Add(newEvidence);
        }
        accusationData = newAccusationList;

        //broadcast that the card data has been loaded/reloaded
        if (onStoryDataRefresh != null) {
            onStoryDataRefresh();
        }
    }

    public Sprite getEvidenceSpriteByName(string evidenceName)
    {
        foreach (var evidence in evidenceSprites) {
            if (evidence.characterName == evidenceName) {
                return evidence.characterPortrait;
            }
        }
        print("there's no sprite for " + evidenceName);
        return null;
    }

    bool EvidenceInSpriteList(string evidenceName)
    {
        foreach (var evidence in evidenceSprites) {
            if (evidence.characterName == evidenceName) {
                return true;
            }
        }
        return false;
    }

    void ParseEvidenceData()
    {
        //read the file
        string JSONtext = File.ReadAllText(JSONEvidenceDataFilePath);
        var evidenceDict = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(JSONtext);

        List<EvidenceData> newEvidenceList = new List<EvidenceData>();
        //first, there are different pieces of evidence
        foreach (var evidence in evidenceDict) {
            EvidenceData newEvidence = new EvidenceData();
            newEvidence.name = evidence.Key;

            if (!EvidenceInSpriteList(newEvidence.name)) {
                var newEvidenceSprite = new RealityManager.characterData();
                newEvidenceSprite.characterName = newEvidence.name;
                evidenceSprites.Add(newEvidenceSprite);
            }

            foreach (var detail in evidence.Value) {
                if (detail.Key == "displayName") {
                    newEvidence.displayName = detail.Value;
                }
                if (detail.Key == "description") {
                    newEvidence.description = detail.Value;
                }
                if (detail.Key == "realities") {
                    var strArray = detail.Value.Split(", ").ToList();
                    newEvidence.allowedRealities = strArray;
                }
                if (detail.Key == "means") {
                    var meansList = detail.Value.Split(", ").ToList();
                    for (int i = 0; i < meansList.Count; i++) {
                        StringPairData newMeans = new StringPairData();
                        var characterListString = GetSubstringByString("[", "]", meansList[i]);
                        var characterList = characterListString.Split(", ").ToList();
                        if (characterList.Count == 0 || string.IsNullOrEmpty(characterList[0])) { newMeans.applicableCharacters.Add("ANY");  }
                        else { newMeans.applicableCharacters.AddRange(characterList); }
                        newMeans.name = meansList[i].Replace("[" + characterListString + "]", "");
                        newEvidence.means.Add(newMeans);
                    }
                }
                if (detail.Key == "motive") {
                    var motiveList = detail.Value.Split(", ").ToList();
                    for (int i = 0; i < motiveList.Count; i++) {
                        StringPairData newMotive = new StringPairData();
                        var characterListString = GetSubstringByString("[", "]", motiveList[i]);
                        var characterList = characterListString.Split(", ").ToList();
                        if (characterList.Count == 0 || string.IsNullOrEmpty(characterList[0])) { newMotive.applicableCharacters.Add("any"); }
                        else { newMotive.applicableCharacters.AddRange(characterList); }
                        newMotive.name = motiveList[i].Replace("[" + characterListString + "]", "");
                        newEvidence.motive.Add(newMotive);
                    }
                }
            }
            newEvidenceList.Add(newEvidence);
        }
        evidenceData = newEvidenceList;

        //broadcast that the card data has been loaded/reloaded
        if (onStoryDataRefresh != null) {
            onStoryDataRefresh();
        }
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
                        newCharacterEffect.variant = characterEffect.Value.ToUpper();
                        newCardEffect.characterEffects.Add(newCharacterEffect);
                    }
                }
                newCard.cardEffects.Add(newCardEffect);
            }
            //print("adding card to data");
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
                            newLine.evidenceGained = evidenceGained.ToUpper();
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

                    //process this variant exsiting in different realities
                    if (line.Key.ToLower().Contains("realities")) {
                        List<string> validRealities = new List<string>();
                        string list = line.Value.ToLower();
                        if (!list.Contains(newReality.name)) {
                            validRealities.Add(newReality.name);
                        }
                        var strArray = list.Split(", ");
                        validRealities.AddRange(strArray);
                        newCharacter.validRealities = validRealities;
                    }
                    else if (!newCharacter.validRealities.Contains(newReality.name)) {
                        newCharacter.validRealities.Add(newReality.name);
                    }

                    //mark if a line of dialogue uncovers new evidence in the case
                    string _evidenceGained = GetSubstringByString("{", "}", line.Value);
                    if (_evidenceGained.Length > 0) {
                        newLine.text = newLine.text.Replace("{" + _evidenceGained + "}", "");
                        newLine.evidenceGained = _evidenceGained.ToUpper();
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
                    else if (!label.Contains("where") && !label.Contains("default") && !label.Contains("relationship") && !label.Contains("[TRUTH]") && !label.Contains("realities")) {
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
