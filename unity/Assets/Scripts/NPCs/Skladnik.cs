using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Playables;

[RequireComponent(typeof(NpcJobs))]
public class Skladnik : MonoBehaviour
{
    [SerializeField] private NpcDialogue defaultDialogue;
    [SerializeField] private NpcDialogue speedDialogue;
    [SerializeField] private NpcDialogue branikBrokenDialogue;
    [SerializeField] private NpcDialogue branikWorkingDialogue;
    [SerializeField] private NpcDialogue breakRoomDialogue;

    [SerializeField] private Transform upperSpawnPoint;
    [SerializeField] private Transform lowerSpawnPoint;
    [SerializeField] private Transform entranceOutside;
    [SerializeField] private Transform boxPile;
    [SerializeField] private Transform microwave;
    [SerializeField] private Transform refridgerator;
    [SerializeField] private Transform armchair;

    [SerializeField] private Navigator navigator;
    [SerializeField] private Animator animator;
    [SerializeField] private Trigger dialogueTrigger;

    private SkladnikState state;
    private bool movingStateBeforeDailogue = false;

    private BranikState branikState;
    private NpcJobs job;
    private MissionsState missionState;
    private PlayerState playerState;

    private void Awake() {
        job = GetComponent<NpcJobs>();
    }

    private void Start() {


        GameState gameState = GameObject.FindGameObjectWithTag("GameState").GetComponent<GameState>();
        missionState = GameObject.FindGameObjectWithTag("GameState").GetComponent<GameState>().GetMissionsState();
        playerState = gameState.GetPlayerState();
        branikState = gameState.GetBranikState();
        job.jobDoneEvent.AddListener(EndState);

        dialogueTrigger.activationEvent.AddListener(Talk);
        branikState.changeEvent.AddListener(ChangeBehaviour);

        if (branikState.AllMachinesOwned())
        {
            job.Teleport(upperSpawnPoint.position);
            ChangeState(SkladnikState.goToCrate);
        }
        else
        {
            job.Teleport(lowerSpawnPoint.position);
            ChangeState(SkladnikState.goingToFridge);

        }
    }

    public void Talk() {
        List<NpcDialogue> availableDialogue = new();

        availableDialogue.Add(speedDialogue);

        if (branikState.AllMachinesOwned())
            availableDialogue.Add(branikWorkingDialogue);
        if (!branikState.AllMachinesOwned())
            availableDialogue.Add(branikBrokenDialogue);
        if (state == SkladnikState.relaxing)
            availableDialogue.Add(breakRoomDialogue);

        availableDialogue.Add(defaultDialogue);


        dialogueTrigger.SetIsEnabled(false);
        movingStateBeforeDailogue = animator.GetBool("IsWalking");
        animator.SetBool("IsWalking", false);
        job.dialogueOutputEvent.AddListener(EnableDialogue);
        job.dialogueOutputEvent.AddListener(DialogueOutput);
        job.PlayDialogue(availableDialogue);
    }


    private void DialogueOutput(string output)
    {
        if (output == "giveSpeedPotion")
        {
            playerState.AddSpeedPotion();
        }
        dialogueTrigger.SetIsEnabled(true);
    }

    public void EndState() {
        switch (state) {
            
            case SkladnikState.managingBoxes:
                ChangeState(SkladnikState.goToCrate);
                break;
            
            case SkladnikState.goToCrate:
                ChangeState(SkladnikState.carryTheCrate);
                break;

            case SkladnikState.carryTheCrate:
                ChangeState(SkladnikState.managingBoxes);
                break;

            case SkladnikState.goingToFridge:
                ChangeState(SkladnikState.goingToArmchair);
                break;

            case SkladnikState.goingToArmchair:
                ChangeState(SkladnikState.relaxing);
                break;

            case SkladnikState.relaxing:
                ChangeState(SkladnikState.goingToFridge);
                break;

        }
    }

    private void ChangeState(SkladnikState newState) {
        state = newState;

        switch (state) {
            case SkladnikState.goToCrate:
                animator.SetBool("IsWalking", true);
                job.FollowPath(navigator.GetPathFromMe(transform.position, entranceOutside.position));
                break;

            case SkladnikState.carryTheCrate:
                animator.SetBool("IsWalking", true);
                job.FollowPath(navigator.GetPathFromMe(transform.position, boxPile.position));
                break;

            case SkladnikState.managingBoxes:
                animator.SetBool("IsWalking", false);
                job.Wait(3);
                break;

            case SkladnikState.goingToFridge:
                animator.SetBool("IsWalking", true);
                job.FollowPath(navigator.GetPathFromMe(transform.position, refridgerator.position));
                break;

            case SkladnikState.goingToArmchair:
                animator.SetBool("IsWalking", true);
                job.FollowPath(navigator.GetPathFromMe(transform.position, armchair.position));
                break;

            case SkladnikState.relaxing:
                animator.SetBool("IsWalking", false);
                job.Wait(10);
                break;
        }
    }


    private void EnableDialogue(string output) {
        animator.SetBool("IsWalking", movingStateBeforeDailogue);
        dialogueTrigger.SetIsEnabled(true);
    }

    /// <summary>
    ///     Changes behaviour from doing nothing to doing something. If all machines are owned.
    /// </summary>
    private void ChangeBehaviour() {
        if (branikState.AllMachinesOwned()) {
            job.Teleport(upperSpawnPoint.position);
            ChangeState(SkladnikState.goToCrate);
        }
    }

    private enum SkladnikState {
        managingBoxes,
        goToCrate,
        carryTheCrate,
        relaxing,
        goingToFridge,
        goingToArmchair,
    }

}
