using System.Collections;
using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;

public class GoldCountRequester : NetworkBehaviour
{
    // Start is called before the first frame update
    public override void OnStartClient()
    {
        text_ = GetComponent<TextMeshProUGUI>();
        bank_ = FindObjectOfType<ResourceBank>();
    }

    uint FindGoldCount()
    {
        // Ugly fix
        // Probably because player is spawned before initialization
        // WHICH WE CAN'T REMOVE BECAUSE OF movement sync
        // . . .
        // Suffer
        if (team_ == Team.Default && NetworkClient.localPlayer != null) {
            team_ = NetworkClient.localPlayer.gameObject.GetComponent<Player>().team;
        }

        if (bank_) return bank_.GetMoney(team_);
        return 0;
    }

    [Client]
    void Update()
    {
        if (!text_) return;
        text_.text = FindGoldCount().ToString();
    }

    private ResourceBank bank_;
    [SerializeField] private Team team_ = Team.Default;
    private TextMeshProUGUI text_;
}
