using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Splines;

public class GraphNavigator : MonoBehaviour
{
    void Start()
    {
        BakeRoutes();
    }

    void BakeRoutes()
    {
        // Target => best nexthop & it's length
        var distances = new Dictionary<GameObject, Tuple<RouteEntry, int>>();

        // Queue of destinations 
        var queue = new Queue<GameObject>();

        // Populate with neighbors
        foreach (RouteEntry neigh in publicRoutes)
        {
            distances[neigh.target] = new Tuple<RouteEntry, int>(neigh, 1);
            queue.Append(neigh.target);
        }

        // Main loop (basically BFS)
        // - Kinda expensive and probably could be better (redoing other nodes work)
        // - Also using O(nodes) GetComponent
        foreach (GameObject destination in queue)
        {
            // Retrieving GraphNavigator 
            var navigator = destination.GetComponent<GraphNavigator>();
            var link = distances[destination];

            // Checking it's neighbors
            foreach (RouteEntry next_dest in navigator.publicRoutes)
            {
                // Skip if too long
                if (distances.ContainsKey(next_dest.target) && distances[next_dest.target].Item2 >= link.Item2 + 1)
                {
                    continue;
                }

                // Update distance
                distances[next_dest.target] = new Tuple<RouteEntry, int>(link.Item1, link.Item2 + 1);
            }
        }

        // Bake into nextHop
        nextHop = new Dictionary<GameObject, RouteEntry>();
        foreach (var destination in distances)
        {
            nextHop[destination.Key] = destination.Value.Item1;
        }
    }

    public RouteEntry NextHopTo(GameObject target)
    {
        return nextHop[target];
    }

    private void OnDrawGizmos()
    {
        FixPathSplines();
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

        first.Position = transform.position - route.path.transform.position;
        last.Position = target.transform.position - route.path.transform.position;

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

    [System.Serializable]
    public class RouteEntry
    {
        [RequireComponentAttribute(typeof(GraphNavigator))]
        public GameObject target;
        [RequireComponentAttribute(typeof(SplineContainer))]
        public GameObject path;

        public bool inverted = false;
    }

    public List<RouteEntry> publicRoutes = new List<RouteEntry>();
    Dictionary<GameObject, RouteEntry> nextHop = new Dictionary<GameObject, RouteEntry>();
}
