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
}
