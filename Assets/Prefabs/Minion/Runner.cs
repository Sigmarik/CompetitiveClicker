using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class Runner : NetworkBehaviour
{
    [Server] 
    public void Init(GameObject start)
    {
        TryGetComponent<GraphWalker>(out GraphWalker walker);

        if (walker == null)
        {
            Debug.LogError("Missing `GraphWalker` component.");
            return;
        }

        walker.Bind(start);
    }

    [Server] 
    public void SendTo(GameObject finish)
    {
        TryGetComponent<GraphWalker>(out GraphWalker walker);

        if (walker == null)
        {
            Debug.LogError("Missing `GraphWalker` component.");
            return;
        }

        walker.GoTo(finish);
    }
}
