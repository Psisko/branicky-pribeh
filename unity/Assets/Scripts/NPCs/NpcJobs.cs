using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using Color = UnityEngine.Color;

[RequireComponent(typeof(Rigidbody2D))]
public class NpcJobs : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private Transform sprite;
    [SerializeField] private float forgivnessDistance = 0.2f;

    [HideInInspector] public UnityEvent jobDoneEvent;
    [HideInInspector] public UnityEvent<string> dialogueOutputEvent;

    private bool isInDialogue = false;
    private bool notWalking = true;
    private bool finishJobUponDialogEnd = false;
    
    private List<Vector3> path;
    private int pathIndex;

    private Rigidbody2D rb;
    private DialogueUI dialogueUI;
    private PlayerController player;

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start() {
        dialogueUI = GameObject.FindAnyObjectByType<DialogueUI>();
        GameObject playerGO = GameObject.FindGameObjectWithTag("Player");
        if (playerGO == null) Debug.LogError("Player not found", gameObject);
        player = playerGO.GetComponent<PlayerController>();
    }

    private void FixedUpdate() {
        if (notWalking || isInDialogue) {
            return;
        }

        Move();
    }

    /// <summary>
    ///     Sets the path following as current job.
    /// </summary>
    /// <param name="newPath"></param>
    public void FollowPath(List<Vector3> newPath) {
        path = newPath;
        pathIndex = 0;
        notWalking = false;
    }

    public void Wait(float seconds) {
        notWalking = true;
        StartCoroutine(WaitCoroutine(seconds));
    }

    /// <summary>
    ///     Plays dialogue. Pauses the current job.
    /// </summary>
    public void PlayDialogue(DialogueSO dialogueData) {
        isInDialogue = true;
        player.SetControlBlocked(true);
        Face(player.transform.position);
        dialogueUI.dialogueEndEvent.AddListener(EndDialogue);
        bool success = dialogueUI.StartDialogue(dialogueData);
        if (!success) {
            EndDialogue("");
        }
    }

    public void PlayDialogue(List<NpcDialogue> options) {
        // Filter unplayable dialogue
        List<NpcDialogue> playable = new();
        foreach (NpcDialogue n in options) { 
            if (!n.onlyOnce || (n.onlyOnce && !n.HasBeenPlayed()))
                playable.Add(n);
        }

        // No suitable dialogue
        if (playable.Count == 0) {
            Debug.LogError("Npc must always have dialogue!", gameObject);
        }

        // Sort the dialogue list by lastPlayed
        playable.Sort((first, second) => first.lastPlayed.CompareTo(second.lastPlayed));

        // Play the dialogue
        playable[0].lastPlayed = Time.time;
        PlayDialogue(playable[0].data);

    }

    /// <summary>
    ///     This method is called when the dialogue ends.
    /// </summary>
    private void EndDialogue(string output) {
        isInDialogue = false;
        player.SetControlBlocked(false);
        dialogueUI.dialogueEndEvent.RemoveListener(EndDialogue);
        dialogueOutputEvent.Invoke(output);

        if (finishJobUponDialogEnd) {
            finishJobUponDialogEnd = false;
            FinishJob();
        }
    }

    /// <summary>
    ///     Teleports the Npc to given point.
    /// </summary>
    public void Teleport(Vector2 point) {
        transform.position = point;
    }

    private void Move() {
        Vector2 movementVector = path[pathIndex] - transform.position;
        if (movementVector.sqrMagnitude < forgivnessDistance*forgivnessDistance) {
            pathIndex++;
            if (pathIndex >= path.Count) {
                FinishJob();
                return;
            }
        }


        Face(path[pathIndex]);

        rb.velocity = movementVector.normalized * speed;
    }

    private IEnumerator WaitCoroutine(float seconds) {
        yield return new WaitForSeconds(seconds);
        FinishJob();
    }

    private void FinishJob() {
        if (isInDialogue) {
            finishJobUponDialogEnd = true;
        } else {
            notWalking = true;
            jobDoneEvent.Invoke();
        }
    }

    private void Face(Vector2 point) {
        // get the direction from gameobject origin, but change the orientation of only sprite
        float dir = point.x - transform.position.x;
        if (dir > 0) {
            sprite.localScale = new(Mathf.Abs(sprite.localScale.x), sprite.localScale.y);
        } else if (dir < 0) {
            sprite.localScale = new(-Mathf.Abs(sprite.localScale.x), sprite.localScale.y);
        }
    }

    private void OnDrawGizmos() {
        if (path != null) {
            Gizmos.color = Color.blue;
            for (int i = 0; i < path.Count - 1; i++) {
                Gizmos.DrawLine(path[i], path[i + 1]);
            }
        }
    }

}
