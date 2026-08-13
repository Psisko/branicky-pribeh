using System.Collections;
using UnityEditor;
using UnityEngine;

public class FirstTimeInBranik : MonoBehaviour {

    [SerializeField] private DialogueSO dialogue;

    private void Start() {
        MissionsState missionState = GameObject.FindGameObjectWithTag("GameState").GetComponent<GameState>().GetMissionsState();
        DialogueUI dialogueUI = GameObject.FindAnyObjectByType<DialogueUI>();


        if (missionState.GetMostRecentMission() == "unetice")
        {
            dialogueUI.StartDialogue(dialogue);
        }
    }
}
