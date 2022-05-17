using RiptideNetworking;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Dictionary<ushort, Player> list = new Dictionary<ushort, Player>();

    public ushort Id {get; private set;}

    public bool IsLocal;// {get; private set;}

    [SerializeField] private Transform camTransform;

    private string username;

    private void OnDestroy()
    {
        if(IsLocal)//Remove this and load new scene
        {
            CameraController.Singleton.SetCameraPos(Vector3.zero);
        }

        list.Remove(Id);
    }

    private void MovePlayer(Vector2 newPos)
    {
        transform.position = newPos;

        if(IsLocal)
        {
            CameraController.Singleton.SetCameraPos(camTransform.position);
        }
    }

    public static void Spawn(ushort id, string username, Vector3 position)
    {
        Player player;
        if(id == NetworkManager.Singleton.Client.Id)
        {
            player = Instantiate(GameLogic.Singleton.LocalPlayerPrefab, position, Quaternion.identity).GetComponent<Player>();
            player.IsLocal = true;
        }
        else
        {
            player = Instantiate(GameLogic.Singleton.PlayerPrefab, position, Quaternion.identity).GetComponent<Player>();
            player.IsLocal = false;
        }

        player.name = $"Player {id} ({(string.IsNullOrEmpty(username) ? "Guest" : username)})";
        player.Id = id;
        player.username = username;

        list.Add(id, player);
    }

    #region Messages

    [MessageHandler((ushort)ServerToClientId.playerSpawned)]
    private static void SpawnPlayer(Message message)
    {
        Spawn(message.GetUShort(), message.GetString(), message.GetVector2());
    }

    [MessageHandler((ushort)ServerToClientId.playerMovement)]
    private static void PlayerMovement(Message message)
    {
        if(list.TryGetValue(message.GetUShort(), out Player player))
        {
            player.MovePlayer(message.GetVector2());
        }
    }
    

    #endregion
    
}
