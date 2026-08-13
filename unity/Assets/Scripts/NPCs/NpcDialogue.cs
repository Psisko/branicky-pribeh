using System;

[Serializable]
public class NpcDialogue {
    public DialogueSO data;
    public bool onlyOnce;
    public float lastPlayed;

    public bool HasBeenPlayed() { 
        return lastPlayed != 0;
    }
}
