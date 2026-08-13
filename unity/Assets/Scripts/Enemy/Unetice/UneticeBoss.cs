using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

[RequireComponent(typeof(EnemyMovement))]
[RequireComponent(typeof(ItemDropper))]
public class UneticeBoss : DamageableObject {

    [SerializeField] private int maxLives = 5; // bylo tady 500
    [SerializeField, Tooltip("Defines points between which the boss moves. Every row must contain " +
        "left point (out of the screen) and right point (in resting area). Must be 2x2 matrix.")] 
        private List<Row> navpoints;
    [SerializeField] private float preparingLength;
    [SerializeField] private float normalSpeed = 8;
    [SerializeField] private float attackSpeed = 30;
    [SerializeField, Range(0f, 1f)] private float attackProbability = 0.3f;
    [SerializeField] private Animator animator;
    [SerializeField] private EventController eventController;
    public UnityEvent<int, int> livesChangeEvent = new();


    private int lives;
    private int currentRow = 0;
    private BossStatus status = BossStatus.idle;
    private Vector3 target;

    private EnemyMovement movement;

    private void Awake() {
        target = navpoints[currentRow].right.position;
        movement = GetComponent<EnemyMovement>();
        lives = maxLives;
    }

    private void Start() {
        animator.SetBool("IsWalking", false);
        eventController.eventStart.AddListener(StartFight);
    }

    private void FixedUpdate() {

        if (lives <= 0)
            return;

        if (status == BossStatus.preparing || status == BossStatus.idle) {
            return;
        }
        float sqrDistanceFromTarget = (transform.position - target).sqrMagnitude;
        if (sqrDistanceFromTarget < 0.5 * 0.5) {
            EndStatus();
        }

        movement.MoveTowardsPoint(target);

    }

    public void StartFight() {
        animator.SetBool("IsWalking", true);
        ChangeAndStartStatus(BossStatus.changingRow);
    }
    /// <summary>
    /// Handles recieving damage and eventual death
    /// </summary>
    /// <returns></returns>
    public override DamageResponseData RecieveDamage(DamageData damageData) {
        animator.SetTrigger("hit");
        lives -= damageData.damage;
        // call UI to update
        livesChangeEvent.Invoke(lives, maxLives);
        if (lives <= 0) {
            GetComponent<ItemDropper>().Drop();
            animator.SetTrigger("death");
            eventController.EndEvent();
            Destroy(gameObject, 3.5f);
        }
        return new DamageResponseData(isSolid, false);
    }

    /// <summary>
    ///     Ends a status. Decides what status should be next.
    /// </summary>
    private void EndStatus() {
        switch (status) {
            case BossStatus.changingRow:
                if (Random.Range(0f, 1f) <= attackProbability) {
                    ChangeAndStartStatus(BossStatus.preparing);
                } else {
                    ChangeAndStartStatus(BossStatus.changingRow);
                }
                break;
            case BossStatus.preparing:
                // ends by coroutine
                break;
            case BossStatus.chargingLeft:
                ChangeAndStartStatus(BossStatus.chargingRight);
                break;
            case BossStatus.chargingRight:
                ChangeAndStartStatus(BossStatus.changingRow);
                break;
        }
    }

    /// <summary>
    ///     Changes the status ad setups the entity for the new status.
    /// </summary>
    /// <param name="newStatus"></param>
    private void ChangeAndStartStatus(BossStatus newStatus) {
        status = newStatus;
        switch (status) {
            case BossStatus.changingRow:
                ChangeRow();
                Face(true);
                movement.speed = normalSpeed;
                animator.SetFloat("speed", normalSpeed);
                target = navpoints[currentRow].right.position;
                break;

            case BossStatus.preparing:
                Face(true);
                movement.speed = attackSpeed;
                animator.SetFloat("speed", attackSpeed/2);
                StartCoroutine(ChangeStatusDelayed(BossStatus.chargingLeft));
                break;

            case BossStatus.chargingLeft:
                Face(true);
                movement.speed = attackSpeed;
                animator.SetFloat("speed", attackSpeed/3);
                target = navpoints[currentRow].left.position;
                break;

            case BossStatus.chargingRight:
                ChangeRow();
                Face(false);
                movement.speed = attackSpeed;
                animator.SetFloat("speed", attackSpeed/3);
                movement.Teleport(navpoints[currentRow].left.position);
                target = navpoints[currentRow].right.position;
                break;

        }
    }

    private IEnumerator ChangeStatusDelayed(BossStatus newStatus) {
        yield return new WaitForSeconds(preparingLength);
        ChangeAndStartStatus(newStatus);
    }

    private void Face(bool left) {
        if (left) {
            transform.localScale = Vector3.one;
        } else {
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }

    private void ChangeRow() {
        if (currentRow == 0)
            currentRow = 1;
        else
            currentRow = 0;
    }

    private enum BossStatus {
        idle,
        changingRow,
        preparing,
        chargingLeft,
        chargingRight,
    }

    public int GetMaxLives() { return maxLives; }

    public int GetLives() { return lives; }

    [Serializable]
    private class Row {
        public Transform left;
        public Transform right;
    }
}
