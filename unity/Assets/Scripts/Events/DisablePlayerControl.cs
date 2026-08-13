using System.Collections;
using UnityEngine;


[RequireComponent(typeof(EventController))]
public class DisablePlayerControl : MonoBehaviour {

    PlayerController player;
    private EventController controller;

    private void Start() {
        controller = GetComponent<EventController>();
        controller.eventStart.AddListener(DisableMovement);
        controller.eventEnd.AddListener(EnableMovement);
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    private void DisableMovement() {
        player.SetControlBlocked(true);
    }

    private void EnableMovement() {
        player.SetControlBlocked(false);
    }
}
