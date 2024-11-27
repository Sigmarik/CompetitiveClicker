using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using TreeEditor;
using UnityEditor;
using UnityEngine;
using UnityEngine.Splines;

public class GraphNavigator : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

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
        if (route.path == null)
        {
            return;
        }

        route.path.TryGetComponent(out SplineContainer splineCollection);

        if (splineCollection == null)
        {
            return;
        }

        if (route.target == null)
        {
            return;
        }

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
                Debug.LogWarning("A route cannot lead back to the object it comes from.");
                continue;
            }

            bool duplicate = false;

            foreach (RouteEntry otherRoute in newRoutes)
            {
                if (otherRoute.target == null)
                {
                    nullRouteExists = true;
                }

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
        }

        publicRoutes = newRoutes;
    }

    void ReplicatePath(RouteEntry route)
    {
        if (route.target == null)
        {
            return;
        }

        if (route.path == null)
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
}
