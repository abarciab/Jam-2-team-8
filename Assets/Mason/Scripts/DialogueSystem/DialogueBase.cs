using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace DialogueSystem {
    public class DialogueBase : MonoBehaviour
    {
        protected IEnumerator writeText(string input, Text textHolder, Color textColor, Font textFont, float delay) {
            textHolder.text = "";
            textHolder.color = textColor;
            textHolder.font = textFont;
            for(int i = 0; i < input.Length; ++i) {
                textHolder.text += input[i];
                //AudioManager.instance.PlayGlobal(0, 1);
                yield return new WaitForSeconds(delay);
            }
        }
    }
}

