using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using UnityEngine;

public class PlayerLightAttack2 : PlayerAction {

    [SerializeField] private DamageProducer hitArea;
    [SerializeField] private GameObject lightWeapon;

    private PlayerController controller;
    private Animator animator;
    private PlayerAnimatorOutputManager outputManager;

    private void Start() {
        controller = GetComponent<PlayerController>();
        animator = controller.GetAvatarAnimator();
        outputManager = controller.GetPlayerAnimatorOutputManager();

        outputManager.lightAttack2Event.AddListener(Damage2);
        outputManager.lightAttack2Event.AddListener(LightAttackEnd2);
    }

    public override void StartAction()
    {
        IsActive = true;
        controller.EquipItem(lightWeapon);
        animator.SetTrigger("lightAttack2");

    }
    public override void InterruptAction() {
        if (IsActive) {
            Cleanup();
        }


    }
    public bool ExitAttack()
    {
        if (animator.GetCurrentAnimatorStateInfo(1).normalizedTime > 0.9 && animator.GetCurrentAnimatorStateInfo(1).IsTag("Attack"))
        {
            return true;

        }
        return false;
    }

    /// <summary>
    ///     Does the damage.
    /// </summary>
    private void Damage2(PlayerAnimatorOutput output) {
        if (IsActive && output == PlayerAnimatorOutput.lightAttackDamage2) {
            hitArea.Attack();
        }
    }

    private void LightAttackEnd2(PlayerAnimatorOutput output) {
        if (IsActive && output == PlayerAnimatorOutput.lightAttackEnd2) {
            Cleanup();
        }
    }

    /// <summary>
    ///     Restores everything to neutral state.
    /// </summary>
    private void Cleanup() {
        IsActive = false;
    }

}
