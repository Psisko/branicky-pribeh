using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(NpcJobs))]
public class Kvasnik : MonoBehaviour
{
    [SerializeField] private NpcDialogue defaultDialogue;
    [SerializeField] private NpcDialogue branikBrokenDialogue;
    [SerializeField] private NpcDialogue kvasnaKadBoughtDialogue;
    [SerializeField] private NpcDialogue branikWorkingDialogue;
    [SerializeField] private NpcDialogue healDialogue;
    [SerializeField] private Trigger dialogueTrigger;


    private BranikState branikState;
    private NpcJobs job;
    private PlayerState playerState;


    private void Awake() {
        job = GetComponent<NpcJobs>();
    }

    private void Start() {
        GameState gameState = GameObject.FindGameObjectWithTag("GameState").GetComponent<GameState>();
        branikState = gameState.GetBranikState();
        playerState = gameState.GetPlayerState();

        dialogueTrigger.activationEvent.AddListener(Talk);
    }

    public void Talk() {
        List<NpcDialogue> availableDialogue = new();

        availableDialogue.Add(healDialogue);

        if (!branikState.AllMachinesOwned())
            availableDialogue.Add(branikBrokenDialogue);
        if (branikState.IsOwned(BranikMachines.kvasnaKad))
            availableDialogue.Add(kvasnaKadBoughtDialogue);
        if (branikState.AllMachinesOwned())
            availableDialogue.Add(branikWorkingDialogue);

        availableDialogue.Add(defaultDialogue);

        dialogueTrigger.SetIsEnabled(false);
        job.dialogueOutputEvent.AddListener(DialogueOutput);
        job.PlayDialogue(availableDialogue);
    }



    private void DialogueOutput(string output) {
        if (output == "giveDmgPotion") {
            playerState.AddDamagePotion();
        }
        dialogueTrigger.SetIsEnabled(true);
    }

}
