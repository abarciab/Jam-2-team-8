using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvidencePickup : MonoBehaviour
{
    public int evidenceID;
    public List<string> allowedRealities;
    public bool collected { get; private set;}

    private void Awake() {
        collected = false;
    }
    
    public void collectEvidence(string evidence) {
        collected = true;
        EvidenceManager.instance.addEvidence(evidence);
        AudioManager.instance.PlayGlobal(12, restart: false);
        UIManager.instance.showOff.ShowOffEvidence(RoomManager.instance.getEvidenceNameByID(evidenceID));
        gameObject.SetActive(false);
    }
}
