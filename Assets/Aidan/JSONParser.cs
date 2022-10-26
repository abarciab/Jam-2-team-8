using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class DialogueLine  {
    public string text;
    public bool lie;

    public string requiredEvidence;
    public string truth;
}

[System.Serializable]
public class CharacterScript {

    public string characterName;
    public DialogueLine defaultResponse;
    public DialogueLine alibi;
    public DialogueLine relationship;
    public Dictionary<string, DialogueLine> evidenceResponses;

}

[ExecuteAlways]
public class JSONParser : MonoBehaviour
{
    public static JSONParser instance;
    public bool parse = false;

    public TextAsset JSONfile;

    public CharacterScript example;



    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        if (parse) {
            parse = false;
            ParseData();
        }
    }

    void ParseData()
    {

    }
}
