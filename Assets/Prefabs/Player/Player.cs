using UnityEngine;

// КАПСОМ выделены методы, которых не существует,
// но которые нужно бы добавить
public class Player : MonoBehaviour
{
    public Team team;
    private GraphWalker graphWalker_;

    //--------------------------------------------------

    void Start()
    {
        
    }

    void Update()
    {

    }

    void Init(GameObject startNode, Team newTeam)
    {
        SaveGraphWalker();
        TeleportTo(startNode);
        team = newTeam;
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

    // goes to node if the teams match
    void TryGoTo(GameObject target)
    {
        // TODO: uncomment
        // if (team != target.GetComponent<SCORE_HOLDER>().TEAM) return;
        graphWalker_.GoTo(target);
    }

    void TeleportTo(GameObject target)
    {
        graphWalker_.Bind(target);
        Vector3 target_pos = target.GetComponent<Transform>().position;
        GetComponent<Transform>().position = target_pos;
    }

    // spawns minion under player
    // works only when player is standing still
    void TrySpawnMinion(GameObject target)
    {
        if (graphWalker_.hopInfo.stage == GraphWalker.HopInfo.HopStage.OnTheWay) return;
        // TODO: uncomment
        // if (team != target.GetComponent<SCORE_HOLDER>().TEAM) return;
        //  CmdSpawnMinion(graphWalker_.currentNode_, target);
    }

    void Escape()
    {
        GraphNavigator navigator = graphWalker_.currentNode.GetComponent<GraphNavigator>();
        GameObject    escapeTile = navigator.FindTeamNode(team);

        if (escapeTile == null) { Die(); return; }
        graphWalker_.GoTo(escapeTile);
    }

    void Die()
    {
        Destroy(gameObject);
    }
}
