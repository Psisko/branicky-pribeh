using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class DragonController : DamageableObject {

    [SerializeField] private int maxLives = 25;
    private int lives;
    [SerializeField] private Animator animator;

    [SerializeField] private DamageProducer attack;

    [SerializeField] private EventController eventController;
    public UnityEvent<int, int> livesChangeEvent = new();

    public ProjectileBehaviour projectileFireBall;
    public Transform LaunchOffset;

    private Coroutine stateChanging;
    // so the dragon doesnt take damage
    private DragonState state = DragonState.sitting;


    private void Awake() {
        lives = maxLives;
    }

    private void Start() {
        eventController.eventStart.AddListener(StartFight);
    }

    private void StartFight() {
        ChangeState(DragonState.attacking);
    }

    private void EndFight() {
        eventController.EndEvent();
    }

    // called by Animator
    public void Attack()
    {
        attack.Attack();
    }

    public int GetMaxLives() { return maxLives; }

    public int GetLives() { return lives; }

    /// <summary>
    /// Handles recieving damage and eventual death
    /// </summary>
    /// <returns></returns>
    public override DamageResponseData RecieveDamage(DamageData damageData) {

        if (state == DragonState.dead) {
            return new(isSolid, false);
        }
        lives -= damageData.damage;
        livesChangeEvent.Invoke(lives, maxLives);
        if (lives <= 0) {
            ChangeState(DragonState.dead);
        } else {
            animator.SetTrigger("getHit");
        }

        return new(isSolid, false);
    }


    private void EndState() {
        switch (state) {
            case DragonState.sitting:
                if (Random.Range(1, 5) == 1)
                    ChangeState(DragonState.fireAttack);
                else
                    ChangeState(DragonState.attacking);
                break;
            case DragonState.attacking:
                ChangeState(DragonState.sitting);
                break;
            case DragonState.dead:
                livesChangeEvent.Invoke(lives, maxLives);
                //Invoke(nameof(EndFight), 10f);
                return;
            case DragonState.fireAttack:
                ChangeState(DragonState.sitting);
                break;
        }
    }

    private IEnumerator EndStateDelayed(float delay) {
        yield return new WaitForSeconds(delay);
        EndState();
    }

    private IEnumerator WaitForAnimation(float delay)
    {
        yield return new WaitForSeconds(delay);
    }

    private void ChangeState(DragonState newState) {


        state = newState;
        switch (state) {
            case DragonState.sitting:
                stateChanging = StartCoroutine(EndStateDelayed(2f));
                break;
            case DragonState.attacking:
                animator.SetTrigger("attack");
                stateChanging = StartCoroutine(EndStateDelayed(2f));
                break;
            case DragonState.dead:
                animator.SetTrigger("death");
                livesChangeEvent.Invoke(lives, maxLives);
                Invoke(nameof(EndFight), 10f);
                break;
            case DragonState.fireAttack:
                animator.SetTrigger("fireAttack");
                stateChanging = StartCoroutine(FireAttackSequence());
                break;
        }
    }
    /// <summary>
    /// Handles creating fireballs in the scene with different Ypositions
    /// </summary>
    /// <returns></returns>
    IEnumerator FireAttackSequence()
    {
        yield return new WaitForSeconds(2.5f);
        for (int i = 0; i < 6; i++)
        {
            float randomY = Random.Range(0, 1.2f);
            Instantiate(projectileFireBall, LaunchOffset.position + new Vector3(0f, randomY, 0f), transform.rotation);
            yield return new WaitForSeconds(0.5f);
        }
        yield return StartCoroutine(EndStateDelayed(2f));
    }

    private enum DragonState {
        sitting,
        fireAttack,
        attacking,
        dead
    }
}