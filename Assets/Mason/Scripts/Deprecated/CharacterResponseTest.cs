using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterResponseTest : MonoBehaviour
{
    [SerializeField] private string characterName;
    [SerializeField] private string dialogueType;

    // Start is called before the first frame update
    void Start()
    {
        CharacterResponseManager.instance.writeCharacterDialogue(characterName, dialogueType);
    }
}
