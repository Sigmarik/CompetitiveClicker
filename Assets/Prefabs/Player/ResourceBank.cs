using UnityEngine;
using System.Collections.Generic;
using Mirror;

public class ResourceBank : NetworkBehaviour
{
    ResourceBank() {
        player_bank_ = new SyncDictionary<Team, double>
        {
            [Team.Bandits] = 0,
            [Team.Knights] = 0,
            [Team.Skeletons] = 0,
            [Team.Wizards] = 0,
            [Team.Default] = 0
        };

        player_income_ = new SyncDictionary<Team, double>
        {
            [Team.Bandits] = 0,
            [Team.Knights] = 0,
            [Team.Skeletons] = 0,
            [Team.Wizards] = 0,
            [Team.Default] = 0
        };
    }

    public override void OnStartServer()
    {
        InvokeRepeating(nameof(UpdateMoney), 1, 1);
    }

    [Server]
    private void UpdateMoney()
    {
        player_bank_[Team.Bandits  ] += player_income_[Team.Bandits  ];
        player_bank_[Team.Knights  ] += player_income_[Team.Knights  ];
        player_bank_[Team.Skeletons] += player_income_[Team.Skeletons];
        player_bank_[Team.Wizards  ] += player_income_[Team.Wizards  ];
        player_bank_[Team.Default  ] += player_income_[Team.Default  ];
    }

    public uint GetMoney(Team team)
    {
        return (uint)player_bank_[team];
    }

    [Server]
    public void AddIncome(Team team, double income)
    {
        player_bank_[team] += income;
    }

    [Server]
    public void SetMoney(Team team, uint money)
    {
        player_bank_[team] = money;
    }

    [Server]
    public void SpendMoney(Team team, uint money)
    {
        player_bank_[team] -= money;
    }

    private readonly SyncDictionary<Team, double> player_bank_;
    private readonly SyncDictionary<Team, double> player_income_;
}