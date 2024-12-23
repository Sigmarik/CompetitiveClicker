using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public struct Perk {

    public Perk (string name, uint cost) {

        this.name = name;
        this.cost = cost;
    }

    public string name;
    public uint cost;
}


public class PlayerPerks {

    public PlayerPerks () {}

    protected void addPerk (string perk) {

        bool added = perks_.Add(perk);
        if (!added) {

            Debug.Log("Player already have perk you are trying to add: " + perk);
        }
    }

    protected void removePerk (string perk) {

        bool removed = perks_.Remove(perk);
        if (!removed) {

            Debug.Log("Player doesn't have perk you are trying to remove: " + perk);
        }
    }

    protected bool isHasPerk (string perk) {

        return perks_.Contains(perk);
    }

    //---Data---//

    [SerializeField] private HashSet<string> perks_;
}


public class PlayerPerksShop: PlayerPerks {

    public PlayerPerksShop (ResourceBank bank, Team team): base() {

        this.bank = bank;
        this.team = team;
    }

    public void buyPerk (Perk perk) {

        if (perk.cost <= bank.GetMoney(team)) {

            if (!isHasPerk(perk.name)) {

                bank.SpendMoney(team, perk.cost);
                addPerk(perk.name);
            }
            else {

                Debug.Log("Player: " + team.ToString() + "trying to buy perk they already has: " + perk.name);
            }
        }
        else {

            Debug.Log("Player: " + team.ToString() + "hasn't got enouth money to buy perk: " + perk.name);
        }
    }

    //---Data---//

    public ResourceBank bank;
    public Team team;
}



