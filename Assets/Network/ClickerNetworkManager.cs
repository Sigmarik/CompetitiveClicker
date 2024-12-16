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
        // playerPrefab is the one assigned in the inspector in Network
        // Manager but you can use different prefabs per race for example
        GameObject gameobject = Instantiate(playerPrefab);

        // Apply data from the message however appropriate for your game
        // Typically Player would be a component you write with syncvars or properties
        Player player = gameobject.GetComponent<Player>();

        // Currently anyone starts at cude
        var playerPos = GameObject.FindGameObjectsWithTag("PlayerSpawn")[playerCount];

        // call this to use this gameobject as the primary controller
        NetworkServer.AddPlayerForConnection(conn, gameobject);
        player.Init(playerPos, (Team) (playerCount + 1));

        playerCount += 1;
    }
}
