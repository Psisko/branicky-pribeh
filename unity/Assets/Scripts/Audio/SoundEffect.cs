using System;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public class SoundEffect {
    public string name;
    [Tooltip("Array of sound variants."), SerializeField]
    private AudioClip[] clips;
    [Range(0f, 1f)]
    public float volume = 1;

    public AudioClip GetClip() {
        return clips[Random.Range(0, clips.Length)];
    }
}
