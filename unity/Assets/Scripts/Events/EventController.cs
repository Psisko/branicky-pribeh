using UnityEngine;
using UnityEngine.Events;

public class EventController : MonoBehaviour {
    public UnityEvent eventStart;
    public UnityEvent eventEnd;

    [SerializeField] private EventController nextEvent;
    [SerializeField, Tooltip("Optional trigger that starts the event. You can also start the event " +
        "by calling StartEvent method.")] 
        private Trigger trigger;
    
    private bool isHappening = false;
    private bool isDisabled = false;

    private void Start() {
        if (trigger != null) {
            trigger.activationEvent.AddListener(StartEvent);
        }    
    }

    public void StartEvent() {
        if (isDisabled) {
            return;
        }

        if (isHappening) {
            Debug.LogError("Can't start an event that already started!", gameObject);
            return;
        }

        if (trigger != null) {
            trigger.SetIsEnabled(false);
        }
        eventStart.Invoke();
        isHappening = true;
    }

    public void EndEvent() {
        if (!isHappening) {
            Debug.LogError("Can't end event that is not happening!", gameObject);
            return;
        }

        if (isDisabled) {
            Debug.Log("Can't end event that is disabled! This shouldn't happen.", gameObject);
            return;
        }

        if (trigger != null) {
            trigger.SetIsEnabled(true);
        }
        eventEnd.Invoke();
        isHappening = false;
        if (nextEvent != null) {
            nextEvent.StartEvent();
        }
    }

    public void DisableEvent() {
        if (isHappening) {
            Debug.LogError("Can't disable an event taht is currently happening!", gameObject);
            return;
        }

        isDisabled = true;
        if (trigger != null) {
            trigger.SetIsEnabled(false);
        }
    }

    public bool IsHappening() { return isHappening; }
}