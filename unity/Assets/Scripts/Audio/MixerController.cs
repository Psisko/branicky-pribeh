using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;
using UnityEngine.Playables;

public class MixerController : MonoBehaviour {

    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] public AudioSource BGM;

    // instance, so it wont destroy on load
    public static MixerController instance { get; private set; }

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
        GameObject SettingsMenu = GameObject.FindGameObjectWithTag("SettingsMenu");
        if (SettingsMenu == null) Debug.LogError("SettingsMenu not found", gameObject);
        SettingsMenuUI audiomenuUI = SettingsMenu.GetComponent<SettingsMenuUI>();

        audiomenuUI.musicChangeEvent.AddListener(SetMusicVolume);
        audiomenuUI.effectsChangeEvent.AddListener(SetEffectsVolume);

    }

    public void TurnDownAllSounds()
    {
        audioMixer.SetFloat("masterParam", -80f);
        BGM.Stop();
    }

    public void TurnUpAllSounds()
    {
        audioMixer.SetFloat("masterParam", 0);
        BGM.Play();
    }

    public void SetMusicVolume(float volume)
    {   
        audioMixer.SetFloat("musicParam", Mathf.Log10(volume) * 20);
    }

    public void SetEffectsVolume(float volume)
    {
        audioMixer.SetFloat("effectsParam", Mathf.Log10(volume) * 20);
    }

}
