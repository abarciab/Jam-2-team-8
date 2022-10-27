using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueButtons : MonoBehaviour
{
    [SerializeField] private string dialogueType;
    public string characterName;        // can be passed in from the button manager
    private Button button;

    private void Start() {
        button  = GetComponent<Button>();
        button.onClick.AddListener(buttonPressed);
    }

    private void buttonPressed() {
        CharacterResponseManager.instance.writeCharacterDialogue(characterName, dialogueType);
    }
}
