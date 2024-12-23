using UnityEngine;

public enum BotActionType {

    AFK,
    SpawnLine,
    SpawnHorde,
    Move,
}

public abstract class BotActionGenerator : MonoBehaviour
{
    private Bot bot_;

    public BotActionType[] actionTypes;
    public float[]         actionTypeProbabilities;

    public int[]   spawnCounts;
    public float[] spawnCountProbabilities;

    //--------------------------------------------------

    public virtual void Init() {

        bot_ = GetComponent<Bot>();
    }

    public BotAction GetRandomAction()
    {
        BotActionType type = GetRandomActionType();
        if (type == BotActionType.AFK)  return new BotActionAFK();
        if (type == BotActionType.Move) return new BotActionMove();

        int spawnCount = GetRandomSpawnCount();
        if (type == BotActionType.SpawnHorde) return new BotActionSpawnHorde(spawnCount);

        GraphNavigator navigator = bot_.graphWalker_.currentNode.GetComponent<GraphNavigator>();
        GameObject target = navigator.FindRandomEnemyTeamNode(bot_.team);
        if (type == BotActionType.SpawnLine) return new BotActionSpawnLine(target, spawnCount);

        //--------------------------------------------------

        return new BotActionAFK();
    }

    BotActionType GetRandomActionType()
    {
        return GetRandomElement<BotActionType>(actionTypes, actionTypeProbabilities);
    }

    int GetRandomSpawnCount()
    {
        return GetRandomElement<int>(spawnCounts, spawnCountProbabilities);
    }

    public T GetRandomElement<T>(T[] elements, float[] probabilities)
    {
        float randomValue = UnityEngine.Random.value;
        float cumulativeProbability = 0f;

        for (int i = 0; i < probabilities.Length; i++)
        {
            cumulativeProbability += probabilities[i];
            if (randomValue < cumulativeProbability) return elements[i];
        }

        // invalid propabilities
        throw new System.InvalidOperationException("The probabilities array must sum to 1.");
    }
}

public class EasyBotActionGenerator : BotActionGenerator {

    public override void Init()
    {
        base.Init();
        actionTypes = new BotActionType[]
        {
            BotActionType.AFK,
            BotActionType.SpawnLine,
            BotActionType.SpawnHorde,
            BotActionType.Move,
        };

        actionTypeProbabilities = new float[]
        {
            0.25f,
            0.25f,
            0.0f,
            0.5f,
        };

        spawnCounts = new int[]
        {
            1,
            2,
        };

        spawnCountProbabilities = new float[]
        {
            0.5f,
            0.5f,
        };
    }
}

public class HardBotActionGenerator : BotActionGenerator {

    public override void Init()
    {
        base.Init();
        actionTypes = new BotActionType[]
        {
            BotActionType.SpawnLine,
            BotActionType.SpawnHorde,
            BotActionType.Move
        };

        actionTypeProbabilities = new float[]
        {
            0.5f,
            0.25f,
            0.25f,
        };

        spawnCounts = new int[]
        {
            2,
            5,
        };

        spawnCountProbabilities = new float[]
        {
            0.5f,
            0.5f
        };
    }
}
