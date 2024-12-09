using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Runner : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        TryGetComponent<GraphWalker>(out GraphWalker walker);

        if (walker == null)
        {
            Debug.LogError("Missing `GraphWalker` component.");
            return;
        }

        walker.BindAndSend(start, finish);
    }

    // Update is called once per frame
    void Update()
    {

    }

    [RequireComponentAttribute(typeof(GraphNavigator))]
    public GameObject start;
    [RequireComponentAttribute(typeof(GraphNavigator))]
    public GameObject finish;
}
