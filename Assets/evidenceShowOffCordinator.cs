using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class evidenceShowOffCordinator : MonoBehaviour
{
    public TextMeshProUGUI evidenceNameText;
    public TextMeshProUGUI evidenceDescription;
    public Image evidenceSprite;
    public void ShowOffEvidence(string evidenceName)
    {
        EvidenceData evidence = JSONParser.instance.GetEvidenceByName(evidenceName);
        evidenceSprite.sprite = JSONParser.instance.getEvidenceSpriteByName(evidenceName);
        evidenceNameText.text = evidence.displayName;
        evidenceDescription.text = evidence.description;
        gameObject.SetActive(true);
    }
}
