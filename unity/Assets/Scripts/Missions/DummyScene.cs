using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DummyScene : MonoBehaviour
{
    [SerializeField] private string missionName;
    [SerializeField] private int reward;
    [SerializeField] private string branikSceneName;

    void Start() {
        Debug.Log($"Dummy mission {SceneManager.GetActiveScene().name} played!");
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.E)) {
            GameObject gameStateGO = GameObject.FindGameObjectWithTag("GameState");
            GameState gameState = gameStateGO.GetComponent<GameState>();
            if (gameState == null) {
                Debug.LogError("No game state found.");
                return;
            }

            gameState.GetPlayerState().ChangeMoney(reward);
            MissionsState ms = gameState.GetMissionsState();
            ms.MarkCompleted(missionName);
            SceneManager.LoadScene(branikSceneName);
        }
    }

}
