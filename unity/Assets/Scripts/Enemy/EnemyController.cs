using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Events;
using Color = UnityEngine.Color;

public class EnemyController : DamageableObject, IEncounterEnemy {

    [SerializeField] private int maxHealth;
    private int health;

    [Header("Attacking")]
    [SerializeField] private Trigger trigger;
    [SerializeField] private DamageProducer attack;
    [SerializeField] private float attackDistance;

    [SerializeField] private float attackSpeedMultiplier = 1f;

    [Header("Getting damaged")]
    [SerializeField] private float pushForce = 100f;
    [SerializeField] private float stunLength = 2f;

    public UnityEvent<Transform> onDeathEvent;
    
    private Transform player;
    private Transform target;
    private Status status = Status.attackPreparing;
    private Coroutine changeStatusCoroutine;
    private bool hasPermissionToAttack = false;

    private EncounterController encounterCtrler;
    private EnemyMovement mover;
    private Animator animator;
    private ScatterPointManager scatterPointManager;
    private Rigidbody2D rb;


    private void Awake() {
        if (!TryGetComponent<EnemyMovement>(out mover)) Debug.LogError("EnemyMovement component not found!", gameObject);
        if (!TryGetComponent<Animator>(out animator)) Debug.LogError("Animator component not found!", gameObject);
        if (!TryGetComponent<ScatterPointManager>(out scatterPointManager)) Debug.LogError("ScatterPointManager component not found!", gameObject);
        if (!TryGetComponent<Rigidbody2D>(out rb)) Debug.LogError("Rigidbody2D component not found!", gameObject);
        health = maxHealth;
    }

    private void Start() {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        trigger.activationEvent.AddListener(OnAttackAvailable);
        attack.parryEvent.AddListener(Stun);
        animator.SetFloat("AttackSpeedMultiplier", attackSpeedMultiplier);

        ChangeStatus(Status.attackPreparing);
    }

    private void FixedUpdate() {
        switch (status) {
            case Status.attackPreparing:
                MoveTowardsPlayer();
                break;
            case Status.scatter:
                MoveScatter();
                break;
        }
    }
    /// <summary>
    /// Handles recieving damage, applying stun and knockback and eventual death
    /// </summary>
    /// <returns></returns>
    public override DamageResponseData RecieveDamage(DamageData damageData) {
        if (mover.dead == true) 
            return new DamageResponseData(isSolid, false);
        // apply damage
        health -= damageData.damage;
        // death
        if (health <= 0 ) {
            mover.dead = true; // to stop the corpse from moving
            GetComponent<CircleCollider2D>().isTrigger = true; // to avoid collisions
            onDeathEvent.Invoke(transform); // drop items
            animator.SetBool("IsStunned", false);
            animator.SetTrigger("Die");
            Destroy(gameObject, 3.0f);
            return new DamageResponseData(isSolid, false);
        }
        // apply stun
        if (status == Status.stunned) {
            animator.SetTrigger("GetHit");
        } else {
            ForceChangeStatus(Status.gettingHit);
        }
        // apply knockback
        if(damageData.isKnockable)
            rb.AddForce(((Vector2)transform.position - (Vector2)damageData.damageOrigin).normalized * pushForce);
        return new DamageResponseData(isSolid, false);
    }

    public void EncounterEnemyInit(EncounterController ec) {
        encounterCtrler = ec;
        mover.SetOtherEnemies(encounterCtrler.GetEnemies());
        scatterPointManager.SetScatterPoints(encounterCtrler.GetScatterPoints());
    }

    public void AddListenerToOnDeathEvent(UnityAction<Transform> call) {
        onDeathEvent.AddListener(call);
    }


    /// <summary>
    ///     Default endings of statuses.
    /// </summary>
    private void EndStatus(Status s) {
        if (s != status) {
            return;
            //Debug.LogWarning($"Status that should be ending ({s}) " +
            //    $"doesn't match the current status ({status})", gameObject);
        }

        switch (status) {
            case Status.idling:
                if (GetAttackPermission()) {
                    ChangeStatus(Status.attackPreparing);
                } else {
                    ChangeStatus(Status.scatter);
                }
                break;

            case Status.attackPreparing:
                // attack preparing should normally end in ForceChangeStatus
                ChangeStatus(Status.scatter);
                break;

            case Status.attacking:
                ChangeStatus(Status.scatter);
                break;

            case Status.scatter:
                if (GetAttackPermission()) {
                    ChangeStatus(Status.attackPreparing);
                } else {
                    ChangeStatus(Status.idling);
                }
                break;

            case Status.stunned:
                ChangeStatus(Status.scatter);
                break;

            case Status.gettingHit:
                ChangeStatus(Status.scatter);
                break;

        }
    }

