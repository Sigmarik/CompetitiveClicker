using UnityEngine;
using System.Collections.Generic;

public class ResourceBank : MonoBehaviour
{
    void Start()
    {
        player_bank_ = new Dictionary<Team, double>
        {
            [Team.Bandits] = 0,
            [Team.Knights] = 0,
            [Team.Skeletons] = 0,
            [Team.Wizards] = 0,
            [Team.Default] = 0
        };

        player_income_ = new Dictionary<Team, double>
        {
            [Team.Bandits] = 0,
            [Team.Knights] = 0,
            [Team.Skeletons] = 0,
            [Team.Wizards] = 0,
            [Team.Default] = 0
        };

        InvokeRepeating(nameof(UpdateMoney), 1, 1);
    }

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

    public void AddIncome(Team team, double income)
    {
        player_income_[team] += income;
    }

    public void SetMoney(Team team, uint money)
    {
        player_bank_[team] = money;
    }

    public void SpendMoney(Team team, uint money)
    {
        player_bank_[team] -= money;
    }

    private Dictionary<Team, double> player_bank_;
    private Dictionary<Team, double> player_income_;
}