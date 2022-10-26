using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace DialogueSystem {
    public class DialogueLine : DialogueBase
    {
        [Header("Text Options")]
        [SerializeField] private string input;
        [SerializeField] private Color textColor;
        [SerializeField] private Font textFont;

        [SerializeField] private float scrollDelay;
        private Text textHolder;

        private void Awake() {
            textHolder = GetComponent<Text>();
            StartCoroutine(writeText(input, textHolder, textColor, textFont, scrollDelay));
        }
    }
}

