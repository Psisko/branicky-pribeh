using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using UnityEngine;

public class PlayerLightAttack1 : PlayerAction {

    [SerializeField] private DamageProducer hitArea;
    [SerializeField] private GameObject lightWeapon;

    private PlayerController controller;
    private Animator animator;
    private PlayerAnimatorOutputManager outputManager;

    private void Start() {
        controller = GetComponent<PlayerController>();
        animator = controller.GetAvatarAnimator();
        outputManager = controller.GetPlayerAnimatorOutputManager();

        outputManager.lightAttack1Event.AddListener(Damage1);
        outputManager.lightAttack1Event.AddListener(LightAttackEnd1);
    }

    public override void StartAction()
    {
        IsActive = true;
        controller.EquipItem(lightWeapon);
        animator.SetTrigger("lightAttack1");

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
    private void Damage1(PlayerAnimatorOutput output) {
        if (IsActive && output == PlayerAnimatorOutput.lightAttackDamage1) {
            hitArea.Attack();
        }
    }

    private void LightAttackEnd1(PlayerAnimatorOutput output) {
        if (IsActive && output == PlayerAnimatorOutput.lightAttackEnd1) {
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
