using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

class ScatterPointManager : MonoBehaviour {

    [SerializeField] private float safeDistance = 5f;
    [SerializeField] private float closeDistance = 5f;


    private List<Transform> scatterPoints;

    public void SetScatterPoints(List<Transform> sp) { 
        scatterPoints = sp; 
    }

    /// <summary>
    ///     Finds scatter point furthest from the origin.
    /// </summary>
    public Transform GetFurthest(Vector2 origin) {
        Transform res = scatterPoints[0];
        float maxDistance = Vector2.Distance(res.position, origin);
        foreach (Transform point in scatterPoints) {
            float distance = Vector2.Distance(point.position, origin);
            if (distance > maxDistance) {
                res = point;
                maxDistance = distance;
            }
        }

        return res;
    }

    /// <summary>
    ///     Finds some safe scatter point.
    /// </summary>
    /// <returns>
    ///     A point that satisfies the safe distance requierement. Or if there aren't any
    ///     returns the furthest from the player.
    /// </returns>
    public Transform GetSafe(Vector2 origin) {
        List<Transform> safePoints = GetPointsSatisfying((point) => {
            return Vector3.Distance(point.position, origin) > safeDistance;
        });

        if (safePoints.Count > 0) {
            return safePoints[Random.Range(0, safePoints.Count)];
        } else {
            return GetFurthest(origin);
        }
    }

    public Transform GetClose(Vector2 origin) {
        List<Transform> closePoints = GetPointsSatisfying((point) => {
            return Vector3.Distance(point.position, origin) > closeDistance;
        });

        if (closePoints.Count > 0) {
            return closePoints[Random.Range(0, closePoints.Count)];
        } else {
            return GetClosest(origin);
        }
    }




    private List<Transform> GetPointsSatisfying(Func<Transform, bool> condition) {
        List<Transform> satisfyingPoints = new List<Transform>();
        foreach (Transform point in scatterPoints) {
            if (condition(point)) {
                satisfyingPoints.Add(point);
            }
        }

        return satisfyingPoints;
    }

    private Transform GetClosest(Vector2 origin) {
        Transform res = scatterPoints[0];
        float minDistance = Vector2.Distance(res.position, origin);
        foreach (Transform point in scatterPoints) {
            float distance = Vector2.Distance(point.position, origin);
            if (distance < minDistance) {
                res = point;
                minDistance = distance;
            }
        }

        return res;
    }
}