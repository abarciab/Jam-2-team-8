using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AccusationEvidenceSelector : MonoBehaviour
{
    public AccusationComponent accusationComponent;
    public enum AccusationComponent { murderer, means, motive};


    public TextMeshProUGUI evidenceNameText;
    public string nameInSelectorList;
    public string evidenceName;
    public string SelectedName;
    public Sprite evidenceSprite;
    public Image evidenceImg;
    public accusationUI UIScript;

    bool setUp;

    public void MakeSelection()
    {
        switch (accusationComponent) {
            case AccusationComponent.murderer:
                UIScript.SelectMurderer(SelectedName);
                break;
            case AccusationComponent.means:
                UIScript.SelectMeans(evidenceName, SelectedName);
                break;
            case AccusationComponent.motive:
                UIScript.SelectMotive(evidenceName, SelectedName);
                break;
        }
        UIScript.moveEndPoint(gameObject);
    }

    private void Update()
    {
        if (!setUp) {
            OnEnable();
        }
    }

    private void OnEnable()
    {
        if (string.IsNullOrEmpty(nameInSelectorList)) { print("i need a name"); return; }
        if (accusationComponent == AccusationComponent.motive || accusationComponent == AccusationComponent.murderer) 
            evidenceNameText.text = SelectedName.Replace("{THEY}", RealityManager.instance.getCharacterPronounByName(UIScript.selectedMurderer));
        else 
            evidenceNameText.text = nameInSelectorList.Replace("{THEY}", RealityManager.instance.getCharacterPronounByName(UIScript.selectedMurderer));

        if (evidenceSprite == null) { print("i need a sprite"); return; }
        evidenceImg.sprite = evidenceSprite;

        setUp = true;
    }
}
