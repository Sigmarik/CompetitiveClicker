using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Splines;

public class GraphNavigator : MonoBehaviour
{
    void Start()
    {
        FixPathSplines();
        RemoveDuplicates();

        foreach (RouteEntry entry in publicRoutes)
        {
            ReplicatePath(entry);
        }

        BakeRoutes();
        firstUpdateTime = Time.time + 0.3f;
    }

    void BakeRoutes()
    {
        var distances = new Dictionary<GameObject, Tuple<RouteEntry, float>>();

        var queue = new Queue<GameObject>();

        foreach (RouteEntry neigh in publicRoutes)
        {
            distances[neigh.target] = new Tuple<RouteEntry, float>(neigh,
                neigh.path.GetComponent<SplineContainer>()[0].GetLength());
            queue.Enqueue(neigh.target);
        }

        while (queue.Count > 0)
        {
            GameObject destination = queue.First();
            queue.Dequeue();
            var navigator = destination.GetComponent<GraphNavigator>();
            var link = distances[destination];

            foreach (RouteEntry nextDest in navigator.publicRoutes)
            {
                SplineContainer container =
                    nextDest.path.GetComponent<SplineContainer>();
                float length = container[0].GetLength();

                if (distances.ContainsKey(nextDest.target) &&
                    distances[nextDest.target].Item2 <= link.Item2 + length)
                {
                    continue;
                }

                distances[nextDest.target] =
                    new Tuple<RouteEntry, float>(link.Item1,
                        link.Item2 + length);

                queue.Enqueue(nextDest.target);
            }
        }

        nextHop = new Dictionary<GameObject, RouteEntry>();
        foreach (var destination in distances)
        {
            nextHop[destination.Key] = destination.Value.Item1;
        }
    }

    private float firstUpdateTime = 0.0f;
    private bool needRebaking = true;

    void Update() {
        // Ugly fix, because path script is called after Bake or idk
        if (needRebaking && Time.time > firstUpdateTime) {
            BakeRoutes();
            needRebaking = false;
        }
    }

    public RouteEntry NextHopTo(GameObject target)
    {
        // Script meme
        if (!nextHop.ContainsKey(target)) {
            BakeRoutes();
        }

        return nextHop[target];
    }

    private void OnDrawGizmos()
    {
        FixPathSplines();
        RemoveDuplicates();

        foreach (RouteEntry entry in publicRoutes)
        {
            ReplicatePath(entry);
        }
    }

    public void FixPathSplines()
    {
        foreach (RouteEntry entry in publicRoutes)
        {
            FixPath(entry);
        }
    }

    private void FixPath(RouteEntry route)
    {
        if (route.path == null || route.target == null)
        {
            return;
        }

        SplineContainer splineCollection = route.path.GetComponent<SplineContainer>();
        GameObject target = route.target;

        if (splineCollection.Splines.Count == 0)
        {
            splineCollection.AddSpline(new Spline());
        }

        Spline spline = splineCollection.Spline;
        spline.Closed = false;
        while (spline.Count < 2)
        {
            spline.Add(new BezierKnot());
            spline.SetTangentMode(spline.Count - 1, TangentMode.AutoSmooth);
        }

        int firstIndex = 0;
        int lastIndex = spline.Count - 1;

        if (route.inverted)
        {
            firstIndex = lastIndex;
            lastIndex = 0;
        }

        BezierKnot first = spline[firstIndex];
        BezierKnot last = spline[lastIndex];

        first.Position = route.path.transform.worldToLocalMatrix.MultiplyPoint3x4(transform.position);
        last.Position = route.path.transform.worldToLocalMatrix.MultiplyPoint3x4(target.transform.position);

        spline.SetKnot(firstIndex, first);
        spline.SetKnot(lastIndex, last);

        spline.SetTangentMode(firstIndex, TangentMode.AutoSmooth);
        spline.SetTangentMode(lastIndex, TangentMode.AutoSmooth);
    }

