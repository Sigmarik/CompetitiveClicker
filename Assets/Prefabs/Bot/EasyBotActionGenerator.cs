using UnityEngine;

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
