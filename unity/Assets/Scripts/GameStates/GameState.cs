using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Stores all persistent data.
/// </summary>
[Serializable]
public class GameState : MonoBehaviour {
    private PlayerState playerState = new();
    private BranikState branikState = new();
    [SerializeField] private MissionsState missionsState;

    // instance, so it wont destroy on load
    public static GameState instance { get; private set; }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public PlayerState GetPlayerState() { return playerState; }

    public BranikState GetBranikState() { return branikState; }

    public MissionsState GetMissionsState() {  return missionsState; }


    public void SetPlayerStateLevelStartingParameters()
    {
        playerState.SetLevelStartingParameters(); 
    }


    [ContextMenu("Completed missions")]
    private void ContextMenuCompletedMissions() {
        Debug.Log(missionsState.MissionsToString());
    }
}