    public void OnEditorUpdate()
    {
        RemoveDuplicates();

        foreach (RouteEntry entry in publicRoutes)
        {
            ReplicatePath(entry);
        }
    }

    void RemoveDuplicates()
    {
        List<RouteEntry> newRoutes = new List<RouteEntry>();

        bool nullRouteExists = false;

        foreach (RouteEntry route in publicRoutes)
        {
            if (route.target == gameObject)
            {
                Debug.LogWarning("A route cannot lead back to the object it belongs to.");
                continue;
            }

            bool duplicate = false;

            foreach (RouteEntry otherRoute in newRoutes)
            {
                if (otherRoute.target == route.target)
                {
                    duplicate = true;
                    route.target = null;
                    route.path = null;
                    route.inverted = false;
                    break;
                }
            }

            if (!duplicate || !nullRouteExists)
            {
                newRoutes.Add(route);
            }

            if (route.target == null)
            {
                nullRouteExists = true;
            }
        }

        publicRoutes = newRoutes;
    }

    void ReplicatePath(RouteEntry route)
    {
        if (route.target == null || route.path == null)
        {
            return;
        }

        GraphNavigator neighborNavigator = route.target.GetComponent<GraphNavigator>();
        foreach (RouteEntry entry in neighborNavigator.publicRoutes)
        {
            if (entry.target == gameObject)
            {
                entry.path = route.path;
                entry.inverted = !route.inverted;
                return;
            }
        }

        RouteEntry neighborRoute = new RouteEntry();
        neighborRoute.target = gameObject;
        neighborRoute.path = route.path;
        neighborRoute.inverted = !route.inverted;

        neighborNavigator.publicRoutes.Add(neighborRoute);
    }

    // finds node with matching team
    public GameObject FindTeamNode(Team team)
    {
        if (FindTeamNodes(team).Count == 0) return null;
        return FindTeamNodes(team) [0];
    }

    // finds a random node with matching team
    public GameObject FindRandomTeamNode(Team team)
    {
        List<GameObject> teamNodes = FindTeamNodes(team);
        if (teamNodes.Count == 0) return null;

        int randomIndex = UnityEngine.Random.Range(0, teamNodes.Count);
        return teamNodes[randomIndex];
    }

    // finds a random node with other team
    public GameObject FindRandomEnemyTeamNode(Team team)
    {
        List<GameObject> enemyTeamNodes = FindEnemyTeamNodes(team);
        if (enemyTeamNodes.Count == 0) return null;

        int randomIndex = UnityEngine.Random.Range(0, enemyTeamNodes.Count);
        return enemyTeamNodes[randomIndex];
    }

    // finds all nodes with matching team
    public List<GameObject> FindTeamNodes(Team team)
    {
        List<GameObject> answer = new List<GameObject>();

        foreach (var hop in nextHop)
        {
            GameObject node = hop.Key;
            if (team != node.GetComponent<ScoreHolder>().team) continue;
            answer.Add(node);
        }

        return answer;
    }

    // finds all nodes with matching team
    public List<GameObject> FindEnemyTeamNodes(Team team)
    {
        List<GameObject> answer = new List<GameObject>();

        foreach (var hop in nextHop)
        {
            GameObject node = hop.Key;
            if (team == node.GetComponent<ScoreHolder>().team) continue;
            answer.Add(node);
        }

        return answer;
    }

    [System.Serializable]
    public class RouteEntry
    {
#if UNITY_EDITOR
        [RequireComponentAttribute(typeof(GraphNavigator))]
#endif
        public GameObject target;
#if UNITY_EDITOR
        [RequireComponentAttribute(typeof(SplineContainer))]
#endif
        public GameObject path;

        public bool inverted = false;
    }

    public List<RouteEntry> publicRoutes = new List<RouteEntry>();
    Dictionary<GameObject, RouteEntry> nextHop = new Dictionary<GameObject, RouteEntry>();
}
