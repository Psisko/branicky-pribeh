using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target;
    //[SerializeField] private float speed = 8;

    [SerializeField] private float lerpSpeed = 4;
    [SerializeField] private Rectangle levelConfines;

    private Camera cam;
    private Rectangle temporaryConfines = null;

    private void Awake() {
        cam = GetComponent<Camera>();
    }

    private void Start() {
        Screen.SetResolution(1920, 1080, true);
        QualitySettings.vSyncCount = 1;
        Application.targetFrameRate = 60;

        transform.position = target.position;
    }

    // If camera movement is done in Update(), then all movement in fixed update is jittery.
    void FixedUpdate()
    {

        Vector3 wantedPosition = target.position;
        wantedPosition.z = -10;
        transform.position = Vector3.Lerp(transform.position, wantedPosition, Time.fixedDeltaTime * lerpSpeed);

        if (temporaryConfines != null) {
            ClampPosition(temporaryConfines);
        } else {
            ClampPosition(levelConfines);
        }

    }

    public void Confine(Rectangle rect) {
        temporaryConfines = rect;
    }

    public void Unconfine() {
        temporaryConfines = null;
    }

    private void ClampPosition(Rectangle rect) {
        Vector2 botLeft = cam.ViewportToWorldPoint(Vector3.zero);
        Vector2 topRight = cam.ViewportToWorldPoint(Vector3.one);

        Vector3 offset = Vector3.zero;
        if (botLeft.x < rect.minX) {
            offset.x = rect.minX - botLeft.x;
        }
        if (botLeft.y < rect.minY) {
            offset.y = rect.minY - botLeft.y;
        }
        if (topRight.x > rect.maxX) {
            offset.x = rect.maxX - topRight.x;
        }
        if (topRight.y > rect.maxY) {
            offset.y = rect.maxY - topRight.y;
        }
        transform.position += offset;
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(new(levelConfines.minX, levelConfines.minY), new(levelConfines.maxX, levelConfines.minY));
        Gizmos.DrawLine(new(levelConfines.maxX, levelConfines.minY), new(levelConfines.maxX, levelConfines.maxY));
        Gizmos.DrawLine(new(levelConfines.maxX, levelConfines.maxY), new(levelConfines.minX, levelConfines.maxY));
        Gizmos.DrawLine(new(levelConfines.minX, levelConfines.maxY), new(levelConfines.minX, levelConfines.minY));
    }
}
