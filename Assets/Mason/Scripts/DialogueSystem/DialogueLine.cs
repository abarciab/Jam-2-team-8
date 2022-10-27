using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class DialogueLine
{
    [Header("Text Options")]
    public string text;
    public Color textColor;
    public Font textFont;
    public bool isNewLine;

    [Header("Other Options")]
    public float scrollDelay;
    public int soundID;

    public DialogueLine(string text, Color textColor, Font textFont, bool isNewLine, float scrollDelay, int soundID) {
        this.text = text;
        this.textColor = textColor;
        this.textFont = textFont;
        this.isNewLine = isNewLine;
        this.scrollDelay = scrollDelay;
        this.soundID = soundID;
    }
}
