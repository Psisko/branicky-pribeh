using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour {
    [SerializeField] List<SoundEffect> soundEffects;


    private AudioSource audioSource;

    private void Awake() {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null ) { Debug.LogError("No AoudioSource component found!", gameObject); }
    }



    public void PlaySoundEffect(string name) {
        SoundEffect effect = soundEffects.Find(x => x.name == name);
        if (effect == null) {
            Debug.LogError("Sound effect with name " + name + " not found!", gameObject); 
            return;
        }

        audioSource.PlayOneShot(effect.GetClip(), effect.volume);
    }
}
