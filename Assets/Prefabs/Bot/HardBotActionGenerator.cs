using UnityEngine;

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
            5,
            10,
        };

        spawnCountProbabilities = new float[]
        {
            0.5f,
            0.5f
        };
    }
}
