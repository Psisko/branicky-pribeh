using System.Collections;
using UnityEngine;

public class PlayerParry : PlayerAction {

    //[SerializeField] private float parryLength = 0.2f;
    //[SerializeField] private float recoveryLength = 0.5f;
    [SerializeField] private GameObject shield;

    private Coroutine coroutine;
    private PlayerController controller;
    private Animator animator;


    private void Start() {
        controller = GetComponent<PlayerController>();
        animator = controller.GetAvatarAnimator();

    }

    public override void StartAction() {
        controller.SetMovementEnabled(false);
        controller.EquipItem(shield);
        Perform();
    }


    public override void InterruptAction() {
        Cleanup();
    }


    public bool TryParryDamage(DamageData damage) {
        if (!damage.isParryable || !IsActive) {
            return false;
        }

        // Damage can be parried and player is parrying, check if damage comes from front.
        if ((damage.damageOrigin.x - transform.position.x) * transform.localScale.x > 0) {
            return true;
        }
        return false;
    }

    /// <summary>
    ///     Starts the parry.
    /// </summary>
    private void Perform() {
        IsActive = true;
        animator.SetBool("isParrying", true);
    }

    /// <summary>
    ///     Restores everything to neutral state.
    /// </summary>
    private void Cleanup() {
        IsActive = false;
        animator.SetBool("isParrying", false);
        controller.SetMovementEnabled(true);
        controller.EquipItem(null);
    }

}
