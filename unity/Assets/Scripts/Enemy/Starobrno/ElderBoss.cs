using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Events;
using Color = UnityEngine.Color;

public class ElderBoss : DamageableObject
{

    [SerializeField] private int maxHealth;
    private int health;

    [Header("Attacking")]
    [SerializeField] private Trigger trigger;
    [SerializeField] private DamageProducer firstAttack;
    [SerializeField] private DamageProducer secondAttackAndKick;
    [SerializeField] private float attackDistance;

    [SerializeField] private float attackSpeedMultiplier = 1f;

    [Header("Getting damaged")]
    [SerializeField] private float pushForce = 2000f;


    [SerializeField] private List<Transform> scatterPoints;
    [SerializeField] private EventController eventController;

    public UnityEvent<Transform> onDeathEvent;

    private Transform player;
    private Transform target;
    private Status status = Status.attackPreparing;
    private Coroutine changeStatusCoroutine;

    private EnemyMovement mover;
    private Animator animator;
    private ScatterPointManager scatterPointManager;
    private Rigidbody2D rb;

    public UnityEvent<int, int> livesChangeEvent = new();

    private void Awake()
    {
        if (!TryGetComponent<EnemyMovement>(out mover)) Debug.LogError("EnemyMovement component not found!", gameObject);
        if (!TryGetComponent<Animator>(out animator)) Debug.LogError("Animator component not found!", gameObject);
        if (!TryGetComponent<ScatterPointManager>(out scatterPointManager)) Debug.LogError("ScatterPointManager component not found!", gameObject);
        if (!TryGetComponent<Rigidbody2D>(out rb)) Debug.LogError("Rigidbody2D component not found!", gameObject);
        health = maxHealth;
    }

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        trigger.activationEvent.AddListener(OnAttackAvailable);
        scatterPointManager.SetScatterPoints(scatterPoints);
        animator.SetFloat("AttackSpeedMultiplier", attackSpeedMultiplier);

