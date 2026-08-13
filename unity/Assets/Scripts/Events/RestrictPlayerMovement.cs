using System.Collections;
using UnityEngine;


[RequireComponent(typeof(EventController))]
public class RestrictPlayerMovement : MonoBehaviour {

    [SerializeField] private Rectangle confines;

    PlayerController player;

    private void Start() {
        EventController controller = GetComponent<EventController>();
        controller.eventStart.AddListener(RestrictMovement);
        controller.eventEnd.AddListener(UnrestrictMovement);
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    private void RestrictMovement() {
        player.SetConfinedArea(confines);
    }

    private void UnrestrictMovement() {
        player.UnsetConfinedArea();
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(new(confines.minX, confines.minY), new(confines.maxX, confines.minY));
        Gizmos.DrawLine(new(confines.maxX, confines.minY), new(confines.maxX, confines.maxY));
        Gizmos.DrawLine(new(confines.maxX, confines.maxY), new(confines.minX, confines.maxY));
        Gizmos.DrawLine(new(confines.minX, confines.maxY), new(confines.minX, confines.minY));
    }

}
