using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
///     This class handles enemy simple movement. It just moves the rigidbody towards a point.
///     It can also avoid other enemies. 
/// </summary>
[Serializable]
public class EnemyMovement : MonoBehaviour {

    public float speed = 8;

    [SerializeField, Tooltip("How much should this enemy try to avoid other enemies (values between 0 and 1)")]
    private float avoidanceForce = 0.5f;
    [SerializeField, Tooltip("Max distance to enemies that should this enemy avoid")]
    private float avoidanceNoticeDistance = 2f;

    private List<Transform> enemyList;
    private Rigidbody2D rb;

    public bool dead = false;

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
    }

    public void SetOtherEnemies(List<Transform> el) {
        enemyList = el;
    }

    /// <summary>
    ///     Moves this enemy closer to the point. Needs to be called every frame.
    /// </summary>
    public void MoveTowardsPoint(Vector3 point) {
        if(dead == true) 
            return;

        Vector2 movementDir = (point - transform.position).normalized;
        Vector2 avoidanceVector = AvoidEnemies();

        rb.velocity = (movementDir + avoidanceVector * avoidanceForce).normalized * speed;
    }

    /// <summary>
    ///     Instantly moves this enemy to the desired point.
    /// </summary>
    public void Teleport(Vector3 point) {
        transform.position = point;
    }

    /// <summary>
    ///     Finds some direction to try and avoid other enemies
    /// </summary>
    /// <returns></returns>
    private Vector2 AvoidEnemies() {
        if (enemyList == null) 
            return Vector2.zero;

        Vector2 finalVector = Vector2.zero;
        foreach (Transform t in enemyList) {
            if (t == transform) continue;

            float enemyDistance = Vector2.Distance(transform.position, t.position);
            if (enemyDistance < avoidanceNoticeDistance) {
                finalVector += (Vector2)(t.position - transform.position).normalized * 1 / enemyDistance;
            }

        }

        // Needs to be negated because finalVector points to direction we don't want to go
        return -finalVector.normalized;
    }

}