    private void ChangeStatus(Status newStatus){
        switch (newStatus) {
            case Status.idling:
                status = Status.idling;
                SetAnimatorBools(false, false);
                Face(player.position);
                changeStatusCoroutine = StartCoroutine(
                    EndStatusDelayed(Status.idling, Random.Range(.5f, 2f))
                );
                break;

            case Status.attackPreparing:
                status = Status.attackPreparing;
                SetAnimatorBools(true, false);
                break;

            case Status.attacking:
                status = Status.attacking;
                SetAnimatorBools(false, false);
                animator.SetTrigger("Attack");
                break;

            case Status.scatter:
                status = Status.scatter;
                SetAnimatorBools(true, false);
                target = scatterPointManager.GetSafe(player.position);
                break;

            case Status.stunned:
                status = Status.stunned;
                SetAnimatorBools(false, true);
                changeStatusCoroutine = StartCoroutine(
                    EndStatusDelayed(Status.stunned, stunLength)
                );
                break;

            case Status.gettingHit:
                status = Status.gettingHit;
                SetAnimatorBools(false, false);
                animator.SetTrigger("GetHit");
                break;

        }

        ManageReturningAttackPermission();
    }

    private void ForceChangeStatus(Status s) {
        if (changeStatusCoroutine != null)
            StopCoroutine(changeStatusCoroutine);
        ChangeStatus(s);
    }

    private IEnumerator EndStatusDelayed(Status s, float delay) {
        yield return new WaitForSeconds(delay);
        EndStatus(s);
    }

    public void OnAttackAvailable() {
        if (status == Status.attackPreparing
            || status == Status.idling) {
            ForceChangeStatus(Status.attacking);
        }
    }

    /// <summary>
    ///     Animator should call this on the frame the hit connects with the player.
    /// </summary>
    public void DealDamage() {
        attack.Attack();
    }

    /// <summary>
    ///     Animator should call this on the last frame of the attack animation.
    /// </summary>
    public void FinishAttack() {
        EndStatus(Status.attacking);
    }

    /// <summary>
    ///     Animator should call this on the last frame of the getting hit animation.
    /// </summary>
    public void EndGettingHit() {
        if (status == Status.gettingHit) {
            EndStatus(Status.gettingHit);
        } 
    }

    private void MoveTowardsPlayer() {
        Vector2 point = player.position;
        float offset = (transform.position.x - player.position.x) > 0 ? attackDistance : -attackDistance;
        point.x += offset;

        mover.MoveTowardsPoint(point);
        Face(player.position);
    }


    private void MoveScatter() {

        // Check if destination is achieved.
        if (Vector2.Distance(transform.position, target.position) < 0.27f) {
            EndStatus(Status.scatter);
            return;
        }

        // Destination has not been achieved.
        if (IsPlayerBlockingPath(target.position)) { 
            Vector2 point = transform.position;
            Vector2 dir = player.position - transform.position;
            if (IsIn1stOr3rdQuadrant(player.position, point)) {
                dir = new Vector2(dir.y, -dir.x);
            } else {
                dir = new Vector2(-dir.y, dir.x);
            }
            mover.MoveTowardsPoint(point + dir);
        } else {
            mover.MoveTowardsPoint(target.position);
        }
        Face(target.position);
    }

    private void Face(Vector2 point) {
        float dir = point.x - transform.position.x;
        if (dir > 0) {
            transform.localScale = new(Mathf.Abs(transform.localScale.x), transform.localScale.y);
        } else if (dir < 0) {
            transform.localScale = new(-Mathf.Abs(transform.localScale.x), transform.localScale.y);
        }
    }

    private void Stun() {
        ForceChangeStatus(Status.stunned);
    }

    /// <summary>
    ///     Checks if player is blocking horizontally a path towards a point
    /// </summary>
    /// <param name="point">Target point</param>
    /// <returns>True if blocking, false otherwise</returns>
    private bool IsPlayerBlockingPath(Vector2 point) {
        Vector2 toPlayer = player.position - transform.position;
        if (toPlayer.magnitude > 2) {
            return false;
        }

        Vector2 flatDirToTarget = point - (Vector2)transform.position;
        flatDirToTarget.y = 0;
        if (Vector2.Angle(flatDirToTarget, toPlayer) < 80) {
            return true;
        }
        return false;
    }

    private bool IsIn1stOr3rdQuadrant(Vector2 centre, Vector2 point) {
        Vector2 dir = point - centre;
        if (dir.x * dir.y >= 0)
            return true;
        else
            return false;
    }

    private void SetAnimatorBools(bool isWalking, bool isStunned) {
        animator.SetBool("IsWalking", isWalking);
        animator.SetBool("IsStunned", isStunned);
    }

    private bool GetAttackPermission() {
        if (!encounterCtrler.IsAttackerSlotAvailable())
            return false;

        encounterCtrler.AddAttacker();
        hasPermissionToAttack = true;
        return true;
        
    }

    private void ManageReturningAttackPermission() {
        if (hasPermissionToAttack && status != Status.attacking && status != Status.attackPreparing) {
            encounterCtrler.RemoveAttacker();
            hasPermissionToAttack = false;
        }
    }

    private enum Status {
        idling,
        attackPreparing,
        attacking,
        scatter,
        stunned,
        gettingHit,
    }

}

