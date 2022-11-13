using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CardButtonAnimationCoordinator : MonoBehaviour
{
    Animator animator;
    string currentCharacter;
    int numValidCards;
    public int lineCounter;

    public int minThreshold;
    public int eagerThreshold;

    bool listenerAdded;

    Button buttonComponent;

    private void Start()
    {
        if (!listenerAdded) {
            DialogueLog.onDialogueLineAdded += IncrementCounter;
            listenerAdded = true;
        }
        animator = GetComponent<Animator>();
        buttonComponent = GetComponent<Button>();
    }

    void IncrementCounter()
    {
        lineCounter += 1;
    }

    private void Update()
    {
        if (currentCharacter != CharacterResponseManager.instance.currentCharacterName) {
            currentCharacter = CharacterResponseManager.instance.currentCharacterName;
            numValidCards = RealityManager.instance.NumValidCards(currentCharacter);
            return;
        }

        animator.SetBool("disabled", numValidCards == 0);
        buttonComponent.enabled = false;

        if (numValidCards == 0 || lineCounter < minThreshold) { return; }
        
        buttonComponent.enabled = true;
        animator.SetBool("eager", lineCounter >= eagerThreshold);      
    }

    public void PointerEnter()
    {
        animator.SetBool("hover", true);
    }
    public void PointerExit()
    {
        animator.SetBool("hover", false);
    }

    public void cardDrawn() {
        lineCounter = 0;
    }
}
