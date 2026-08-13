using System;
using UnityEngine;

[Serializable]
public class DialogueLine {
    [TextArea(4, 8)]
    public string text;
    public string speakerName;
}
