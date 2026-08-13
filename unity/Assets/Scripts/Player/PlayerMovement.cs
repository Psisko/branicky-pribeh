using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

    private float speed = 14f;
    private Animator avatarAnimator;

    [HideInInspector] public bool movementEnabled = true;

    private Vector3 movementDir = Vector3.zero;
    private Rigidbody2D rb;
    private Rectangle confines = null;

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
        avatarAnimator = GetComponent<PlayerController>().GetAvatarAnimator();
    }

    private void Update() {
        GetInput();
        if (movementDir.x > 0) {
            transform.localScale = new(1, transform.localScale.y);
        } else if (movementDir.x < 0) {
            transform.localScale = new(-1, transform.localScale.y);
        }

        if (movementDir == Vector3.zero) {
            avatarAnimator.SetBool("isRunning", false);
        } else {
            avatarAnimator.SetBool("isRunning", true);
        }

    }

    private void FixedUpdate() {

        Vector2 velocity = movementDir * speed;

        if (confines != null) {
            if (transform.position.x < confines.minX && velocity.x < 0) {
                velocity.x = 0;
            }
            if (transform.position.x > confines.maxX && velocity.x > 0) {
                velocity.x = 0;
            }
            if (transform.position.y < confines.minY && velocity.y < 0) {
                velocity.y = 0;
            }
            if (transform.position.y > confines.maxY && velocity.y > 0) {
                velocity.y = 0;
            }
        }

        rb.velocity = velocity;
    }

    public bool IsFacingRight() {
        return transform.localScale.x > 0;
    }

    public void SetConfinedArea(Rectangle rect) {
        confines = rect;
    }

    public void UnsetConfinedArea() {
        confines = null;
    }

    public IEnumerator ChangeSpeedTemporarily()
    {
        float previousValue = speed;
        speed = 2 * previousValue;
        if (speed > 25)
            speed = 25;

        yield return new WaitForSeconds(15f);
        speed = previousValue;
    }

    private void GetInput() {
        if (!movementEnabled) {
            movementDir = Vector3.zero;
            return;
        }

        movementDir.x = Input.GetAxisRaw("Horizontal");
        movementDir.y = Input.GetAxisRaw("Vertical");
        movementDir.Normalize();
    }
}
