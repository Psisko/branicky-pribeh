using System.Collections;
using UnityEngine;
using UnityEngine.Events;


/// <summary>
///     Dialogue player based on custom event system.
///     Ends the event.
/// </summary>
[RequireComponent(typeof(EventController))]
public class EventDialogue : MonoBehaviour {

    public DialogueSO dialogueData;
    public UnityEvent<string> dialogueOutputEvent;


    private EventController eventController;
    private DialogueUI dialogueUI;


    private void Awake() {
        eventController = GetComponent<EventController>();
    }

    private void Start() {
        dialogueUI = GameObject.FindAnyObjectByType<DialogueUI>();
        eventController.eventStart.AddListener(PlayDialogue);
    }

    private void PlayDialogue() {
        dialogueUI.dialogueEndEvent.AddListener(EndDialogue);
        dialogueUI.StartDialogue(dialogueData);
    }

    private void EndDialogue(string output) {
        dialogueUI.dialogueEndEvent.RemoveListener(EndDialogue);
        dialogueOutputEvent.Invoke(output);
        eventController.EndEvent();
    }


}
