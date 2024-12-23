using Mirror;
using UnityEngine;

public class Bot : Player
{
    private BotAction          currentAction_ = new BotActionAFK();
    private BotActionGenerator actionGenerator_;

    // TODO: someone check this funtion
    [Server]
    public new void Init(GameObject startNode)
    {
        base.Init(startNode);

        actionGenerator_ = GetComponent<BotActionGenerator>();

        InvokeRepeating("Act", 2f, 2f);
        InvokeRepeating("ChangeAction", 2f, 2f);
    }

    public override void Update()
    {
        if (isServer) {
            TryEscape();
        }
    }

    void Act() {

        currentAction_.Act(this);
    }

    void ChangeAction() {

        currentAction_ = actionGenerator_.GetRandomAction();
    }
}
