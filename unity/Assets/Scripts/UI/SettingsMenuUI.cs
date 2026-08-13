using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SettingsMenuUI : MonoBehaviour
{

    public UnityEvent<float> musicChangeEvent = new();
    public UnityEvent<float> effectsChangeEvent = new();

    private PauseMenuUI pauseMenuUI;
    public GameObject AudioScreen;
    public GameObject GraphicsScreen;
    public GameObject KeybindsScreen;

    [SerializeField] private Slider MusicSlider;
    [SerializeField] private Slider EffectsSlider;

    // instance, so it wont destroy on load
    public static SettingsMenuUI instance { get; private set; }

    Resolution[] resolutions;
    public TMPro.TMP_Dropdown resolutionsDropdown;
    public TMPro.TMP_Dropdown qualitiesDropdown;

    int currentResolutionIndex = 0;
    int currentQualityIndex = 4;
    bool currentFullScreen = true;

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

    private void Start()
    {

        GameObject HUD = GameObject.FindGameObjectWithTag("HUD");
        if (HUD == null) Debug.LogError("HUD not found", gameObject);
        pauseMenuUI = HUD.GetComponentInChildren<PauseMenuUI>();

        resolutions = Screen.resolutions;

        resolutionsDropdown.ClearOptions();
        // makes strings from resolutions
        List<string> options = new List<string>();

        for(int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);

            if (resolutions[i].width == Screen.width &&
                resolutions[i].height == Screen.height)
            {
                currentResolutionIndex = i;
            }
        }
        // sets qualities
        qualitiesDropdown.value = currentQualityIndex;
        qualitiesDropdown.RefreshShownValue();
        // sets resolutions
        resolutionsDropdown.AddOptions(options);
        resolutionsDropdown.value = currentResolutionIndex;
        resolutionsDropdown.RefreshShownValue();
        // sets volumes
        SetMusicVolume();
        SetSoundEffectsVolume();
    }
    
    void Update()
    {
        // finds pauseMenuUI for the first time
        if(pauseMenuUI == null)
        {
            GameObject HUD = GameObject.FindGameObjectWithTag("HUD");
            if (HUD == null) Debug.LogError("HUD not found", gameObject);
            pauseMenuUI = HUD.GetComponentInChildren<PauseMenuUI>();

            QualitySettings.SetQualityLevel(currentQualityIndex);
            Screen.SetResolution(resolutions[currentResolutionIndex].width, resolutions[currentResolutionIndex].height, Screen.fullScreen);
            Screen.fullScreen = currentFullScreen;
        }
        if (Input.GetKeyDown(KeyCode.Escape) && pauseMenuUI.PauseScreen.activeSelf == false)
        {
            CloseMenuUI();
        }
    }
    // set quality in settings
    public void SetQuality(int qualityIndex)
    {
        currentQualityIndex = qualityIndex;
        
    }
    // set fullscreen in settings
    public void SetFullscreen(bool isFullscreen)
    {
        currentFullScreen = isFullscreen;
    }

    public void SetResolution(int resolutionIndex)
    {
        currentResolutionIndex = resolutionIndex;
    }

    public void CloseMenuUI()
    {
        pauseMenuUI.CloseAudioMenuUI();
    }

    public void SetMusicVolume()
    {
        musicChangeEvent.Invoke(MusicSlider.value);
    }

    public void SetSoundEffectsVolume()
    {
        effectsChangeEvent.Invoke(EffectsSlider.value);
    }

}
