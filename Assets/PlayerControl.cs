using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class PlayerControl : NetworkBehaviour
{
    public GameObject runnerPrefab;

    void Update()
    {
        if (!isLocalPlayer) return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            var runnerStart = GameObject.Find("Cube");
            var runnerEnd = GameObject.Find("Sphere");
        
            CmdSpawnRunner(runnerStart, runnerEnd);
        }
    }

    [Command]
    void CmdSpawnRunner(GameObject runnerStart, GameObject runnerEnd)
    {
        // Setup runner
        var runner_obj = Instantiate(runnerPrefab);
        var runner = runner_obj.GetComponent<Runner>();
        runner.Init(runnerStart);
        // Spawn on all nodes
        NetworkServer.Spawn(runner_obj);

        // Begin walking
        runner.SendTo(runnerEnd);
    }
}
