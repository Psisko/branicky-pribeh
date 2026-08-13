using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TP : MonoBehaviour
{
    [SerializeField] private Trigger TPTrigger;
    [SerializeField] private GameObject newPosition;

    private Camera mainCamera;
    private Transform player;


    // Start is called before the first frame update
    private void Start()
    {
        GameObject gameStateGO = GameObject.FindGameObjectWithTag("GameState");
        GameState gameState = gameStateGO.GetComponent<GameState>();

        GameObject playerGO = GameObject.FindGameObjectWithTag("Player");
        if (playerGO == null) Debug.LogError("Player not found", gameObject);
        player = playerGO.transform;

        if (gameState == null)
        {
            Debug.LogError("No game state found.");
            return;
        }

        // Initialize mainCamera
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        // TPTrigger.SetIsEnabled(false);
        TPTrigger.activationEvent.AddListener(TPfunction);
    }

    // Update is called once per frame
    private void TPfunction()
    {
        player.transform.position = newPosition.transform.position;
        mainCamera.transform.position = player.transform.position;
    }
}
