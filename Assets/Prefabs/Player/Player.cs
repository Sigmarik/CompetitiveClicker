using Mirror;
using UnityEngine;

public class Player : NetworkBehaviour
{
    public Team team;
    public GameObject runnerPrefab;

    public GraphWalker graphWalker_;

    //--------------------------------------------------

    public virtual void Update()
    {
        if (isServer) {
            TryEscape();
        }

        if (!isLocalPlayer) return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            var runnerEnd = GameObject.Find("Sphere");
        
            if (runnerEnd == graphWalker_.currentNode) {
                runnerEnd = GameObject.Find("Cube");
            }

            TrySpawnMinion(runnerEnd);
        }
    }

    [Server]
    public void Init(GameObject startNode)
    {
        SaveGraphWalker();
        TeleportTo(startNode);
        RpcInit(startNode);
    }

    [ClientRpc]
    void RpcInit(GameObject startNode) {
        SaveGraphWalker();
        TeleportTo(startNode);
    }

    [Command(requiresAuthority = false)]
    void CmdSpawnMinion(GameObject runnerStart, GameObject runnerEnd)
    {
        // Setup runner
        var runner_obj = Instantiate(runnerPrefab, runnerStart.transform);
        var runner = runner_obj.GetComponent<Runner>();
        runner.Init(runnerStart, team);
        // Spawn on all nodes
        NetworkServer.Spawn(runner_obj);

        // Begin walking
        runner.SendTo(runnerEnd);
    }

    private void SaveGraphWalker()
    {
        TryGetComponent<GraphWalker>(out GraphWalker walker);

        if (walker == null)
        {
            Debug.LogError("Missing `GraphWalker` component.");
            return;
        }

        graphWalker_ = walker;
    }

    // checks target node team
    // and goes there
    [Command(requiresAuthority = false)]
    public void CmdTryGoTo(GameObject target)
    {
        if (IsMoving()) return; // can't spawn minion while moving
        if (target == graphWalker_.currentNode) return;
        graphWalker_.GoTo(target);
    }

    void TeleportTo(GameObject target)
    {
        graphWalker_.Bind(target);
        var target_pos = target.GetComponent<Transform>().position;
        GetComponent<Transform>().position = target_pos;
    }

    // spawns minion under player
    // works only when player is standing still
    public void TrySpawnMinion(GameObject target)
    {
        if (IsMoving()) return; // can't spawn minion while moving
        CmdSpawnMinion(graphWalker_.currentNode, target);
    }

    [Server]
    protected void TryEscape()
    {
        if (IsMoving())                   return;
        if (GetCurrentNodeTeam() == team) return;

        //--------------------------------------------------

        GraphNavigator navigator = graphWalker_.currentNode.GetComponent<GraphNavigator>();
        GameObject    escapeTile = navigator.FindTeamNode(team);

        if (escapeTile == null) { Die(); return; }
        graphWalker_.GoTo(escapeTile);
    }

    void Die()
    {
        Destroy(gameObject);
    }

    //--------------------------------------------------

    private bool IsMoving()
    {
        return graphWalker_.hopInfo.stage == GraphWalker.HopInfo.HopStage.OnTheWay;
    }

    private Team GetCurrentNodeTeam()
    {
        return graphWalker_.currentNode.GetComponent<ScoreHolder>().team;
    }
}
