using UnityEngine;
using Mirror;

public class ClickableNode : NetworkBehaviour
{
    private Score scoreData_;

    //--------------------------------------------------

    void Start()
    {
        scoreData_ = GetComponent<ScoreHolder>().logic.scoreData;
    }

    void OnMouseDown()
    {
        if (GetMyTeam() == GetLocalPlayer().team_) PlayerComeToMe();
        else                                       SendMinionToMe();
    }

    void PlayerComeToMe()
    {
        GetLocalPlayer().TryGoTo(gameObject);
    }

    void SendMinionToMe()
    {
        GetLocalPlayer().TrySpawnMinion(gameObject);
    }

    //--------------------------------------------------

    Player GetLocalPlayer()
    {
        return NetworkClient.localPlayer.gameObject.GetComponent<Player>();
    }

    Team GetMyTeam()
    {
        return scoreData_.team;
    }
}
