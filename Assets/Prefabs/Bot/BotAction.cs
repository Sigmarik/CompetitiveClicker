using System.Threading.Tasks;
using Mirror;
using UnityEngine;

public class BotAction {

    public virtual void Act(Bot bot) {}
}

public class BotActionAFK: BotAction {}

// sends n minions to one node
public class BotActionSpawnLine: BotAction {

    GameObject target_;
    int spawnCount_;

    const int SPAWN_DELAY = 1000;

    //--------------------------------------------------

    public BotActionSpawnLine(GameObject target, int spawnCount)
    {
        target_     = target;
        spawnCount_ = spawnCount;
    }

    public override async void Act(Bot bot)
    {
        for (int i = 0; i < spawnCount_; ++i)
        {
            var task = SpawnMinionWithDelay(bot, target_, i);
            await task;
        }
    }

    private async Task SpawnMinionWithDelay(Bot bot, GameObject target, int index)
    {
        await Task.Delay(index * SPAWN_DELAY);
        bot.TrySpawnMinion(target);
    }
}

// sends n minions to random nodes
public class BotActionSpawnHorde: BotAction {

    int spawnCount_;

    public BotActionSpawnHorde(int spawnCount)
    {
        spawnCount_ = spawnCount;
    }

    public override void Act(Bot bot)
    {
        for (int i = 0; i < spawnCount_; ++i)
        {
            GraphNavigator navigator = bot.graphWalker_.currentNode.GetComponent<GraphNavigator>();
            GameObject target = navigator.FindRandomEnemyTeamNode(bot.team);
            bot.TrySpawnMinion(target);
        }
    }
}

public class BotActionMove: BotAction {

    public override void Act(Bot bot)
    {
        GraphNavigator navigator = bot.graphWalker_.currentNode.GetComponent<GraphNavigator>();
        GameObject target = navigator.FindRandomTeamNode(bot.team);
        bot.CmdTryGoTo(target);
    }
}
