using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using UnityEngine;

public class PlayerLightAttack : PlayerAction {

    [SerializeField] private DamageProducer hitArea;
    [SerializeField] private GameObject lightWeapon;
    [SerializeField] private PlayerAnimatorOutputManager outputManager;

    private PlayerController controller;
    private Animator animator;


    private void Start() {
        controller = GetComponent<PlayerController>();
        animator = controller.GetAvatarAnimator();
        outputManager = controller.GetPlayerAnimatorOutputManager();

        outputManager.lightAttackEvent.AddListener(Damage);
        outputManager.lightAttackEvent.AddListener(LightAttackEnd);
    }

    public override void StartAction()
    {
        IsActive = true;
        controller.EquipItem(lightWeapon);
        animator.SetTrigger("lightAttack");
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
        if (IsActive && output == PlayerAnimatorOutput.lightAttackDamage) {
            hitArea.Attack();
        }
    }

    private void LightAttackEnd(PlayerAnimatorOutput output) {
        if (IsActive && output == PlayerAnimatorOutput.lightAttackEnd) {
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
