using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterInteract : MonoBehaviour
{
    [SerializeField] private string characterName;
    [SerializeField] private string characterGreeting;
    [SerializeField] private Sprite portraitSprite;
    private Button button;

    private void Start() {
        button = GetComponent<Button>();
        button.onClick.AddListener(openCharacter);
    }

    private void openCharacter() {
        CharacterResponseManager.instance.currentCharacterName = characterName;
        CharacterResponseManager.instance.portraitSprite = portraitSprite;
        UIManager.instance.showDialogueUI();
        //DialogueButtonManager.instance.setCharacter(characterName);
        
        DialogueLine line = new DialogueLine(characterGreeting);
        CharacterResponseManager.instance.characterGreeting(line);
        UIManager.instance.hideCharacters();
    }
}
