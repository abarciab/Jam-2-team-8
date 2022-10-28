using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndConversationButton : MonoBehaviour
{
    private Button button;

    private void Start() {
        button = GetComponent<Button>();
        button.onClick.AddListener(endConvo);
    }

    private void endConvo() {
        UIManager.instance.hideUI();
    }
}
