using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;

public class PauseMenuUI : MonoBehaviour
{

    public GameObject PauseScreen;
    private GameObject audioScreen;
    private GameObject graphicsScreen;
    private GameObject keybindsScreen;
    private PlayerController playerController;
    private bool isPaused = false;
    public bool isDeathScreenActive;
    private bool isPlayerAbleToMove;

    private void Start()
    {

        GameObject playerGO = GameObject.FindGameObjectWithTag("Player");
        if (playerGO == null) Debug.LogError("Player not found", gameObject);
        playerController = playerGO.GetComponent<PlayerController>();

        GameObject SettingsMenu = GameObject.FindGameObjectWithTag("SettingsMenu");
        if (SettingsMenu == null) Debug.LogError("SettingsMenu not found", gameObject);
        SettingsMenuUI settingsMenuUI = SettingsMenu.GetComponent<SettingsMenuUI>();
        audioScreen = settingsMenuUI.AudioScreen;
        graphicsScreen = settingsMenuUI.GraphicsScreen;
        keybindsScreen = settingsMenuUI.KeybindsScreen;
        isDeathScreenActive = false;

    }
    /// <summary>
    /// Handles showing pause screen
    /// </summary>
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && isPaused == true && isDeathScreenActive == false)
            ClosePauseScreen();
        else if (Input.GetKeyDown(KeyCode.Escape) && isPaused == false && isDeathScreenActive == false)
            OpenPauseScreen();
    }

    public void OpenPauseScreen()
    {
        isPlayerAbleToMove = playerController.GetPlayerMovement();
        isPaused = true;
        if(isPlayerAbleToMove)
            playerController.SetMovementEnabled(false);
        PauseScreen.SetActive(true);
        // stops time in game
        Time.timeScale = 0;
    }

    public void ClosePauseScreen()
    {
        isPaused = false;
        playerController.SetMovementEnabled(isPlayerAbleToMove);
        PauseScreen.SetActive(false);
        Time.timeScale = 1;
    }

    // called on click in unity
    public void OpenAudioMenuUI()
    {
        PauseScreen.SetActive(false);
        audioScreen.SetActive(true);
    }

    public void CloseAudioMenuUI()
    {
        if(isPaused)
            PauseScreen.SetActive(true);
        if(audioScreen.activeSelf == true)
            audioScreen.SetActive(false);
        if(graphicsScreen.activeSelf == true)
            graphicsScreen.SetActive(false);
        if (keybindsScreen.activeSelf == true)
            keybindsScreen.SetActive(false);
    }

    // called by a button in unity
    public void OpenGraphicsMenuUI()
    {
        PauseScreen.SetActive(false);
        graphicsScreen.SetActive(true);
    }

    public void OpenKeybindsMenuUI()
    {
        PauseScreen.SetActive(false);
        keybindsScreen.SetActive(true);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

}
