using Mirror;
using UnityEngine;

public class Bot : Player
{
    private BotActionGenerator actionGenerator_;
    private BotAction          currentAction_ = new BotActionAFK();

    // TODO: someone check this funtion
    [Server]
    public new void Init(GameObject startNode)
    {
        base.Init(startNode);

        actionGenerator_ = GetComponent<BotActionGenerator>();
        actionGenerator_.Init();

        InvokeRepeating("Act", 2f, 2f);
        InvokeRepeating("ChangeAction", 2f, 2f);
    }

    public override void Update()
    {
        if (isServer) {
            TryEscape();
        }
    }

    [Server]
    void Act() {

        currentAction_.Act(this);
    }

    void ChangeAction() {

        currentAction_ = actionGenerator_.GetRandomAction();
    }
}
