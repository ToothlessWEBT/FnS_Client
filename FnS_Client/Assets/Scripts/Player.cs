using RiptideNetworking;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Dictionary<ushort, Player> list = new Dictionary<ushort, Player>();

    public ushort Id {get; private set;}

    public bool IsLocal;// {get; private set;}

    [SerializeField] private Transform camTransform;

    public Sprite[] facing;

    private string username;

    [SerializeField] private SpriteRenderer sr;

    [SerializeField]
    private Transform GunHolderTrans;
    
    [SerializeField]
    private GunHolder gunHolder;

    [SerializeField] private Text text;

    private void OnDestroy()
    {
        if(IsLocal)//Remove this and load new scene
        {
            CameraController.Singleton.SetCameraPos(Vector3.zero);
        }

        list.Remove(Id);
    }

    private void FixedUpdate()
    {
        RotateToFaceCamera();
    }

    private void RotateToFaceCamera()
    {
        if(!IsLocal) return;

        Vector2 mouseOnScreen = CameraController.Singleton.GetCamera().ScreenToWorldPoint(Input.mousePosition);
         
        float angle = Mathf.Atan2(mouseOnScreen.y - transform.position.y, mouseOnScreen.x - transform.position.x) * Mathf.Rad2Deg;
 
        GunHolderTrans.rotation = Quaternion.Euler(new Vector3(0f,0f,angle));

        //Send Camera Rotation to Server

        Message message = Message.Create(MessageSendMode.unreliable, ClientToServerId.playerRotate);

        message.AddFloat(angle);

        NetworkManager.Singleton.Client.Send(message);
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

    private void SetHealth(int h)
    {
        if(text == null) text = UIManager.Singleton.GetText();
        text.text = h.ToString();
    }

    public GunHolder GetGunHolder() => gunHolder;

    public SpriteRenderer GetSpriteRenderer() => sr;

    #region Messages

    [MessageHandler((ushort)ServerToClientId.playerSpawned)]
    private static void SpawnPlayer(Message message)
    {
        Spawn(message.GetUShort(), message.GetString(), message.GetVector2());
    }

    [MessageHandler((ushort)ServerToClientId.killPlayer)]
    private static void KillPlayer(Message message)
    {
        if(list.TryGetValue(message.GetUShort(), out Player player))
        {
            player.GetSpriteRenderer().enabled = false;
            player.GetGunHolder().gameObject.SetActive(false);
        }
    }

    [MessageHandler((ushort)ServerToClientId.respawnPlayer)]
    private static void RespawnPlayer(Message message)
    {
        if(list.TryGetValue(message.GetUShort(), out Player player))
        {
            player.GetSpriteRenderer().enabled = true;
            player.GetGunHolder().gameObject.SetActive(true);
        }

    }

    [MessageHandler((ushort)ServerToClientId.playerFacingDir)]
    private static void FacePlayer(Message message)
    {
        if(list.TryGetValue(message.GetUShort(), out Player player))
        {
            player.GetSpriteRenderer().sprite = player.facing[message.GetUShort()];
        }

    }


    [MessageHandler((ushort)ServerToClientId.damagePlayer)]
    private static void DamagePlayer(Message message)
    {
        if(list.TryGetValue(message.GetUShort(), out Player player))
        {
            if(player.IsLocal)player.SetHealth(message.GetInt());
        }

    }

    [MessageHandler((ushort)ServerToClientId.playerMovement)]
    private static void PlayerMovement(Message message)
    {
        if(list.TryGetValue(message.GetUShort(), out Player player))
        {
            player.MovePlayer(message.GetVector2());
        }
    }

    [MessageHandler((ushort)ServerToClientId.weaponPickedUp)]
    private static void PickUpWeapon(Message message)
    {
        if(list.TryGetValue(message.GetUShort(), out Player player))
        {
            player.gunHolder.AddWeapon(message.GetUShort(), message.GetUShort());
        }
    }

    [MessageHandler((ushort)ServerToClientId.swichedSlots)]
    private static void SwitchSlots(Message message)
    {
        ushort id = message.GetUShort();
        ushort indeX = message.GetUShort();
        print(indeX);
        Player.list[id].gunHolder.SetActiveSlot(indeX);
    }
    

    #endregion
    
}
