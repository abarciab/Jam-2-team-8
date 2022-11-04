using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterInteract : MonoBehaviour
{
    public string characterName;
    [SerializeField] private string characterGreeting;
    [SerializeField] private Sprite portraitSprite;
    [SerializeField] private Sprite fullBodySprite;
    private Button button;


    private void Start() {
        button = GetComponent<Button>();
        button.onClick.AddListener(openCharacter);
    }

    private void Update()
    {
        if (portraitSprite == null && RealityManager.instance != null) {
            Initialize();
        }
    }

    void Initialize()
    {
        portraitSprite = RealityManager.instance.getCharacterPortraitByName(characterName);
        fullBodySprite = RealityManager.instance.getCharacterFullBodyByName(characterName);
        GetComponent<Image>().sprite = fullBodySprite;
    }

    private void openCharacter() {
        // determine which character will be interacted with
        CharacterResponseManager.instance.currentCharacterName = characterName;
        CharacterResponseManager.instance.portraitSprite = portraitSprite;
        UIManager.instance.showDialogueUI();
        //DialogueButtonManager.instance.setCharacter(characterName);
        
        // have character greet player and hide clickable characters
        DialogueLine line = new DialogueLine(characterGreeting);
        CharacterResponseManager.instance.characterGreeting(line);
        UIManager.instance.hideUIElement("character");
        AudioManager.instance.PlayGlobal(11, 1, false);
    }
}
