using UnityEngine;

//spustí se event, když seberu tento předmět
// použit u klíče v uněticích
public class OnPickupStartEvent : MonoBehaviour {

    [SerializeField] private string eventObjectName;

    private void Start() {
        GetComponent<LootItem>().pickupEvent.AddListener(StartEvent);
    }

    private void StartEvent() {
        GameObject.Find(eventObjectName).GetComponent<EventController>().StartEvent();
    }
}