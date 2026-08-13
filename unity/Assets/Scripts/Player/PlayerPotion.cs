using System.Collections;
using UnityEngine;

public class PlayerPotion : PlayerAction {

    [SerializeField] private GameObject potion;

    private PlayerController controller;
    private Animator animator;
    private PlayerAnimatorOutputManager outputManager;
    private PlayerState playerState;

    private void Start() {
        controller = GetComponent<PlayerController>();
        animator = controller.GetAvatarAnimator();
        outputManager = controller.GetPlayerAnimatorOutputManager();

        outputManager.outputEvent.AddListener(PotionEnd);
    }

    
    public override void StartAction() {
        playerState = controller.GetPlayerState();

        if (playerState.GetHealthpacks() == 0)
            return;

        IsActive = true;
        animator.SetTrigger("drink");
        controller.SetMovementEnabled(false);
        controller.EquipItem(potion);
    }

    
    public override void InterruptAction() {
        if (IsActive) {
            Cleanup();
        }
    }

    private void PotionEnd(PlayerAnimatorOutput output)
    {
        // apply effect
        if (IsActive && output == PlayerAnimatorOutput.potionEnd)
        {
            playerState.UseHealthpack();
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
