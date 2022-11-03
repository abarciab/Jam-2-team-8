using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvidenceManager : MonoBehaviour
{
    public bool add = false;

    public static EvidenceManager instance;
    public List<EvidenceData> evidenceList = new List<EvidenceData>();
    public List<EvidenceData> discardedEvidence = new List<EvidenceData>();
    public List<string> charactersInteractedWith = new List<string>();

    private void Start()
    {
        //print("testing with intial evidence, remove when done!");
        //add = true;
    }

    private void Awake() {
        // if no duplicates
        if(instance == null) {
            instance = this;                    // makes instance a singleton
            //DontDestroyOnLoad(gameObject);      // prevents EvidenceManager from being destroyed when new level loaded
        }
        // if duplicates exist
        else if(instance != null && instance != this) {
            Destroy(gameObject);                // get rid of duplicate
        }
    }

    // lookup evidence from JSON Parser and add it to evidence list
    public void addEvidence(string evidenceName) {
        // loop through all parsed evidence
        foreach(EvidenceData evidence in JSONParser.instance.evidenceData) {
            // if evidence is found, add to list
            if(evidence.name.ToLower() == evidenceName.ToLower()) {
                evidenceList.Add(evidence);
                return;
            }
        }
        Debug.LogError("Evidence: " + evidenceName + " does not exist");
    }

    public void removeEvidence(string evidenceName) {
        foreach(EvidenceData evidence in evidenceList) {
            // remove evidence if current reality is not inside the allowedRealities
            if(!evidence.allowedRealities.Contains(RealityManager.instance.currentReality.name)) {
                discardedEvidence.Add(evidence);
                evidenceList.Remove(evidence);
            }
        }
    }

    public void addCharacter(string characterName) {
        if(!charactersInteractedWith.Contains(characterName))
            charactersInteractedWith.Add(characterName);
    }

    // write displayName into UI
    

    /*
    *  This is just test stuff
    */
    private void Update() {
        if(add) {
            //printList();
            addEvidence("gun used to kill joe");
            addEvidence("gambling debts");
            addEvidence("john protect sarah");
            add = false;
        }
    }

    private void printList() {
        print(JSONParser.instance.evidenceData.Count);
        foreach(EvidenceData evidence in JSONParser.instance.evidenceData) {
            print(evidence.name);
        }
    }
}
