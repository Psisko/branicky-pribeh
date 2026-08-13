using System;
using UnityEngine;

public class LoadingParametersEvent : MonoBehaviour {

    private EventController eventController;

    private void Awake()
    {
        eventController = GetComponent<EventController>();
    }

    private void Start()
    {
        eventController.eventStart.AddListener(LoadParameters);
    }
    /// <summary>
    /// Loading parameters on the start of a level for EndingStats
    /// </summary>
    public void LoadParameters()
	{
        GameObject gameStateGO = GameObject.FindGameObjectWithTag("GameState");
        if (gameStateGO == null) { Debug.LogError("No game state object found.", gameObject); return; }
        GameState gameState = gameStateGO.GetComponent<GameState>();
        if (gameState == null) { Debug.LogError("No game state script found.", gameObject); return; }

        gameState.SetPlayerStateLevelStartingParameters();

    }

}
