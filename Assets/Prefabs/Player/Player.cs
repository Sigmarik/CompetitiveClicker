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

    void Init(GameObject start_node, Team team)
    {
        SaveGraphWalker();
        TeleportTo(start_node);
        team = team;
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
        if (!target.GetComponent<SCORE_HOLDER>()) return;
        if (team != target.GetComponent<SCORE_HOLDER>().TEAM) return; // can only go to my node
        graphWalker_.GoTo(target);
    }

    void TeleportTo(GameObject target)
    {
        graphWalker_.Bind(place);
        target_pos = target.GetComponent<Transform>().position;
        GetComponent<Transform>.SetPosition(target_pos);
    }

    // spawns minion under player
    // works only when player is standing still
    void TrySpawnMinion(GameObject target)
    {
        // TODO: поправить состояния движения
        if (graphWalker_.hopInfo.stage == OnTheWay) return; // can't spawn minion while moving
        if (!target.GetComponent<SCORE_HOLDER>()) return;             // can only go to enemy node
        if (team != target.GetComponent<SCORE_HOLDER>().TEAM) return; // can only go to enemy node
        CmdSpawnMinion(graphWalker_.currentNode_, target);
    }

    void Escape()
    {

    }

    void Lose()
    {
        
    }
}
