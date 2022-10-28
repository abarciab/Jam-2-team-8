using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterInteract : MonoBehaviour
{
    [SerializeField] private string characterName;
    [SerializeField] private string characterGreeting;
    private Button button;

    private void Start() {
        button = GetComponent<Button>();
        button.onClick.AddListener(openCharacter);
    }

    private void openCharacter() {
        UIManager.instance.showUI();
        DialogueButtonManager.instance.setCharacter(characterName);
        DialogueLine line = new DialogueLine(characterGreeting);
        StartCoroutine(DialogueBase.instance.writeText(line));
    }
}
