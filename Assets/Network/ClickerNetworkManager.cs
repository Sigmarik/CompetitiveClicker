using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public struct CreateCharacterMessage : NetworkMessage {}

public class ClickerNetworkManager : NetworkManager
{
    private int playerCount = 0;

    public override void OnStartServer()
    {
        base.OnStartServer();

        NetworkServer.RegisterHandler<CreateCharacterMessage>(OnCreateCharacter);
    }

    public override void OnClientConnect()
    {
        base.OnClientConnect();

        CreateCharacterMessage characterMessage = new();
        NetworkClient.Send(characterMessage);
    }

    void OnCreateCharacter(NetworkConnectionToClient conn, CreateCharacterMessage message)
    {
        if (playerCount >= Score.overallTeams.Length)
            return;
        Team current_team = Score.overallTeams[playerCount];


        // playerPrefab is the one assigned in the inspector in Network
        // Manager but you can use different prefabs per race for example
        GameObject player_object = null;
        switch (current_team)
        {
            case Team.Bandits:   player_object = Instantiate(BanditPlayerPrefab);   break;
            case Team.Knights:   player_object = Instantiate(KnightPlayerPrefab);   break;
            case Team.Skeletons: player_object = Instantiate(SkeletonPlayerPrefab); break;
            case Team.Wizards:   player_object = Instantiate(WizardPlayerPrefab);   break;
        }

        // Apply data from the message however appropriate for your game
        // Typically Player would be a component you write with syncvars or properties
        Player player = player_object.GetComponent<Player>();

        // Currently anyone starts at cude
        var playerPos = GameObject.FindGameObjectsWithTag("PlayerSpawn")[playerCount];
        if (playerPos.TryGetComponent<ScoreHolder>(out ScoreHolder score)) {
            score.scoreData.team = current_team;
            score.scoreData.score = 100; // Start bonus
        }

        // call this to use this gameobject as the primary controller
        NetworkServer.AddPlayerForConnection(conn, player_object);
        player.Init(playerPos);

        playerCount += 1;
    }

    void OnCreateBot()
    {
        if (playerCount >= Score.overallTeams.Length)
            return;
        Team current_team = Score.overallTeams[playerCount];

        //--------------------------------------------------

        GameObject botObject = null;
        switch (current_team)
        {
            case Team.Bandits:   botObject = Instantiate(BanditBotPrefab);   break;
            case Team.Knights:   botObject = Instantiate(KnightBotPrefab);   break;
            case Team.Skeletons: botObject = Instantiate(SkeletonBotPrefab); break;
            case Team.Wizards:   botObject = Instantiate(WizardBotPrefab);   break;
        }

        Bot bot = botObject.GetComponent<Bot>();

        var playerPos = GameObject.FindGameObjectsWithTag("PlayerSpawn")[playerCount];
        if (playerPos.TryGetComponent<ScoreHolder>(out ScoreHolder score)) {
            score.scoreData.team = current_team;
            score.scoreData.score = 100; // Start bonus
        }

        bot.Init(playerPos);
    }

    public GameObject BanditPlayerPrefab;
    public GameObject KnightPlayerPrefab;
    public GameObject SkeletonPlayerPrefab;
    public GameObject WizardPlayerPrefab;

    public GameObject BanditBotPrefab;
    public GameObject KnightBotPrefab;
    public GameObject SkeletonBotPrefab;
    public GameObject WizardBotPrefab;
}