        ChangeStatus(Status.attackPreparing);
    }

    private void FixedUpdate()
    {

        switch (status)
        {
            case Status.attackPreparing:
                MoveTowardsPlayer();
                break;
            case Status.scatter:
                MoveScatter();
                break;
        }
    }

    public int GetMaxLives() { return maxHealth; }

    public int GetLives() { return health; }

    /// <summary>
    /// Handles recieving damage, knockback and eventual death
    /// </summary>
    /// <returns></returns>
    public override DamageResponseData RecieveDamage(DamageData damageData)
    {
        if (mover.dead == true)
            return new DamageResponseData(isSolid, false);
        // apply damage
        health -= damageData.damage;
        livesChangeEvent.Invoke(health, maxHealth);
        // death
        if (health <= 0)
        {
            eventController.EndEvent();
            mover.dead = true; // to stop the corpse from moving
            livesChangeEvent.Invoke(health, maxHealth);
            GetComponent<CircleCollider2D>().isTrigger = true; // to avoid collisions
            onDeathEvent.Invoke(transform); // drop items
            animator.SetTrigger("Die");
            Destroy(gameObject, 10.4f);
            return new DamageResponseData(isSolid, false);
        }
        // apply hit
        ForceChangeStatus(Status.gettingHit);

        // apply knockback
        if (damageData.isKnockable)
        {
            rb.AddForce(((Vector2)transform.position - (Vector2)damageData.damageOrigin).normalized * pushForce);
        }
            
        return new DamageResponseData(isSolid, false);
    }

    /// <summary>
    ///     Default endings of statuses.
    /// </summary>
    private void EndStatus(Status s)
    {
        if (s != status)
        {
            return;
            //Debug.LogWarning($"Status that should be ending ({s}) " +
            //    $"doesn't match the current status ({status})", gameObject);
        }

        switch (status)
        {
            case Status.idling:
                    ChangeStatus(Status.attackPreparing);
                break;

            case Status.attackPreparing:
                ChangeStatus(Status.scatter);
                break;

            case Status.attacking:
                ChangeStatus(Status.scatter);
                break;

            case Status.kicking:
                ChangeStatus(Status.scatter);
                break;

            case Status.scatter:
                    ChangeStatus(Status.attackPreparing);
                break;

            case Status.gettingHit:
                ChangeStatus(Status.scatter);
                break;

        }
    }

    private void ChangeStatus(Status newStatus)
    {
        switch (newStatus)
        {
            case Status.idling:
                status = Status.idling;
                animator.SetBool("IsWalking", false);
                Face(player.position);
                changeStatusCoroutine = StartCoroutine(
                    EndStatusDelayed(Status.idling, Random.Range(0.5f, 2f))
                );
                break;

            case Status.attackPreparing:
                status = Status.attackPreparing;
                animator.SetBool("IsWalking", true);
                break;

            case Status.attacking:
                status = Status.attacking;
                animator.SetBool("IsWalking", false);
                animator.SetTrigger("Attack");
                break;

            case Status.kicking:
                status = Status.kicking;
                animator.SetBool("IsWalking", false);
                animator.SetTrigger("Kick");
                break;

            case Status.scatter:
                status = Status.scatter;
                animator.SetBool("IsWalking", true);
                target = scatterPointManager.GetSafe(player.position);
                break;

            case Status.gettingHit:
                status = Status.gettingHit;
                animator.SetBool("IsWalking", false);
                animator.SetTrigger("GetHit");
                break;

        }

    }

    private void ForceChangeStatus(Status s)
    {
        if (changeStatusCoroutine != null)
            StopCoroutine(changeStatusCoroutine);
        ChangeStatus(s);
    }

    private IEnumerator EndStatusDelayed(Status s, float delay)
    {
        yield return new WaitForSeconds(delay);
        EndStatus(s);
    }
    /// <summary>
    /// Handles which random attack pattern
    /// </summary>
    public void OnAttackAvailable()
    {
        if (status == Status.attackPreparing
            || status == Status.idling)
        {
            if(Random.Range(0, 5) >= 1)
                ForceChangeStatus(Status.attacking);
            else
                ForceChangeStatus(Status.kicking);
        }
    }

    /// <summary>
    ///     Animator should call this on the frame the hit connects with the player.
    /// </summary>
    public void DealLightDamage()
    {
        firstAttack.Attack();
    }

    public void DealKnockBackDamage()
    {
        secondAttackAndKick.Attack();
    }

    /// <summary>
    ///     Animator should call this on the last frame of the attack animation.
    /// </summary>
    public void FinishAttack()
    {
        EndStatus(Status.attacking);
    }

    /// <summary>
    ///     Animator should call this on the last frame of the attack animation.
    /// </summary>
    public void FinishKick()
    {
        EndStatus(Status.kicking);
    }

    /// <summary>
    ///     Animator should call this on the last frame of the getting hit animation.
    /// </summary>
    public void EndGettingHit()
    {
        if (status == Status.gettingHit)
        {
            EndStatus(Status.gettingHit);
        }
        //else {
        //    Debug.LogWarning($"getttingHit status should be ending, but the status is {status}.", gameObject);
        //}
    }

    private void MoveTowardsPlayer()
    {
        Vector2 point = player.position;
        float offset = (transform.position.x - player.position.x) > 0 ? attackDistance : -attackDistance;
        point.x += offset;

        mover.MoveTowardsPoint(point);
        Face(player.position);
    }


    private void MoveScatter()
    {

        // Check if destination is achieved.
        if (Vector2.Distance(transform.position, target.position) < 0.27f)
        {
            EndStatus(Status.scatter);
            return;
        }

        // Destination has not been achieved.
        if (IsPlayerBlockingPath(target.position))
        {
            Vector2 point = transform.position;
            Vector2 dir = player.position - transform.position;
            if (IsIn1stOr3rdQuadrant(player.position, point))
            {
                dir = new Vector2(dir.y, -dir.x);
            }
            else
            {
                dir = new Vector2(-dir.y, dir.x);
            }
            mover.MoveTowardsPoint(point + dir);
        }
        else
        {
            mover.MoveTowardsPoint(target.position);
        }
        Face(target.position);
    }

    private void Face(Vector2 point)
    {
        float dir = point.x - transform.position.x;
        if (dir > 0)
        {
            transform.localScale = new(Mathf.Abs(transform.localScale.x), transform.localScale.y);
        }
        else if (dir < 0)
        {
            transform.localScale = new(-Mathf.Abs(transform.localScale.x), transform.localScale.y);
        }
    }

    /// <summary>
    ///     Checks if player is blocking horizontally a path towards a point
    /// </summary>
    /// <param name="point">Target point</param>
    /// <returns>True if blocking, false otherwise</returns>
    private bool IsPlayerBlockingPath(Vector2 point)
    {
        Vector2 toPlayer = player.position - transform.position;
        if (toPlayer.magnitude > 2)
        {
            return false;
        }

        Vector2 flatDirToTarget = point - (Vector2)transform.position;
        flatDirToTarget.y = 0;
        if (Vector2.Angle(flatDirToTarget, toPlayer) < 80)
        {
            return true;
        }
        return false;
    }

    private bool IsIn1stOr3rdQuadrant(Vector2 centre, Vector2 point)
    {
        Vector2 dir = point - centre;
        // Checks if x and y have the same sign (plus*plus=plus and minus*minus=plus)
        if (dir.x * dir.y >= 0)
            return true;
        else
            return false;
    }

    private enum Status
    {
        idling,
        attackPreparing,
        attacking,
        kicking,
        scatter,
        gettingHit,
    }

}

