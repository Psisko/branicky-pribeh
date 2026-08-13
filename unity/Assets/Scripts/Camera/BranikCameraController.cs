using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BranikCameraController : MonoBehaviour
{
    public Transform target;
    //[SerializeField] private float speed = 8;

    [SerializeField] private float lerpSpeed = 4;
    [SerializeField] private Rectangle levelConfines1;
    [SerializeField] private Rectangle levelConfines2;

    private Camera cam;
    private Rectangle temporaryConfines = null;

    private void Awake()
    {
        cam = GetComponent<Camera>();
    }

    private void Start()
    {
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

        if (temporaryConfines != null)
        {
            ClampPosition(temporaryConfines);
        }
        else
        {   
            if (wantedPosition.y > -40)
            {
                ClampPosition(levelConfines1);     
            }
            else
            {
                ClampPosition(levelConfines2);
            }
        }

    }

    public void Confine(Rectangle rect)
    {
        temporaryConfines = rect;
    }

    public void Unconfine()
    {
        temporaryConfines = null;
    }

    private void ClampPosition(Rectangle rect)
    {
        Vector2 botLeft = cam.ViewportToWorldPoint(Vector3.zero);
        Vector2 topRight = cam.ViewportToWorldPoint(Vector3.one);

        Vector3 offset = Vector3.zero;
        if (botLeft.x < rect.minX)
        {
            offset.x = rect.minX - botLeft.x;
        }
        if (botLeft.y < rect.minY)
        {
            offset.y = rect.minY - botLeft.y;
        }
        if (topRight.x > rect.maxX)
        {
            offset.x = rect.maxX - topRight.x;
        }
        if (topRight.y > rect.maxY)
        {
            offset.y = rect.maxY - topRight.y;
        }
        transform.position += offset;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(new(levelConfines1.minX, levelConfines1.minY), new(levelConfines1.maxX, levelConfines1.minY));
        Gizmos.DrawLine(new(levelConfines1.maxX, levelConfines1.minY), new(levelConfines1.maxX, levelConfines1.maxY));
        Gizmos.DrawLine(new(levelConfines1.maxX, levelConfines1.maxY), new(levelConfines1.minX, levelConfines1.maxY));
        Gizmos.DrawLine(new(levelConfines1.minX, levelConfines1.maxY), new(levelConfines1.minX, levelConfines1.minY));
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(new(levelConfines2.minX, levelConfines2.minY), new(levelConfines2.maxX, levelConfines2.minY));
        Gizmos.DrawLine(new(levelConfines2.maxX, levelConfines2.minY), new(levelConfines2.maxX, levelConfines2.maxY));
        Gizmos.DrawLine(new(levelConfines2.maxX, levelConfines2.maxY), new(levelConfines2.minX, levelConfines2.maxY));
        Gizmos.DrawLine(new(levelConfines2.minX, levelConfines2.maxY), new(levelConfines2.minX, levelConfines2.minY));
    }
}
