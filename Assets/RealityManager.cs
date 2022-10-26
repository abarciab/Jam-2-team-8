using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RealityManager : MonoBehaviour
{
    [System.Serializable]
    public class CardDrawEvent
    {
        public string card;
        public string character;
    }


    public static RealityManager instance;

    public List<CardDrawEvent> cardHistory = new List<CardDrawEvent>();

    public List<List<CharacterDialogueData>> allRealities = new List<List<CharacterDialogueData>>();
    public List<CharacterDialogueData> baseReality = new List<CharacterDialogueData>();
    public List<CharacterDialogueData> currentReality = new List<CharacterDialogueData>(); 


    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        JSONParser.instance.onStoryDataRefresh += InitializeReality;
    }

    void InitializeReality()
    {
        allRealities = JSONParser.instance.storyData;
    }

    //use this function to get all the lines for a certain character in a certain reality. if 
    public CharacterDialogueData getCharacterDialogue(string characterName)
    {
        return null;
    }

    public void drawCard()
    {

    }
}
