using UnityEngine;
using Mirror;

// КАПСОМ выделены методы, которых не существует,
// но которые нужно бы добавить
public class Player : NetworkBehaviour
{
    [SyncVar]
    public Team team_;
    public GameObject runnerPrefab;

    private GraphWalker graphWalker_;

    //--------------------------------------------------

    void Update()
    {
        if (!isLocalPlayer) return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            var runnerEnd = GameObject.Find("Sphere");
        
            if (runnerEnd == graphWalker_.currentNode) {
                runnerEnd = GameObject.Find("Cube");
            }

            TrySpawnMinion(runnerEnd);
        }

        TryEscape();
    }

    [Server]
    public void Init(GameObject start_node, Team team)
    {
        SaveGraphWalker();
        TeleportTo(start_node);
        team_ = team;
        RpcInit(start_node);
    }

    [ClientRpc]
    void RpcInit(GameObject start_node) {
        SaveGraphWalker();
        TeleportTo(start_node);
    }

    // checks target node team
    // and goes there
    public void TryGoTo(GameObject target)
    {
        if (!IsSameTeamAs(target)) return;
        graphWalker_.GoTo(target);
    }

    // spawns minion under player
    // works only when player is standing still
    public void TrySpawnMinion(GameObject target)
    {
        if (IsMoving())           return;
        if (IsSameTeamAs(target)) return;
        CmdSpawnMinion(graphWalker_.currentNode, target);
    }

    //--------------------------------------------------

    void TeleportTo(GameObject target)
    {
        graphWalker_.Bind(target);
        var target_pos = target.GetComponent<Transform>().position;
        GetComponent<Transform>().position = target_pos;
    }

    [Server]
    void TryEscape()
    {
        if (IsMoving())                             return;
        if (IsSameTeamAs(graphWalker_.currentNode)) return;

        //--------------------------------------------------

        GraphNavigator navigator = graphWalker_.currentNode.GetComponent<GraphNavigator>();
        GameObject    escapeTile = navigator.FindTeamNode(team_);

        if (escapeTile == null) { Die(); return; }
        graphWalker_.GoTo(escapeTile);
    }

    void Die()
    {
        Destroy(gameObject);
    }

    //--------------------------------------------------

    bool IsMoving()
    {
        return graphWalker_.hopInfo.stage == GraphWalker.HopInfo.HopStage.OnTheWay;
    }

    bool IsSameTeamAs(GameObject obj)
    {
        return team_ == obj.GetComponent<ScoreHolder>().logic.scoreData.team;
    }

    //--------------------------------------------------

    [Command]
    void CmdSpawnMinion(GameObject runnerStart, GameObject runnerEnd)
    {
        // Setup runner
        var runner_obj = Instantiate(runnerPrefab);
        var runner = runner_obj.GetComponent<Runner>();
        runner.Init(runnerStart, team_);
        // Spawn on all nodes
        NetworkServer.Spawn(runner_obj);

        // Begin walking
        runner.SendTo(runnerEnd);
    }

    void SaveGraphWalker()
    {
        TryGetComponent<GraphWalker>(out GraphWalker walker);

        if (walker == null)
        {
            Debug.LogError("Missing `GraphWalker` component.");
            return;
        }

        graphWalker_ = walker;
    }

}
