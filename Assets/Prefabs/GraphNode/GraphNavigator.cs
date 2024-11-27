using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
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

        BezierKnot first = spline[0];
        BezierKnot last = spline[spline.Count - 1];

        first.Position = transform.position - route.path.transform.position;
        last.Position = target.transform.position - route.path.transform.position;

        spline.SetKnot(0, first);
        spline.SetKnot(spline.Count - 1, last);

        spline.SetTangentMode(0, TangentMode.AutoSmooth);
        spline.SetTangentMode(spline.Count - 1, TangentMode.AutoSmooth);
    }

    [System.Serializable]
    public class RouteEntry
    {
        [RequireComponentAttribute(typeof(GraphNavigator))]
        public GameObject target;
        [RequireComponentAttribute(typeof(SplineContainer))]
        public GameObject path;
    }

    public List<RouteEntry> publicRoutes = new List<RouteEntry>();
}
