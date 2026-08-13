using System.Collections;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class DeathController : MonoBehaviour {

    private int moneyOnSceneLoad;
    private int livesOnSceneLoad;
    private int healthPotionsOnSceneLoad;
    private int damagePotionsOnSceneLoad;
    private int speedPotionsOnSceneLoad;

    private MixerController mixerController;
    PlayerState ps;

    void Start() {
        GameState gameState = GameObject.FindGameObjectWithTag("GameState").GetComponent<GameState>();
        if (gameState == null) { Debug.LogError("No game state found."); return; }
        ps = gameState.GetPlayerState();
        (this.livesOnSceneLoad, this.moneyOnSceneLoad, 
            this.healthPotionsOnSceneLoad, this.damagePotionsOnSceneLoad, this.speedPotionsOnSceneLoad) = ps.GetPlayerResources();

        GameObject mixerControllerGO = GameObject.FindGameObjectWithTag("Audio");
        if (mixerControllerGO == null) { Debug.LogError("No mixerControllerGO state found.", gameObject); return; }
        mixerController = mixerControllerGO.GetComponent<MixerController>();
        if (mixerController == null) { Debug.LogError("No mixerController state found.", gameObject); return; }
    }

    /// <summary>
    /// Is called by death screen button
    /// </summary>
    public void PlayerDeath() {
        ps.LoadPlayerResources(livesOnSceneLoad, moneyOnSceneLoad, healthPotionsOnSceneLoad, damagePotionsOnSceneLoad, speedPotionsOnSceneLoad);
        Time.timeScale = 1;
        mixerController.TurnUpAllSounds();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

}
