using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Bartender : MonoBehaviour
{
    [SerializeField] private NpcDialogue defaultDialogue;
    [SerializeField] private NpcDialogue servingOtherBeer;
    [SerializeField] private NpcDialogue servingBranikBeer;
    [SerializeField] private NpcDialogue chillRoomDialogue;

    [SerializeField] private Transform bar;
    [SerializeField] private List<Transform> tables;
    [SerializeField] private Transform barellMachine;
    [SerializeField] private Transform barellAtBar;
    [SerializeField] private Transform chillRoomNavNode;

    [SerializeField] private Navigator navigator;
    [SerializeField] private Animator animator;
    [SerializeField] private Trigger dialogueTrigger;

    private BartenderState bState;
    //private int pouredBeerCount = 0;
    private bool movingStateBeforeDailogue = false;

    private BranikState branikState;
    private NpcJobs job;


    private void Awake() {
        job = GetComponent<NpcJobs>();
    }

    private void Start() {
        GameState gameState = GameObject.FindGameObjectWithTag("GameState").GetComponent<GameState>();
        branikState = gameState.GetBranikState();
        job.jobDoneEvent.AddListener(EndState);

        
        dialogueTrigger.activationEvent.AddListener(Talk);
        job.Teleport(bar.position);
        ChangeState(BartenderState.idleAtBar);
    }

    public void Talk() {
        List<NpcDialogue> availableDialogue = new();

        if (branikState.AllMachinesOwned())
            availableDialogue.Add(servingBranikBeer);
        if (!branikState.AllMachinesOwned())
            availableDialogue.Add(servingOtherBeer);
        if (bState == BartenderState.relaxing)
            availableDialogue.Add(chillRoomDialogue);

        availableDialogue.Add(defaultDialogue);

        dialogueTrigger.SetIsEnabled(false);
        movingStateBeforeDailogue = animator.GetBool("IsWalking");
        animator.SetBool("IsWalking", false);
        job.dialogueOutputEvent.AddListener(EnableDialogue);
        job.PlayDialogue(availableDialogue);
    }


    public void EndState() {
        switch (bState) {
            case BartenderState.idleAtBar:
                ChangeState(BartenderState.pouringBeer);
                break;

            case BartenderState.pouringBeer:
                ChangeState(BartenderState.goToTable);
                break;

            case BartenderState.goToTable:
                ChangeState(BartenderState.puttingBeerOnTheTable);
                break;

            case BartenderState.puttingBeerOnTheTable:
                ChangeState(BartenderState.goToBar);
                break;

            case BartenderState.goToBar:
                ChangeState(BartenderState.idleAtBar);
                break;

            case BartenderState.walkingToRelaxRoom:
                ChangeState(BartenderState.relaxing);
                break;

            case BartenderState.relaxing:
                ChangeState(BartenderState.goToBar);
                break;

        }
    }

    private void ChangeState(BartenderState newState) {
        bState = newState;
        switch (bState) {
            case BartenderState.idleAtBar:
                animator.SetBool("IsWalking", false);
                job.Wait(8);
                break;

            case BartenderState.pouringBeer:
                animator.SetBool("IsWalking", false);
                job.Wait(5);
                break;

            case BartenderState.goToTable:
                animator.SetBool("IsWalking", true);
                job.FollowPath(navigator.GetPathFromMe(transform.position, tables[Random.Range(0, 5)].position));
                break;

            case BartenderState.puttingBeerOnTheTable:
                animator.SetBool("IsWalking", false);
                job.Wait(5);
                break;

            case BartenderState.goToBar:
                animator.SetBool("IsWalking", true);
                job.FollowPath(navigator.GetPathFromMe(transform.position, bar.position));
                break;

            case BartenderState.pickingUpBarell:
                animator.SetBool("IsWalking", false);
                job.Wait(3);
                break;

            case BartenderState.walkingToRelaxRoom:
                animator.SetBool("IsWalking", true);
                job.FollowPath(navigator.GetPathFromMe(transform.position, chillRoomNavNode.position));
                break;

            case BartenderState.relaxing:
                animator.SetBool("IsWalking", false);
                job.Wait(40);
                break;
        }
    }


    private void EnableDialogue(string output) {
        animator.SetBool("IsWalking", movingStateBeforeDailogue);
        dialogueTrigger.SetIsEnabled(true);
    }



    private enum BartenderState {
        idleAtBar,
        pouringBeer,
        goToTable,
        puttingBeerOnTheTable,
        goToBar,
        pickingUpBarell,
        walkingToRelaxRoom,
        relaxing,
    }

}
