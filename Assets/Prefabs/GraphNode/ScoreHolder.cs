using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Mirror;

public class ScoreHolder: NetworkBehaviour {

    void Start() {
        scoreData = new()
        {
            onOwnershipChange = onOwnershipChange
        };
    }

    void Update() {
        // Cursed, but design of scores are awfull and unmanagable from network standpoint

        if (isServer) {
            accrueGoldIfNeed(Time.deltaTime);
            team  = scoreData.team;
            score = scoreData.score;
        }

        if (isClient) {
            scoreData.team = team;
            scoreData.score = score;
        }
    }

    [Server]
    private void accrueGoldIfNeed (float dt) {
        passedTime_ += dt;

        if (passedTime_ >= tickLenth) {
            passedTime_ = 0;
            accrueGold(oneTickAmount);
        }
    }

    [Server]
    public void accrueGold ( uint amount) {
        if (scoreData.Teamed()) {
            bank.AddIncome(scoreData.team, amount);
        }
    }

    [Server]
    public void onOwnershipChange() {
        // TODO
    }

    [SyncVar]
    public Team team = Team.Default;
    
    [SyncVar]
    public uint score = 0;


    //---Data---
    public Score scoreData;
    public ResourceBank bank;

    public uint oneTickAmount = 1;

    public uint tickLenth = 1;          //time to accrue gold to player

    [SerializeField] private float passedTime_ = 0;
}
