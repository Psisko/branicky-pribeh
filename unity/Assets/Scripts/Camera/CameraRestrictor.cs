using System.Collections;
using UnityEngine;

[RequireComponent(typeof(EventController))]
public class CameraRestrictor : MonoBehaviour {

    [SerializeField] private Rectangle confines;

    private CameraController cam;
    private EventController eventController;

    private void Start() {
        cam = Camera.main.GetComponent<CameraController>();
        eventController = GetComponent<EventController>();
        eventController.eventStart.AddListener(Confine);
        eventController.eventEnd.AddListener(Unconfine);
    }


    private void Confine() {
        cam.Confine(confines);
    }

    private void Unconfine() {
        cam.Unconfine();
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(new(confines.minX, confines.minY), new(confines.maxX, confines.minY));
        Gizmos.DrawLine(new(confines.maxX, confines.minY), new(confines.maxX, confines.maxY));
        Gizmos.DrawLine(new(confines.maxX, confines.maxY), new(confines.minX, confines.maxY));
        Gizmos.DrawLine(new(confines.minX, confines.maxY), new(confines.minX, confines.minY));
    }

}
