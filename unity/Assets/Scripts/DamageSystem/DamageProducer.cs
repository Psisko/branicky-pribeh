using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DamageProducer : MonoBehaviour {

    [SerializeField] private int dmg;
    [SerializeField] private bool knockback;
    [SerializeField] private List<Team> damagesTeams;
    [SerializeField] private bool isParryable;
    [SerializeField] GameObject attacker;

    public UnityEvent hitEvent;
    public UnityEvent parryEvent;

    private HashSet<Collider2D> collisions = new();



    private void OnTriggerEnter2D(Collider2D collision) {

        collisions.Add(collision);

    }



    private void OnTriggerExit2D(Collider2D collision) {

        collisions.Remove(collision);

    }



    public void Attack() {

        foreach (Collider2D collision in collisions) {
            if (collision != null) {
                DamageOpponent(collision);
            }

        }

    }

    public IEnumerator ChangeDamageTemporarily()
    {
        int previousValue = dmg;
        dmg = 2 * previousValue;
        yield return new WaitForSeconds(30f);
        dmg = previousValue;
    }

    /// <summary>
    /// Checks if the receiver is on on the opposite team, then deal damage
    /// </summary>
    private void DamageOpponent(Collider2D collision) {

        DamageableObject reciever = collision.gameObject.GetComponent<DamageableObject>();
        if (reciever == null || !reciever.IsTeam(damagesTeams)) {
            return;
        }

        DamageData damage = new DamageData(dmg, isParryable, knockback, attacker.transform.position);

        ManageResponse(reciever.RecieveDamage(damage));

    }

    private void ManageResponse(DamageResponseData response) {

        if (response.wasParried) {
            parryEvent.Invoke();
        }

        if (response.isSolid) {
            hitEvent.Invoke();
        }

    }
}