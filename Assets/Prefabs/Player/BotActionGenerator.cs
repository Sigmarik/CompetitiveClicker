using UnityEngine;

public enum BotActionType {

    AFK,
    SpawnLine,
    SpawnHorde,
    Move,
}

public class BotActionGenerator : MonoBehaviour
{
    public Bot bot_;
    public BotActionType[] actionTypes =
    {
        BotActionType.AFK,
        BotActionType.SpawnLine,
        BotActionType.SpawnHorde,
        BotActionType.Move
    };

    public float[] actionTypeProbabilities =
    {
        0.25f,
        0.25f,
        0.0f,
        0.5f,
    };

    //--------------------------------------------------

    public int[] spawnCounts =
    {
        1,
        2,
        5,
    };

    public float[] spawnCountProbabilities =
    {
        0.5f,
        0.25f,
        0.25f
    };

    //--------------------------------------------------

    public void Init() {

        bot_ = GetComponent<Bot> ();
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

