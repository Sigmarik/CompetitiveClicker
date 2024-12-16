using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Mirror;

public class ScoreHolder : NetworkBehaviour {

    void Start() {
    
        var bank = NetworkClient.localPlayer.gameObject.GetComponent<ResourceBank>();

        logic = new NodeLogic(bank);
    }

    void Update() {
        
        accrueGoldIfNeed(Time.deltaTime);
    }

    private void accrueGoldIfNeed (float dt) {

        passedTime_ += dt;

        if (passedTime_ >= tickLenth) {

            passedTime_ = 0;

            logic.accrueGold(type, oneTickAmount);
        }
    }

    //---Data---

    public NodeLogic logic;

    public ResourceType type = ResourceType.Copper;
    public uint oneTickAmount = 1;

    public uint tickLenth = 1;          //time to accrue gold to player

    [SerializeField] private float passedTime_ = 0;
}
