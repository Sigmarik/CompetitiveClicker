using Mirror;
using UnityEngine;
using System.Collections;


// КАПСОМ выделены методы, которых не существует,
// но которые нужно бы добавить
public class Player : NetworkBehaviour
{
    public Team team;
    public GameObject runnerPrefab;

    private GraphWalker graphWalker_;
    private PlayerPerksShop perks_;

    private float speed_;

    //--------------------------------------------------

    void Awake()
    {
        perks_= new PlayerPerksShop(team);
    }

    void Update()
    {
        if (isServer) {
            TryEscape();
        }

        if (!isLocalPlayer) return;


        if (Input.GetKeyDown(KeyCode.Space)) {
            
            if(perks_.buyPerk(Perks.Speed)) {

                StartCoroutine(speedRemover());
            }
        }
    }

     //Kalische
    private IEnumerator speedRemover () {

        yield return new WaitForSeconds(5);
        perks_.removePerk(Perks.Speed);
    }
    //End of Kalische (jokes on you it will never ever ends)

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
    void CmdSpawnMinion(GameObject runnerStart, GameObject runnerEnd, float speedFactor)
    {
        // Setup runner
        var runner_obj = Instantiate(runnerPrefab, runnerStart.transform);
        var runner = runner_obj.GetComponent<Runner>();

        runner_obj.GetComponent<GraphWalker>().speed *= speed_;
        runner_obj.GetComponent<Transform>().localScale *= speed_;

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
    [Command]
    public void CmdTryGoTo(GameObject target)
    {
        if (IsMoving()) return; // can't spawn minion while moving
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
        speed_ = 1.0f;
        if (perks_.isHasPerk(Perks.Speed))
            speed_ = 5.0f;

        if (IsMoving()) return; // can't spawn minion while moving
        CmdSpawnMinion(graphWalker_.currentNode, target, 1);
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
        return graphWalker_.currentNode.GetComponent<ScoreHolder>().team;
    }
}
