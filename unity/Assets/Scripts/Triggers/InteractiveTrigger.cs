using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractiveTrigger : Trigger {

    [SerializeField] private Transform UIPopup;

    private Transform player;
    private bool isPlayerInside = false;

    protected override void Start()
    {
        base.Start();
        GameObject playerGO = GameObject.FindGameObjectWithTag("Player");
        if (playerGO == null) Debug.LogError("Player not found", gameObject);
        player = playerGO.transform;

        if (UIPopup == null) Debug.LogError("No UI popup", gameObject);
        UIPopup.gameObject.SetActive(false);

    }

    private void Update() {
        if (isPlayerInside && Input.GetKeyDown(KeyCode.E)) {
            Activate();
        }
    }

    public override void SetIsEnabled(bool enabled) {
        if (!enabled) UIPopup.gameObject.SetActive(false);
        if (enabled && isPlayerInside) UIPopup.gameObject.SetActive(true);

        base.SetIsEnabled(enabled);
    }
    /// <summary>
    /// Handles activating if the player moves into the trigger
    /// </summary>
    private void OnTriggerEnter2D(Collider2D collision) {


        if (collision.transform == player) {
            isPlayerInside = true;
            if (isEnabled) UIPopup.gameObject.SetActive(true);
        }

    }

    /// <summary>
    /// Handles deactivating if the player moves out of the trigger
    /// </summary>
    private void OnTriggerExit2D(Collider2D collision) {

        if (collision.transform == player) {
            isPlayerInside = false;
            if (isEnabled) UIPopup.gameObject.SetActive(false);
        }

    }







    [ContextMenu("Set Enabled")]
    private void EditorSetEnabled() {
        SetIsEnabled(true);
    }
    [ContextMenu("Set Disabled")]
    private void EditorSetDisabled() {
        SetIsEnabled(false);
    }
}
