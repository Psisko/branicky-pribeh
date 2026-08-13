using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class Trigger : MonoBehaviour
{
    public UnityEvent activationEvent;
    [Tooltip("If true, trigger destroys itself after first use.")]
    public bool onlyOnce = true;
    [Tooltip("If false, trigger cannot be activated"), SerializeField]
    protected bool isEnabled = true;

    private AudioSource BGMaudioSource;

    private Coroutine enableDelayedCoroutine;

    protected virtual void Start()
    {
        GameObject audioprefab = GameObject.FindGameObjectWithTag("Audio");
        MixerController controller = audioprefab.GetComponent<MixerController>();
        BGMaudioSource = controller.BGM;

    }

    public virtual void SetIsEnabled(bool enabled) {

        if (enableDelayedCoroutine != null) {
            StopCoroutine(enableDelayedCoroutine); 
        }

        if (enabled) {
            enableDelayedCoroutine = StartCoroutine(EnableDelayed());
        } else {
            isEnabled = false;
        }
    }

    public void SetBGMClip(AudioClip clip)
    {
        BGMaudioSource.clip = clip;
        BGMaudioSource.Play();
    }
    public void SetBGMVolume(float volume)
    {
        BGMaudioSource.volume = volume;
    }

    /// <summary>
    ///     Activates the trigger.
    /// </summary>
    protected void Activate() {

        if (!isEnabled) return;

        activationEvent.Invoke();


        if (onlyOnce) Destroy(gameObject);

    }


    private IEnumerator EnableDelayed() {
        yield return new WaitForSeconds(0.1f);
        isEnabled = true;
    }

}
