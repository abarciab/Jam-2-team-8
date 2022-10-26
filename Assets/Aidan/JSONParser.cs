using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using Newtonsoft.Json;
using Unity.VisualScripting;
using System.IO;
using System;
using System.Data;

[System.Serializable]
public class DialogueLineData  {
    public string text;
    public bool lie;
    public string evidenceGained;
    public string requiredEvidence;
    public string truth;
}

[System.Serializable]
public class CharacterDialogueData {

    public string characterName;
    public string variant;
    public DialogueLineData defaultResponse;
    public DialogueLineData alibi;
    public DialogueLineData relationship;
    public Dictionary<string, DialogueLineData> evidenceResponses = new Dictionary<string, DialogueLineData>();
}

[ExecuteAlways]
public class JSONParser : MonoBehaviour
{
    public static JSONParser instance;
    public bool parse = false;

    public string JSONfilePath = "Assets/Aidan/testJSON.txt";

    public CharacterDialogueData example;
    public List<List<CharacterDialogueData>> storyData = new List<List<CharacterDialogueData>>();
    public event Action onStoryDataRefresh;


    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        ParseData();
    }

    private void Update()
    {
        if (parse) {
            parse = false;
            ParseData();
        }
    }

    public string GetSubstringByString(string a, string b, string c)
    {
        if (a.Length <= 0 || b.Length <= 0 || c.Length <= 0 || !c.Contains(a) || !c.Contains(b) ) { return ""; }
        return c.Substring((c.IndexOf(a) + a.Length), (c.IndexOf(b) - c.IndexOf(a) - a.Length));
    }

    void ParseData()
    {
        print("parsing JSON data");

        //broadcast that the story data has been loaded/reloaded
        if (onStoryDataRefresh != null) {
            onStoryDataRefresh();
        }

        List<List<CharacterDialogueData>> newData = new List<List<CharacterDialogueData>>();
        string JSONtext = File.ReadAllText(JSONfilePath);

        //first, there are the different realities.
        var realityDict = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, Dictionary<string, string>>>>(JSONtext);
        foreach (var reality in realityDict) {

            newData.Add(new List<CharacterDialogueData>());

            //each reality has characters
            var characterDict = reality.Value;
            foreach (var character in characterDict) {
                CharacterDialogueData newCharacter = new CharacterDialogueData();

                string characterName = character.Key;
                if (character.Key.Contains("[")) {
                    string variant = GetSubstringByString("[", "]", characterName);
                    characterName = characterName.Replace(variant, "");
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
                        newCharacter.evidenceResponses.Add(label, newLine);
                    }
                }

                example = newCharacter;
                newData[newData.Count - 1].Add(newCharacter);
            }
        }
        storyData = newData;
    }
}
