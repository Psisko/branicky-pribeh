using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BranikExitController : MonoBehaviour
{
    [SerializeField] private Trigger exitTrigger;

    private string nextSceneName;
    private MissionsState missionState;

    private void Start() {
        GameObject gameStateGO = GameObject.FindGameObjectWithTag("GameState");
        GameState gameState = gameStateGO.GetComponent<GameState>();
        if (gameState == null) {
            Debug.LogError("No game state found.");
            return;
        }

        missionState = gameState.GetMissionsState();

        exitTrigger.SetIsEnabled(false);
        exitTrigger.activationEvent.AddListener(LeaveBranik);
    }

    public void SetupExit(string missionName) {
        nextSceneName = missionState.GetMissionSceneName(missionName);
        exitTrigger.SetIsEnabled(true);
    }

    private void LeaveBranik() {
        SceneManager.LoadScene(nextSceneName);
    }
}
