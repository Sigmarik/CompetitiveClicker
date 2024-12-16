using UnityEngine;

// КАПСОМ выделены методы, которых не существует,
// но которые нужно бы добавить
public class Player : MonoBehaviour
{
    public Team team;
    public MinionSpawner minionSpawner;

    private GraphWalker graphWalker_;

    //--------------------------------------------------

    void Start()
    {
        TryGetComponent<GraphWalker>(out GraphWalker walker);

        if (walker == null)
        {
            Debug.LogError("Missing `GraphWalker` component.");
            return;
        }

        graphWalker_ = walker;
    }

    void Update()
    {

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

    // spawns minion under player
    // works only when player is standing still
    void TrySpawnMinion(GameObject target)
    {
        // TODO: поправить состояния движения
        if (graphWalker_.IS_WALKING()) return; // can't spawn minion while moving
        minionSpawner.SpawnMinion(graphWalker_.CURRENT_NODE, target);
    }
}
