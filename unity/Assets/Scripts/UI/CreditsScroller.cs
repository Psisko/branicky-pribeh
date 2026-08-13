using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditsScroller : MonoBehaviour
{
    [SerializeField] private float creditsHeight = 10;
    [SerializeField] private string mainMenuSceneName;
    [SerializeField] private AudioClip clip;

    private AudioSource audioSource;
    private float audioLength;
    private GameObject SettingsMenu;

    private Vector2 startPos;
    private Vector2 endPos;

    private bool fastForward = false;
    private float fastForwardChangeTime = -100;


    private void Awake()
    {
        GameObject SettingsMenu = GameObject.FindGameObjectWithTag("SettingsMenu");
        SettingsMenu.SetActive(false);
    }

    private void Start() {

        GameObject audioprefab = GameObject.FindGameObjectWithTag("Audio");
        MixerController controller = audioprefab.GetComponent<MixerController>();
        audioSource = controller.BGM;
        audioSource.clip = clip;
        audioSource.Play();
        audioSource.loop = false;
        audioLength = audioSource.clip.length;

        startPos = transform.position;
        endPos = transform.position;
        endPos.y += creditsHeight;
    }

    private void Update() {

        if (Input.GetKeyDown(KeyCode.E)) {
            fastForward = true;
            fastForwardChangeTime = Time.time;
        }
        if (Input.GetKeyUp(KeyCode.E)) {
            fastForward = false;
            fastForwardChangeTime = Time.time;
        }

        if (fastForward) {
            float pitch = Mathf.Clamp(Time.time - fastForwardChangeTime, 0f, 1f) * 2 + 1;
            audioSource.pitch = pitch;
        } else {
            float pitch = (1 - Mathf.Clamp(Time.time - fastForwardChangeTime, 0f, 1f)) * 2 + 1;
            audioSource.pitch = pitch;
        }

        float percent = audioSource.time / audioLength;
        transform.position = Vector2.Lerp(startPos, endPos, percent);

        if (!audioSource.isPlaying) {
            SceneManager.LoadScene(mainMenuSceneName);
        }
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, new Vector2(transform.position.x, transform.position.y - creditsHeight));
    }
}
