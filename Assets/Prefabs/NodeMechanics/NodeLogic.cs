using System.Collections;
using System.Collections.Generic;
using Palmmedia.ReportGenerator.Core.Parser.Analysis;
using UnityEngine;

public class NodeLogic {

    public NodeLogic (ResourceBank bank) {

        scoreData = new Score();

        scoreData.onOwnershipChange = onOwnershipChange;

        bank_ = bank;
    }

    public NodeLogic (ResourceBank bank, Team team) {

        scoreData = new Score();

        scoreData.onOwnershipChange = onOwnershipChange;
        scoreData.team = team;

        bank_ = bank;
    }

    public void accrueGold (ResourceType type, uint amount) {

        if (scoreData.Teamed()) {

            // get player's score bank
            // accure gold to player

            bank_.Add(type, amount);
        }
    }

    private void onOwnershipChange () {

        // To Do
    }
    
    public Score scoreData;

    [SerializeField] private ResourceBank bank_; 
}
