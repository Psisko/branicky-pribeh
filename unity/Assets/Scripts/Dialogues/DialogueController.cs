using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueController : MonoBehaviour
{
    public DialogueSO dialogueData;
    [SerializeField]
    private Trigger trigger;
    [SerializeField]
    private DialogueUI dialogueUI;

    public void Start() {
        trigger.activationEvent.AddListener(StartDialogue);
    }



    private void StartDialogue() {
        trigger.SetIsEnabled(false);
        dialogueUI.dialogueEndEvent.AddListener(EndDialogue);
        dialogueUI.StartDialogue(dialogueData);
    }

    public void EndDialogue(string endChoice) {
        dialogueUI.dialogueEndEvent.RemoveListener(EndDialogue);
        trigger.SetIsEnabled(true);
    }

}
