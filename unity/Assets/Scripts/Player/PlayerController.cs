using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Burst.Intrinsics;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class PlayerController : DamageableObject {

    [Header("Player Controller")]
    [SerializeField] private Animator avatarAnimator;
    [SerializeField] private PlayerAnimatorOutputManager animatorOutputManager;
    [SerializeField] private AudioManager audioManager;
    [SerializeField] private GameObject itemSlot;

    private bool controlBlocked = false;
    
    private PlayerLightAttack lightAttack;
    private PlayerLightAttack1 lightAttack1;
    private PlayerLightAttack2 lightAttack2;
    private PlayerHeavyAttack heavyAttack;
    private PlayerParry parry;
    private PlayerPotion healthPack;
    private PlayerDamagePotion damagePotion;
    private PlayerSpeedPotion speedPotion;
    private List<PlayerAction>  playerActions;

    private PlayerMovement playerMovement;
    private PlayerState playerState;

    private int comboStage = 0;       // 0 = lightAttack, 1 = lightAttack1, 2 = lightAttack2
    private float lastHitTime = 0f;
    private float comboTimeout = 0.5f;
    private bool attackOnCooldown = false;

    private void Awake() {
        lightAttack = GetComponent<PlayerLightAttack>();
        lightAttack1 = GetComponent<PlayerLightAttack1>();
        lightAttack2 = GetComponent<PlayerLightAttack2>();
        heavyAttack = GetComponent<PlayerHeavyAttack>();
        parry = GetComponent<PlayerParry>();
        healthPack = GetComponent<PlayerPotion>();
        damagePotion = GetComponent<PlayerDamagePotion>();
        speedPotion = GetComponent<PlayerSpeedPotion>();

        playerActions = new List<PlayerAction> { lightAttack, lightAttack1, lightAttack2, heavyAttack, parry, healthPack, damagePotion, speedPotion};

        playerMovement = GetComponent<PlayerMovement>();
    }

    private void Start() {
        GameState gameState = GameObject.FindGameObjectWithTag("GameState").GetComponent<GameState>();
        if (gameState == null) { Debug.LogError("No game state found."); return; }

        playerState = gameState.GetPlayerState();
    }


    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.W) && CanPerformAction())
        {
            speedPotion.StartAction();
        }

        if (Input.GetKeyDown(KeyCode.R) && CanPerformAction())
        {
            damagePotion.StartAction();
        }

        if (Input.GetKeyDown(KeyCode.Q) && CanPerformAction())
        {
            healthPack.StartAction();
        }

        // Handles switching animations between attacks 
        if (Input.GetKeyDown(KeyCode.D) && CanPerformAction() && !attackOnCooldown)
        {
                // Reset combo if timeout occurs
                if (Time.time - lastHitTime > comboTimeout)
                {
                    comboStage = 0;
                }

                // Execute attack based on current combo stage
                switch (comboStage)
                {
                    case 0:
                        lightAttack.StartAction();
                        break;
                    case 1:
                        lightAttack1.StartAction();
                        break;
                    case 2:
                        lightAttack2.StartAction();
                        StartCoroutine(NextAttackDelay());
                        break;
                }

                // Schedule combo reset
                CancelInvoke("ResetCombo");
                Invoke("ResetCombo", comboTimeout);
        }

        if (Input.GetKeyDown(KeyCode.S) && CanPerformAction())
        {
            heavyAttack.StartAction();
        }

        if (Input.GetKeyDown(KeyCode.A) && CanPerformAction())
        {
            parry.StartAction();
        }

        if (Input.GetKeyUp(KeyCode.A))
        {
            parry.InterruptAction();
        }


    }

    void ResetCombo()
    {
        comboStage = 0;
    }

    /// <summary>
    /// Waits for a few moments after the third attack
    /// </summary>
    /// <returns></returns>
    private IEnumerator NextAttackDelay()
    {
        attackOnCooldown = true;
        yield return new WaitForSeconds(0.8f);
        attackOnCooldown = false;
    }

    // is called by lightAttack trigger 
    public void LightAttackHit()
    {
        // Progress combo stage only if within limits
        if (comboStage < 2)
        {
            comboStage++;
            lastHitTime = Time.time;
        }

    }

    /// <summary>
    /// Handles recieving damage from enemies, parry and eventual death
    /// </summary>
    /// <returns></returns>
    public override DamageResponseData RecieveDamage(DamageData damageData) {

        bool parrySuccessful = parry.TryParryDamage(damageData);

        if (!parrySuccessful) {
            InterruptActions();
            avatarAnimator.SetTrigger("damage");
            playerState.ChangeLives(-damageData.damage);
            if (playerState.GetLives() <= 0) {
                // Death
                GameObject.FindAnyObjectByType<UIController>().Death();
            }
        }

        return new DamageResponseData(isSolid, parrySuccessful);
    }

    public void Celebrate()
    {
        EquipItem(null);
        avatarAnimator.SetTrigger("celebrate");
    }

    public void HealToFull()
    {
        playerState.ChangeLives(playerState.GetMaxLives());
    }

    public void EquipItem(GameObject item) {

        // Destroy any held objects (should always be one or none)
        if (itemSlot.transform.childCount > 0) {
            foreach (Transform child in itemSlot.transform) {
                Destroy(child.gameObject);
            }
        }

        if (item != null) {
            // Can't instantiate item as a child of itemSlot, because of scaling issues
            GameObject newItem = Instantiate(item);
            newItem.transform.parent = itemSlot.transform;

            // Set items position and rotation the same as the itemSlot (so make local values zero)
            newItem.transform.localPosition = Vector3.zero;
            newItem.transform.localRotation = Quaternion.identity;

            // if player is facing left, then the item needs to be flipped
            if (!playerMovement.IsFacingRight()) {
                Vector3 newScale = newItem.transform.localScale;
                newScale.x = -newScale.x;
                newItem.transform.localScale = newScale;
            }
        }
    }

    /// <summary>
    ///     Disable control for player (no movement and no actions).
    /// </summary>
    public void SetControlBlocked(bool blocked) {
        controlBlocked = blocked;
        SetMovementEnabled(!blocked);
    }

    /// <summary>
    ///     Enables/disables player movement.
    /// </summary>
    public void SetMovementEnabled(bool enabled) {
        playerMovement.movementEnabled = enabled;
    }

    public void SetConfinedArea(Rectangle rect) {
        playerMovement.SetConfinedArea(rect);
    }

    public void UnsetConfinedArea() {
        playerMovement.UnsetConfinedArea();
    }

    public Animator GetAvatarAnimator() { return avatarAnimator; }

    public bool GetPlayerMovement() { return playerMovement.movementEnabled; }

    public PlayerAnimatorOutputManager GetPlayerAnimatorOutputManager() { return animatorOutputManager; }

    public AudioManager GetAudioManager() { return audioManager; }

    public PlayerState GetPlayerState() { return playerState; }

    /// <summary>
    ///     Stops all actions.
    /// </summary>
    private void InterruptActions() {
        foreach (var action in playerActions) {
            action.InterruptAction();
        }
    }

    /// <summary>
    ///     Checks wether player can perform an action
    /// </summary>
    /// <returns>True if true, false if false</returns>
    private bool CanPerformAction() {
        if (controlBlocked) { 
            return false;
        }
        foreach (var action in playerActions) {
            if (action.IsActive) return false;
        }
        return true;
    }

}
