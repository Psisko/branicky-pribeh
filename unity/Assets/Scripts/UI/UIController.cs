using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Playables;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [SerializeField] private Image livesSlider;
    [SerializeField] private DialogueUI dialogueUI;
    [SerializeField] private TextMeshProUGUI moneyText;
    [SerializeField] private GameObject healthCans;
    [SerializeField] private GameObject damageCans;
    [SerializeField] private GameObject speedCans;
    [SerializeField] private GameObject deathScreen;

    private MixerController mixerController;

    private float UIhealth, UImaxHealth;
    private float lerpSpeed;

    private void Start() {
        SetupPlayerDataUI();
    }

    private void Update()
    {
        lerpSpeed = 5f * Time.deltaTime;

        livesSlider.fillAmount = Mathf.Lerp(livesSlider.fillAmount, UIhealth / UImaxHealth, lerpSpeed);
    }

    private void UpdateLivesUI(int health, int maxHealth) {
        UIhealth = (float)health;
        UImaxHealth = (float)maxHealth;
    }

    private void UpdateMoneyUI(int money) {
        moneyText.text = $"{money}";
    }

    private void UpdateHealthpacksUI(int healthpacks) {

        for (int i = 0; i < 5; i++)
        {
            // getting i-child
            Transform child = healthCans.transform.GetChild(i);
            Image img = child.GetComponent<Image>();

            if (img != null)
            {
                // showing only if i < healthpacks, otherwise hide
                img.enabled = (i < healthpacks);
            }
        }
    }

    private void UpdatedamagePotionUI(int damagePotions)
    {
        for (int i = 0; i < 3; i++)
        {
            // getting i-child
            Transform child = damageCans.transform.GetChild(i);
            Image img = child.GetComponent<Image>();

            if (img != null)
            {
                // showing only if i < healthpacks, otherwise hide
                img.enabled = (i < damagePotions);
            }
        }
    }

    private void UpdatespeedPotionUI(int speedPotions)
    {
        for (int i = 0; i < 3; i++)
        {
            // getting i-child
            Transform child = speedCans.transform.GetChild(i);
            Image img = child.GetComponent<Image>();

            if (img != null)
            {
                // showing only if i < healthpacks, otherwise hide
                img.enabled = (i < speedPotions);
            }
        }
    }

    private void UpdatespeedPotiontimerUI(int speedPotions)
    {
        StartCoroutine(SpeedCanSlider(speedPotions));
    }

    IEnumerator SpeedCanSlider(int numberOfPotions)
    {
        // getting last child
        Transform child = speedCans.transform.GetChild(numberOfPotions);
        Image img = child.GetComponent<Image>();

        img.enabled = true;

        if (img != null)
        {
            
            float duration = 15.0f; // animation is 15 seconds long
            float elapsed = 0.0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                img.fillAmount = 1 - (elapsed / duration);
                yield return null; // Update every frame
            }

            img.fillAmount = 0;
            img.enabled = false;
        }
    }

    private void UpdatedamagePotiontimerUI(int damagePotions)
    {
        StartCoroutine(DamageCanSlider(damagePotions));
    }

    IEnumerator DamageCanSlider(int numberOfPotions)
    {
        // getting last child
        Transform child = damageCans.transform.GetChild(numberOfPotions);
        Image img = child.GetComponent<Image>();

        img.enabled = true;

        if (img != null)
        {

            float duration = 30.0f; // animation is 30 seconds long
            float elapsed = 0.0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                img.fillAmount = 1 - (elapsed / duration);
                yield return null; // Update every frame
            }

            img.fillAmount = 0;
            img.enabled = false;
        }
    }

    public void OpenPauseMenu()
    {
        PauseMenuUI pauseMenuUI = GetComponentInChildren<PauseMenuUI>();
        pauseMenuUI.OpenPauseScreen();
    }

    /// <summary>
    /// Shows deathscreen
    /// </summary>
    public void Death() {
        deathScreen.SetActive(true);
        PauseMenuUI pauseMenuUI = GetComponentInChildren<PauseMenuUI>();
        pauseMenuUI.isDeathScreenActive = true;
        Time.timeScale = 0;
        mixerController.TurnDownAllSounds();
    }

    /// <summary>
    /// Initial setup
    /// </summary>
    private void SetupPlayerDataUI() {
        GameObject gameStateGO = GameObject.FindGameObjectWithTag("GameState");
        if (gameStateGO == null) { Debug.LogError("No game state object found.", gameObject); return; }
        GameState gameState = gameStateGO.GetComponent<GameState>();
        if (gameState == null) { Debug.LogError("No game state script found.", gameObject); return; }
        PlayerState playerState = gameState.GetComponent<GameState>().GetPlayerState();
        if (gameState == null) { Debug.LogError("No player state found.", gameObject); return;  }

        GameObject mixerControllerGO = GameObject.FindGameObjectWithTag("Audio");
        if (mixerControllerGO == null) { Debug.LogError("No mixerControllerGO state found.", gameObject); return; }
        mixerController = mixerControllerGO.GetComponent<MixerController>();
        if (mixerController == null) { Debug.LogError("No mixerController state found.", gameObject); return; }

        livesSlider.fillAmount = (float)playerState.GetLives() / (float)playerState.GetMaxLives();

        playerState.livesChangeEvent.AddListener(UpdateLivesUI);
        playerState.moneyChangeEvent.AddListener(UpdateMoneyUI);
        playerState.healthpacksChangeEvent.AddListener(UpdateHealthpacksUI);
        playerState.damagePotionsChangeEvent.AddListener(UpdatedamagePotionUI);
        playerState.speedPotionsChangeEvent.AddListener(UpdatespeedPotionUI);
        playerState.speedPotionstimerChangeEvent.AddListener(UpdatespeedPotiontimerUI);
        playerState.damagePotionstimerChangeEvent.AddListener(UpdatedamagePotiontimerUI);

        UpdateLivesUI(playerState.GetLives(), playerState.GetMaxLives());
        UpdateMoneyUI(playerState.GetMoney());
        UpdateHealthpacksUI(playerState.GetHealthpacks());
        UpdatedamagePotionUI(playerState.GetDamagePotions());
        UpdatespeedPotionUI(playerState.GetSpeedPotions());
    }
}
