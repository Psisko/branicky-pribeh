using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Dialogues/Dialogue", fileName = "NewDialogue")]
public class DialogueSO : ScriptableObject {
    public List<DialogueLine> entryLines;
    public List<DialogueChoice> choices;
    public Sprite participantPortrait;
    public bool monologue;
    //public Sprite participantAudio; pripraveno pro Kryštofùv voiceover
}
