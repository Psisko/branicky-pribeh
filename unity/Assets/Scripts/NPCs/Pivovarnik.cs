using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(NpcJobs))]
public class Pivovarnik : MonoBehaviour
{
    [SerializeField] private NpcDialogue defaultDialogue;
    [SerializeField] private NpcDialogue branikBrokenDialogue;
    [SerializeField] private NpcDialogue branikWorkingDialogue;
    [SerializeField] private NpcDialogue breakRoomDialogue;

    [SerializeField] private List<Transform> machines;

    [SerializeField] private Transform upperSpawnPoint;
    [SerializeField] private Transform lowerSpawnPoint;
    [SerializeField] private Transform microwave;
    [SerializeField] private Transform armchair;

    [SerializeField] private Navigator navigator;
    [SerializeField] private Animator animator;
    [SerializeField] private Trigger dialogueTrigger;

    private PivovarnikState state;
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
        branikState.changeEvent.AddListener(ChangeBehaviour);


        if (branikState.AllMachinesOwned()) 
        {
            job.Teleport(upperSpawnPoint.position);
            ChangeState(PivovarnikState.goingToMachine);
        } 
        else 
        {
            job.Teleport(lowerSpawnPoint.position);
            ChangeState(PivovarnikState.goingToMicrowave);

        }
        
    }

    public void Talk() {
        List<NpcDialogue> availableDialogue = new();

        if (branikState.AllMachinesOwned())
            availableDialogue.Add(branikWorkingDialogue);
        if (!branikState.AllMachinesOwned())
            availableDialogue.Add(branikBrokenDialogue);
        if (state == PivovarnikState.relaxing)
            availableDialogue.Add(breakRoomDialogue);

        availableDialogue.Add(defaultDialogue);

        dialogueTrigger.SetIsEnabled(false);
        movingStateBeforeDailogue = animator.GetBool("IsWalking");
        animator.SetBool("IsWalking", false);
        job.dialogueOutputEvent.AddListener(EnableDialogue);
        job.PlayDialogue(availableDialogue);
    }


    public void EndState() {
        switch (state) {
            case PivovarnikState.operatingMachine:
                ChangeState(PivovarnikState.goingToMachine);
                break;

            case PivovarnikState.goingToMachine:
                ChangeState(PivovarnikState.operatingMachine);
                break;

            case PivovarnikState.goingToMicrowave:
                ChangeState(PivovarnikState.goingToArmchair);
                break;

            case PivovarnikState.goingToArmchair:
                ChangeState(PivovarnikState.relaxing);
                break;

            case PivovarnikState.relaxing:
                ChangeState(PivovarnikState.goingToMicrowave);
                break;


        }
    }

    private void ChangeState(PivovarnikState newState) {
        state = newState;

        switch (state) {

            case PivovarnikState.operatingMachine:
                animator.SetBool("IsWalking", false);
                job.Wait(Random.Range(3, 8));
                break;

            case PivovarnikState.goingToMachine:
                animator.SetBool("IsWalking", true);
                job.FollowPath(navigator.GetPathFromMe(transform.position, GetRandomMachine().position));
                break;

            case PivovarnikState.goingToMicrowave:
                animator.SetBool("IsWalking", true);
                job.FollowPath(navigator.GetPathFromMe(transform.position, microwave.position));
                break;

            case PivovarnikState.goingToArmchair:
                animator.SetBool("IsWalking", true);
                job.FollowPath(navigator.GetPathFromMe(transform.position, armchair.position));
                break;

            case PivovarnikState.relaxing:
                animator.SetBool("IsWalking", false);
                job.Wait(20);
                break;


        }
    }


    private void EnableDialogue(string output) {
        animator.SetBool("IsWalking", movingStateBeforeDailogue);
        dialogueTrigger.SetIsEnabled(true);
    }

    private Transform GetRandomMachine() {
        return machines[Random.Range(0, machines.Count)];
    }

    /// <summary>
    ///     Changes behaviour from doing nothing to doing something. If all machines are owned.
    /// </summary>
    private void ChangeBehaviour() {
        if (branikState.AllMachinesOwned()) {
            job.Teleport(upperSpawnPoint.position);
            ChangeState(PivovarnikState.goingToMachine);
        }
    }



    private enum PivovarnikState {
        operatingMachine,
        goingToMachine,
        goingToMicrowave,
        relaxing,
        goingToArmchair,
    }

}
