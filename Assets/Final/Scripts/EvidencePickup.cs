using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvidencePickup : MonoBehaviour
{
    public string evidenceName;
    public bool collected { get; private set;}

    private void Awake() {
        collected = false;
    }
    
    public void collectEvidence(string evidence) {
        collected = true;
        EvidenceManager.instance.addEvidence(evidence);
        gameObject.SetActive(false);
    }
}
