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


        if (Input.GetKeyDown(KeyCode.Alpha1)) {
            
            if(perks_.buyPerk(Perks.Speed)) {

                StartCoroutine(speedRemover());
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha2)) {
            
            if(perks_.buyPerk(Perks.Size)) {

                StartCoroutine(sizeRemover());
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha3)) {
            
            if(perks_.buyPerk(Perks.MashroomSpeed)) {
                
                CmdChangeSpeed(10);
                StartCoroutine(mashroomSpeedRemover());
            }
        }
    }

    [Command]
    private void CmdChangeSpeed (float factor) {

        graphWalker_.speed *= factor;
    }

     //Kalische
    private IEnumerator speedRemover () {

        yield return new WaitForSeconds(5);
        perks_.removePerk(Perks.Speed);
    }

    private IEnumerator sizeRemover () {

        yield return new WaitForSeconds(5);
        perks_.removePerk(Perks.Size);
    }

    private IEnumerator mashroomSpeedRemover () {

        yield return new WaitForSeconds(5);
        perks_.removePerk(Perks.MashroomSpeed);

        CmdChangeSpeed(0.1f);
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
    void CmdSpawnMinion(GameObject runnerStart, GameObject runnerEnd, float speedFactor, float sizeFuck)
    {
        // Setup runner
        var runner_obj = Instantiate(runnerPrefab, runnerStart.transform);
        var runner = runner_obj.GetComponent<Runner>();

        runner_obj.GetComponent<GraphWalker>().speed *= speedFactor;
        runner.transform.localScale = new Vector3(sizeFuck, sizeFuck, sizeFuck);

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
        float speedFactor = 1f, sizeFuck = 1f;

        if (perks_.isHasPerk(Perks.Speed)) {

            speedFactor *= 5f;
            sizeFuck /= 2f;
        }

        if (perks_.isHasPerk(Perks.Size)) {

            speedFactor /= 5f;
            sizeFuck *= 2f;
        }

        if (IsMoving()) return; // can't spawn minion while moving
        CmdSpawnMinion(graphWalker_.currentNode, target, speedFactor, sizeFuck);
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
