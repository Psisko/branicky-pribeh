using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MissionExit : MonoBehaviour
{
    [SerializeField] private Trigger trigger;
    [SerializeField] private string missionName;
    [SerializeField] private StatsState statsState;
    [SerializeField] private string branikSceneName = "0-branik";


    private void Start() {
        trigger.activationEvent.AddListener(ChangeScene);
    }


    public void ChangeScene() {

        StartCoroutine(ShowStats());
    }

    /// <summary>
    /// Show ending level stats
    /// </summary>
    /// <returns></returns>
    public IEnumerator ShowStats()
    {
        GameObject gameStateGO = GameObject.FindGameObjectWithTag("GameState");
        GameState gameState = gameStateGO.GetComponent<GameState>();

        MissionsState ms = gameState.GetMissionsState();
        ms.MarkCompleted(missionName);

        statsState.ShowStatsScreen(missionName);

        Time.timeScale = 0;

        yield return new WaitForSecondsRealtime(5f);

        Time.timeScale = 1;

        SceneManager.LoadScene(branikSceneName);
    }
}
