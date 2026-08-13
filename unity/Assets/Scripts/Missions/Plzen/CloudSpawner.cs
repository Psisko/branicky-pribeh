using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudSpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] clouds;

    // Clouds will be spawned randomly between these two points
    [SerializeField] private Transform point1;
    [SerializeField] private Transform point2;
    [SerializeField] private float minPause;
    [SerializeField] private float maxPause;

    private void Start() {
        StartCoroutine(SpawningLoop());
    }

    private IEnumerator SpawningLoop() {
        while (true) {
            yield return new WaitForSeconds(Random.Range(minPause, maxPause));
            Instantiate(
                clouds[Random.Range(0, clouds.Length)],
                Vector2.Lerp(point1.position, point2.position, Random.Range(0f, 1f)),
                Quaternion.identity
                ) ;
        }
    }
}
