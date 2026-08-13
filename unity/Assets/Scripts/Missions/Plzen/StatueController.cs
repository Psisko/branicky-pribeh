using System.Collections;
using UnityEngine;
using UnityEngine.Events;


/// <summary>
///     Controls the statues in statue puzzle. Requires EventController and ends the event.
/// </summary>
[RequireComponent(typeof(EventController))]
[Tooltip("Controls statues")]
public class StatueController : MonoBehaviour {

    [SerializeField] private DialogueSO dialogue;
    [SerializeField] private GameObject itemSlot;
    [SerializeField] private GameObject uneticeBottle;
    [SerializeField] private GameObject staropramenBottle;
    [SerializeField] private GameObject starobrnoBottle;
    [SerializeField] private GameObject plzenBottle;
    [SerializeField, Tooltip("name of correct beer (unetice, starobrno, staropramen, plzen)")] 
        private string correctChoice;

    [HideInInspector] public UnityEvent changeEvent;

    private string lastChoice = "";

    private DialogueUI dialogueUI;
    private EventController eventController;

    private void Awake() {
        eventController = GetComponent<EventController>();
    }

    private void Start() {
        dialogueUI = GameObject.FindAnyObjectByType<DialogueUI>();

        eventController.eventStart.AddListener(PlayDialogue);
    }

    public void DisableInteraction() {
        eventController.DisableEvent();
    }

    public bool IsChoiceCorrect() {
        return correctChoice == lastChoice;
    }

    private void PlayDialogue() {
        dialogueUI.dialogueEndEvent.AddListener(MakeChoice);
        dialogueUI.StartDialogue(dialogue);
    }

    private void MakeChoice(string str) {
        eventController.EndEvent();
        dialogueUI.dialogueEndEvent.RemoveListener(MakeChoice);
        switch (str) {
            case "unetice":
                SetItem(uneticeBottle);
                break;
            case "staropramen":
                SetItem(staropramenBottle);
                break;
            case "starobrno":
                SetItem(starobrnoBottle);
                break;
            case "plzen":
                SetItem(plzenBottle);
                break;
            default:
                Debug.LogError("Dialogue contains invalid choice!", gameObject);
                break;
        }
        lastChoice = str;
        changeEvent.Invoke();
    }

    private void SetItem(GameObject item) {
        // Destroy any held objects (should always be one or none)
        if (itemSlot.transform.childCount > 0) {
            foreach (Transform child in itemSlot.transform) {
                Destroy(child.gameObject);
            }
        }

        // Can't instantiate item as a child of itemSlot, because of scaling issues
        GameObject newItem = Instantiate(item);
        newItem.transform.parent = itemSlot.transform;

        // Set items position and rotation the same as the itemSlot (so make local values zero)
        newItem.transform.localPosition = Vector3.zero;
        newItem.transform.localRotation = Quaternion.identity;
    }
}
