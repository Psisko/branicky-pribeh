using System.Collections;
using UnityEngine;


[RequireComponent(typeof(EventController))]
public class DelayEvent : MonoBehaviour {

    [SerializeField] private float delay = 0.1f;
    [SerializeField] private bool blockPlayerMovement;
    [SerializeField] private bool celebrate;
    PlayerController playerController;

    private EventController controller;

    private void Start() {
        controller = GetComponent<EventController>();
        controller.eventStart.AddListener(EndEvent);
        GameObject playerGO = GameObject.FindGameObjectWithTag("Player");
        if (playerGO == null) Debug.LogError("Player not found", gameObject);
        playerController = playerGO.GetComponent<PlayerController>();
    }


    private void EndEvent() {
        StartCoroutine(EndEventCoroutine());
    }

    /// <summary>
    /// Handles delays between evennts and celebration after boss fights
    /// </summary>
    /// <returns></returns>
    private IEnumerator EndEventCoroutine() {
        yield return new WaitForSeconds(1);
        if(celebrate)
            playerController.Celebrate();
        if (blockPlayerMovement)
            playerController.SetControlBlocked(true);
        yield return new WaitForSeconds(delay);
        if (blockPlayerMovement)
            playerController.SetControlBlocked(false);
        controller.EndEvent();

    }
}
