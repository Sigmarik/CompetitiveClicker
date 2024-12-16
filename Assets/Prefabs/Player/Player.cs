using UnityEngine;

// КАПСОМ выделены методы, которых не существует,
// но которые нужно бы добавить
public class Player : MonoBehaviour
{
    public Team team;
    private GraphWalker graphWalker_;

    //--------------------------------------------------

    void Update()
    {
        TryEscape();
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
        // if (team != target.GetComponent<NodeMechanics>().team) return;
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
        if (IsMoving()) return;
        if (GetCurrentNodeTeam() == team) return;
        // TODO: uncomment
        // CmdSpawnMinion(graphWalker_.currentNode_, target);
    }

    // escapes from node, if needed
    // TODO: uncomment
    // [Server]
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
        return Team.Spiders;
    }
}
