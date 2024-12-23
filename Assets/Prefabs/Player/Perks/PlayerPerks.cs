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

    public PlayerPerks () {

        perks_ = new HashSet<string>();
    }

    public void addPerk (Perk perk) {

        bool added = perks_.Add(perk.name);
        if (!added) {

            Debug.Log("Player already have perk you are trying to add: " + perk);
        }
    }

    public void removePerk (Perk perk) {

        bool removed = perks_.Remove(perk.name);
        if (!removed) {

            Debug.Log("Player doesn't have perk you are trying to remove: " + perk);
        }
    }

    public bool isHasPerk (Perk perk) {

        return perks_.Contains(perk.name);
    }

    //---Data---//

    [SerializeField] private HashSet<string> perks_;
}


public class PlayerPerksShop: PlayerPerks {

    public PlayerPerksShop (Team team): base() {

        this.bank = GameObject.FindFirstObjectByType<ResourceBank>();   //TODO
        this.team = team;
    }

    public void buyPerk (Perk perk) {

        if (perk.cost <= bank.GetMoney(team)) {

            if (!isHasPerk(perk)) {

                bank.SpendMoney(team, perk.cost);
                addPerk(perk);
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



