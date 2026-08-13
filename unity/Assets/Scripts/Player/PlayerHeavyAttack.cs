using System.Collections;
using UnityEngine;

public class PlayerHeavyAttack : PlayerAction {

    [SerializeField] private DamageProducer hitArea;
    [SerializeField] private GameObject heavyWeapon;

    private PlayerController controller;
    private Animator animator;
    private PlayerAnimatorOutputManager outputManager;

    private void Start() {
        controller = GetComponent<PlayerController>();
        animator = controller.GetAvatarAnimator();
        outputManager = controller.GetPlayerAnimatorOutputManager();

        outputManager.outputEvent.AddListener(Damage);
        outputManager.outputEvent.AddListener(HeavyAttackEnd);
    }


    public override void StartAction() {
        IsActive = true;
        animator.SetTrigger("heavyAttack");
        controller.SetMovementEnabled(false);
        controller.EquipItem(heavyWeapon);
    }


    public override void InterruptAction() {
        if (IsActive) {
            Cleanup();
        }
    }

    /// <summary>
    ///     Does the damage.
    /// </summary>
    private void Damage(PlayerAnimatorOutput output) {
        if (IsActive && output == PlayerAnimatorOutput.heavyAttackDamage) {
            hitArea.Attack();
        }
    }

    private void HeavyAttackEnd(PlayerAnimatorOutput output) {
        if (IsActive && output == PlayerAnimatorOutput.heavyAttackEnd) {
            Cleanup();
        }
    }

    /// <summary>
    ///     Restores everything to neutral state.
    /// </summary>
    private void Cleanup() {
        IsActive = false;
        controller.SetMovementEnabled(true);
    }

}
