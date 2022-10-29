using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class DialogueLine
{
    [Header("Text Options")]
    public string text;
    public Color textColor;
    //public Font textFont;
    public TMP_FontAsset textFont;
    public bool isNewLine;

    [Header("Other Options")]
    public float scrollDelay;
    public int soundID;

    public DialogueLine(string text, Color? textColor = null, TMP_FontAsset? textFont = default,//Font? textFont = null, 
                        bool isNewLine = true, float scrollDelay = 0.01f, int soundID = 0) {
        this.text = text;
        this.textColor = textColor ?? Color.black;
        this.textFont = textFont;// ?? Resources.GetBuiltinResource<Font>("Arial.ttf");
        this.isNewLine = isNewLine;
        this.scrollDelay = scrollDelay;
        this.soundID = soundID;
    }
}
