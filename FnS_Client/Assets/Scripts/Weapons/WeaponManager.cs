using RiptideNetworking;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    private static WeaponManager _singleton;

    public static WeaponManager Singleton
    {
        get => _singleton;

        private set
        {
            if(_singleton == null)
                _singleton = value;
            else if(_singleton != value)
            {
                Debug.Log($"{nameof(WeaponManager)} instance already exists");
                Destroy(value);
            }
        }
    }

    private bool switchSendCooldown = false;

    [SerializeField]
    private GameObject[] allPossWeapons;

    [SerializeField]
    public static Dictionary<ushort, GameObject> allActiveWeapons = new Dictionary<ushort, GameObject>();

    public Transform GetWeapon(ushort weaponId) => allActiveWeapons[weaponId].transform;

    private void Awake()
    {
        Singleton = this;
    }

    private void FixedUpdate()
    {
        SendInput();
    }

    private void SendInput()
    {
        bool shooting = InputManager.Singleton.Shooting();

        Message message = Message.Create(MessageSendMode.unreliable, ClientToServerId.playerShoot);

        message.AddBool(shooting);

        NetworkManager.Singleton.Client.Send(message);

        bool pickup = InputManager.Singleton.Pickup();

        message = Message.Create(MessageSendMode.unreliable, ClientToServerId.playerAttemptPickUp);

        message.AddBool(pickup);

        NetworkManager.Singleton.Client.Send(message);
    }

    public bool SendSwitch()
    {
        if(switchSendCooldown) return false;

        Message message = Message.Create(MessageSendMode.reliable, ClientToServerId.switchWeaponSlots);

        message.AddUShort(InputManager.Singleton.GetActiveSlot());

        NetworkManager.Singleton.Client.Send(message);

        return true;
    }

    [MessageHandler((ushort)ServerToClientId.weaponSpawn)]
    private static void SpawnWeapon(Message message)
    {
        GameObject spawnedWeapon = Instantiate(WeaponManager.Singleton.allPossWeapons[message.GetUShort()], message.GetVector3(), Quaternion.identity);

        allActiveWeapons.Add(message.GetUShort(), spawnedWeapon);

        print("Spawned Weapon");
    }
}
