 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BranikKrystofController : MonoBehaviour
{
    [SerializeField] private DialogueSO beforeStarobrnoDialogue;
    [SerializeField] private DialogueSO beforePlzenDialogue;
    [SerializeField] private DialogueSO afterPlzenDialogue;
    [SerializeField] private Trigger dialogueTrigger;
    [SerializeField] private BranikExitController exitController;
    [SerializeField] private Animator animator;

    private MissionsState missionState;
    private DialogueUI dialogueUI;

    private void Start() {
        dialogueTrigger.activationEvent.AddListener(ChooseDialogue);
        missionState = GameObject.FindGameObjectWithTag("GameState").GetComponent<GameState>().GetMissionsState();
        dialogueUI = GameObject.FindAnyObjectByType<DialogueUI>();
        animator.SetBool("sitting", true);
    }


    /// <summary>
    /// Chooses dialog based on completed missions
    /// </summary>
    private void ChooseDialogue() {
        if (missionState.GetMostRecentMission() == "unetice") {
            PlayDialogue(beforeStarobrnoDialogue);
            exitController.SetupExit("starobrno");
        } else if (missionState.IsAvailable("plzen")) {
            PlayDialogue(beforePlzenDialogue);
            exitController.SetupExit("plzen");
        } else if (missionState.IsCompleted("plzen")) {
            PlayDialogue(afterPlzenDialogue);
        }
    }

    /// <summary>
    /// Plays the chosen dialogue
    /// </summary>
    private void PlayDialogue(DialogueSO dialogue) {
        dialogueTrigger.SetIsEnabled(false);
        dialogueUI.dialogueEndEvent.AddListener(EndDialogue);
        if (!dialogueUI.StartDialogue(dialogue)) {
            EndDialogue("");
        }
    }

    public void EndDialogue(string endChoice) {
        dialogueUI.dialogueEndEvent.RemoveListener(EndDialogue);
        dialogueTrigger.SetIsEnabled(true);

        switch (endChoice) {
            case "exit":
                SceneManager.LoadScene("Credits");
                break;
        }    
    }

}
