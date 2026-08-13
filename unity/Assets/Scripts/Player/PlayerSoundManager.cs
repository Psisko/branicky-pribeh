using System.Collections;
using UnityEngine;

public class PlayerSoundManager : MonoBehaviour {

    [SerializeField] private AudioClip[] steps;
    [SerializeField] private AudioSource stepPlayer;   
    [SerializeField] private AudioSource lightAttackPlayer;
    [SerializeField] private AudioSource heavyAttackPlayer;  
    [SerializeField] private AudioSource damagePlayer;

    public void PlaySound(string name) {
        switch (name) {
            case "step":
                stepPlayer.clip = steps[Random.Range(0, steps.Length)];
                stepPlayer.Play();
                break;
            case "lightAttack":
                lightAttackPlayer.Play();
                break;
            case "heavyAttack":
                heavyAttackPlayer.Play();
                break;
            case "damage":
                damagePlayer.Play();
                break;
            default:
                Debug.LogWarning($"PlaySound: {name} is invalid sound name", gameObject);
                break;
        }
    }
}
