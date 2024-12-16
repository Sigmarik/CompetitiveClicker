using Mirror;
using UnityEngine;

// КАПСОМ выделены методы, которых не существует,
// но которые нужно бы добавить
public class Player : NetworkBehaviour
{
    public Team team;
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

        // TryEscape();
    }

    [Server]
    public void Init(GameObject start_node)
    {
        SaveGraphWalker();
        TeleportTo(start_node);
        RpcInit(start_node);
    }

    [ClientRpc]
    void RpcInit(GameObject start_node) {
        SaveGraphWalker();
        TeleportTo(start_node);
    }

    [Command]
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
    void TryGoTo(GameObject target)
    {
        // TODO: поправить получение команды цели
        // if (!target.GetComponent<SCORE_HOLDER>()) return;
        // if (team != target.GetComponent<SCORE_HOLDER>().TEAM) return; // can only go to my node
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
    void TrySpawnMinion(GameObject target)
    {
        // TODO: поправить состояния движения
        if (graphWalker_.hopInfo.stage == GraphWalker.HopInfo.HopStage.OnTheWay) return; // can't spawn minion while moving
        // if (!target.GetComponent<SCORE_HOLDER>()) return;             // can only go to enemy node
        // if (team != target.GetComponent<SCORE_HOLDER>().TEAM) return; // can only go to enemy node
        CmdSpawnMinion(graphWalker_.currentNode, target);
    }

    [Server]
    void TryEscape()
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
        // TODO: uncomment
        // return graphWalker_.currentNode.GetComponent<NodeMechanics>().team;
        return Team.Default;
    }
}
