using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EventController))]
public class KidSpawner : MonoBehaviour {

    [SerializeField] private int kidsCount = 20;
    [SerializeField] private Transform[] scatterPoints;
    [SerializeField, Tooltip("Array of points, where the kids should run when hit.")] private Transform[] exitPoints;
    [SerializeField] private GameObject kidPf;
    [SerializeField] private GameObject kidWitKeyPf;

    private EventController eventController;
    private List<Transform> kids = new List<Transform>();

    private void Awake() {
        eventController = GetComponent<EventController>();
        if (eventController == null ) { Debug.LogError("No EventController found!"); }
    }

    private void Start() {
        eventController.eventStart.AddListener(SpawnKids);
    }

    /// <summary>
    /// Spawn kids on spawnpoints
    /// </summary>
    private void SpawnKids() {
        SpawnKid(kidWitKeyPf);
        for (int i = 0; i < kidsCount; i++) {
            SpawnKid(kidPf);
        }
    }

    private void SpawnKid(GameObject prefab) {
        GameObject go = Instantiate(prefab, scatterPoints[Random.Range(0, scatterPoints.Length)].position, Quaternion.identity);
        kids.Add(go.transform);
        go.GetComponent<KidController>().SetPoints(scatterPoints, exitPoints);
        go.GetComponent<EnemyMovement>().SetOtherEnemies(kids);

    }
}
