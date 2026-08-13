using System.Collections;
using UnityEngine;

public class PlayerDamagePotion : PlayerAction
{

    [SerializeField] private GameObject potion;
    [SerializeField] private GameObject lightAttack;
    [SerializeField] private GameObject heavyAttack;

    private PlayerController controller;
    private Animator animator;
    private PlayerAnimatorOutputManager outputManager;
    private PlayerState playerState;
    private DamageProducer lightAttackDamageProducer;
    private DamageProducer heavyAttackDamageProducer;

    private void Start()
    {
        lightAttackDamageProducer = lightAttack.GetComponent<DamageProducer>();
        heavyAttackDamageProducer = heavyAttack.GetComponent<DamageProducer>();
        controller = GetComponent<PlayerController>();
        animator = controller.GetAvatarAnimator();
        outputManager = controller.GetPlayerAnimatorOutputManager();

        outputManager.outputEvent.AddListener(PotionEnd);
    }


    public override void StartAction()
    {
        playerState = controller.GetPlayerState();

        if (playerState.GetDamagePotions() == 0)
            return;

        IsActive = true;
        animator.SetTrigger("drink");
        controller.SetMovementEnabled(false);
        controller.EquipItem(potion);
    }


    public override void InterruptAction()
    {
        if (IsActive)
        {
            Cleanup();
        }
    }

    private void PotionEnd(PlayerAnimatorOutput output)
    {
        // apply effect
        if (IsActive && output == PlayerAnimatorOutput.potionEnd)
        {
            playerState.UseDamagePotion();
            StartCoroutine(lightAttackDamageProducer.ChangeDamageTemporarily());
            StartCoroutine(heavyAttackDamageProducer.ChangeDamageTemporarily());
            Cleanup();
        }
    }

    /// <summary>
    ///     Restores everything to neutral state.
    /// </summary>
    private void Cleanup()
    {
        IsActive = false;
        controller.SetMovementEnabled(true);
    }

}
