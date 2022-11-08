using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CardButtonAnimationCoordinator : MonoBehaviour
{
    Animator animator;
    string currentCharacter;
    int numValidCards;
    public int lineCounter;

    public int eagerThreshold;
    public int forceDrawThreshold;

    bool listenerAdded;

    private void Start()
    {
        if (!listenerAdded) {
            DialogueLog.onDialogueLineAdded += IncrementCounter;
            listenerAdded = true;
        }
        animator = GetComponent<Animator>();
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
        if (numValidCards == 0) { return; }
        animator.SetBool("eager", lineCounter >= eagerThreshold);
        if (lineCounter >= forceDrawThreshold) { ForceDraw(); }
    }

    public void PointerEnter()
    {
        animator.SetBool("hover", true);
    }
    public void PointerExit()
    {
        animator.SetBool("hover", false);
    }

    void ForceDraw()
    {
        print("I'm forcing you to draw a card");
        lineCounter = 0;
    }


}
