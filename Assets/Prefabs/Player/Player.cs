using System;
using Mirror;
using UnityEngine;
using System.Collections;


public class Player : NetworkBehaviour
{
    public Team team;
    public GameObject runnerPrefab;

    public GraphWalker graphWalker_;

    private ResourceBank bank_;
    private PlayerPerksShop perks_;

    //--------------------------------------------------

    void Awake()
    {
        perks_= new PlayerPerksShop(team);
    }

    public override void OnStartServer() {
        bank_ = FindObjectOfType<ResourceBank>();
    }

    public virtual void Update()
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

        GameObject center = GameObject.Find("crates_stacked");
        gameObject.transform.LookAt(center.transform);

        if (isLocalPlayer) {
            var camera = FindObjectOfType<CameraMovement>();
            Vector3 forward = gameObject.transform.forward;
            float azimuth = (float) Math.Atan2(forward.z, forward.x) * Mathf.Rad2Deg;
            camera.Teleport(gameObject.transform.position, azimuth);
        }
    }

    [Command(requiresAuthority = false)]
    void CmdSpawnMinion(GameObject runnerStart, GameObject runnerEnd, float speedFactor, float sizeFuck)
    {
        SpawnMinion(runnerStart, runnerEnd, speedFactor, sizeFuck);
    }

    void SpawnMinion(GameObject runnerStart, GameObject runnerEnd, float speedFactor, float sizeFuck)
    {
        if (bank_.GetMoney(team) < 1) {
            return;
        }

        bank_.SpendMoney(team, 1);

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

    [Command(requiresAuthority = false)]
    public void CmdGoTo(GameObject target)
    {
        GoTo(target);
    }

    public void GoTo(GameObject target)
    {
        graphWalker_.GoTo(target);
    }

    public void TryGoTo(GameObject target)
    {
        if (IsMoving()) return; // can't spawn minion while moving
        if (target == graphWalker_.currentNode) return;

        if (isServer) GoTo(target);
        else       CmdGoTo(target);
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

        if (isServer) SpawnMinion(graphWalker_.currentNode, target, 1.0f, 1.0f);
        else       CmdSpawnMinion(graphWalker_.currentNode, target, speedFactor, sizeFuck);
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
