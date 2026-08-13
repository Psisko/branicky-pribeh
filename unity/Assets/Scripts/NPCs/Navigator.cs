using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Aoiti.Pathfinding;
using static UnityEngine.UI.Image;

public class Navigator : MonoBehaviour
{
    // For editor purposes
    [SerializeField] private List<Transform> initialNodes;
    [SerializeField] private List<NodeConnection> initialConnections;
    [SerializeField] private bool showPaths = true;


    // For other purposes
    private Pathfinder<Vector3> pathfinder;
    private Dictionary<Vector3, GraphNode> graph;



    private void Awake() {
        pathfinder = new Pathfinder<Vector3>(GetDistance, GetNeighbourNodes);
        CreateGraph();
    }



    public List<Vector3> GetPath(Vector3 from, Vector3 to) {
        pathfinder.GenerateAstarPath(from, to, out List<Vector3> path);
        path.Insert(0, from);
        return path;
    }

    public List<Vector3> GetPathFromMe(Vector3 origin, Vector3 to) {
        return GetPath(GetClosestNode(origin), to);
    }



    public Vector3 GetClosestNode(Vector2 origin) {
        // This should not get value from initialNodes, because the actual collection is 'graph'.
        // But in this point in time I can't care less.

        Vector3 res = initialNodes[0].position;
        float minDistance = Vector2.Distance(res, origin);

        foreach (var point in graph) {
            float distance = Vector2.Distance(point.Key, origin);
            if (distance < minDistance) {
                res = point.Key;
                minDistance = distance;
            }
        }
        return res;
    }




    /// <summary>
    ///     Returns squared distance between points.
    /// </summary>
    private float GetDistance(Vector3 a, Vector3 b) {
        return (a - b).sqrMagnitude;
    }



    private Dictionary<Vector3, float> GetNeighbourNodes(Vector3 vector) {
        return graph[vector].connections;
    }



    /// <summary>
    ///     To allow simple editor exposure of graph, the nodes and connections are initially stored in Lists.
    ///     This is not effective, so these Lists need to be converted into Dictionary.
    /// </summary>
    private void CreateGraph() {
        graph = new();
        foreach (var connection in initialConnections) {

            if (!graph.ContainsKey(connection.a.position)) { graph.Add(connection.a.position, new()); }
            if (!graph.ContainsKey(connection.b.position)) { graph.Add(connection.b.position, new()); }

            graph[connection.a.position].connections.Add(
                connection.b.position,
                GetDistance(connection.a.position, connection.b.position)
            );
            graph[connection.b.position].connections.Add(
                connection.a.position,
                GetDistance(connection.b.position, connection.a.position)
            );
        }
    }




    private void OnDrawGizmos() {
        if (!showPaths) {
            return;
        }
         
        if (graph == null) {
            // graph doesn't exist, therefore we are not in play mode
            Gizmos.color = Color.blue;
            foreach (var connection in initialConnections) {
                Gizmos.DrawLine(connection.a.position, connection.b.position);
            }
        } else {
            // graph has some nodes, therefore we must be in play mode
            Gizmos.color = Color.blue;
            foreach (var node in graph) {
                Gizmos.DrawSphere(node.Key, 0.2f);

                foreach (var connection in node.Value.connections) {
                    Gizmos.DrawLine(node.Key, connection.Key);
                    // draw sphere at the further side of the line, acting as an arrow,
                    // so we can see the direction of a connection
                    Gizmos.DrawWireSphere(
                        Vector3.Lerp(node.Key, connection.Key, 0.9f),
                        0.05f
                    );
                }
            }
        }
    }



    [ContextMenu("Find nodes")]
    private void FindNodesContextMenu() {
        initialNodes.Clear();
        Transform nodesContainer = transform.Find("Nodes");
        foreach (var n in nodesContainer.GetComponentsInChildren<Transform>()) {
            if (n == nodesContainer) continue;
            if (!initialNodes.Contains(n)) {
                initialNodes.Add(n);
            }
        }
    }



    [ContextMenu("Make automatic connections")]
    private void MakeAutomaticConnectionsContextmenu() {
        initialConnections.Clear();
        float maxDistance = 10;

        ContactFilter2D contactFilter = new ContactFilter2D();

        contactFilter.useTriggers = false;

        RaycastHit2D[] hits = new RaycastHit2D[20];

        foreach (var from in initialNodes) {
            foreach (var to in initialNodes) {
                if (from.Equals(to)) continue;

                if ((from.position - to.position).sqrMagnitude > maxDistance*maxDistance) {
                    continue;
                }

                
                if (Physics2D.Linecast(from.position, to.position, contactFilter, hits) > 0) {
                    continue;
                }
                

                NodeConnection connection = new NodeConnection(from, to);
                if (!initialConnections.Contains(connection)) {
                    initialConnections.Add(connection);
                }

            }
        }
    }


}



public class GraphNode {
    public Dictionary<Vector3, float> connections = new();
}



[Serializable]
public struct NodeConnection {
    public Transform a;
    public Transform b;

    public NodeConnection(Transform a, Transform b) {
        this.a = a;
        this.b = b;
    }



    // override object.Equals
    public override bool Equals(object obj) {
        //       
        // See the full list of guidelines at
        //   http://go.microsoft.com/fwlink/?LinkID=85237  
        // and also the guidance for operator== at
        //   http://go.microsoft.com/fwlink/?LinkId=85238
        //

        if (obj == null || GetType() != obj.GetType()) {
            return false;
        }

        if (a == ((NodeConnection)obj).a && b == ((NodeConnection)obj).b) {
            return true;
        }
        if (a == ((NodeConnection)obj).b && b == ((NodeConnection)obj).a) {
            return true;
        }

        return false;

    }

    // override object.GetHashCode
    public override int GetHashCode() {
        return (a, b).GetHashCode();
    }
}
