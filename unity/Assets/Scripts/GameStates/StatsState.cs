using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StatsState : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI headingText;
    [SerializeField] private TextMeshProUGUI moneyText;
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private TextMeshProUGUI enemiesText;
    public GameObject statsScreen;
    public Animator animator;

    public void ShowStatsScreen(string missionName)
    {

        CalculateLevelStats();

        if(missionName == "unetice")
            headingText.text = "Unìtický pivovar dokonèen!";

        if (missionName == "starobrno")
            headingText.text = "Pivovar Starobrno dokonèen!";

        if (missionName == "plzen")
            headingText.text = "Plzeòský pivovar dokonèen!";

        statsScreen.SetActive(true);
        animator.updateMode = AnimatorUpdateMode.UnscaledTime;
        animator.SetTrigger("darken");


    }

    /// <summary>
    /// Calculates ending level stats
    /// </summary>
    public void CalculateLevelStats()
    {
        GameObject gameStateGO = GameObject.FindGameObjectWithTag("GameState");
        if (gameStateGO == null) { Debug.LogError("No game state object found.", gameObject); return; }
        GameState gameState = gameStateGO.GetComponent<GameState>();
        if (gameState == null) { Debug.LogError("No game state script found.", gameObject); return; }
        PlayerState playerState = gameState.GetComponent<GameState>().GetPlayerState();
        if (gameState == null) { Debug.LogError("No player state found.", gameObject); return; }


        moneyText.text = $"{playerState.GetFinalMoney()}";
        timeText.text = $"{playerState.GetFinalTime().ToString("F2")}";
        enemiesText.text = $"{playerState.GetFinalEnemies()}";

    }

}
