using System.Collections;
using UnityEngine;

public class KidController : DamageableObject {

    [SerializeField] private Animator animator;

    private Transform[] scatterPoints;
    private Transform[] exitPoints;
    private EnemyMovement movement;

    private int scatterIndex;
    private int exitIndex;
    private bool beenHit = false;

    private void Awake() {
        movement = GetComponent<EnemyMovement>();
        if (movement == null ) { Debug.LogError("No EnemyMovement found!", gameObject); }
    }

    // Update is called once per frame
    void Update() {
        Vector2 point;
        if (!beenHit) {
            if (Vector2.Distance(transform.position, scatterPoints[scatterIndex].position) < 0.5f) {
                scatterIndex = Random.Range(0, scatterPoints.Length);
            }
            point = scatterPoints[scatterIndex].position;
        } else {
            point = exitPoints[exitIndex].position;
        }
        movement.MoveTowardsPoint(point);
        Face(point);
    }
    /// <summary>
    /// Handles running away after getting hit
    /// </summary>
    /// <returns></returns>
    public override DamageResponseData RecieveDamage(DamageData damageData) {
        beenHit = true;
        exitIndex = Random.Range(0, exitPoints.Length);
        animator.SetTrigger("hit");
        GetComponent<CircleCollider2D>().isTrigger = true;
        GetComponent<ItemDropper>().Drop();
        movement.SetOtherEnemies(null);
        movement.speed *= 1.7f;
        return new DamageResponseData(isSolid, false);
    }

    public void SetPoints(Transform[] scatterPoints, Transform[] exitPoints) {
        this.scatterPoints = scatterPoints;
        this.exitPoints = exitPoints;
        scatterIndex = Random.Range(0, this.scatterPoints.Length);
        exitIndex = Random.Range(0, this.exitPoints.Length);
    }

    private void Face(Vector2 point) {
        // Kid is by default facing left
        if (point.x - transform.position.x > 0 && transform.localScale.x > 0
            || point.x - transform.position.x < 0 && transform.localScale.x < 0) {
            Vector3 newScale = transform.localScale;
            newScale.x = -newScale.x;
            transform.localScale = newScale;
        }
    }
}
