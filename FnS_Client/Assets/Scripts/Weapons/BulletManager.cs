using System.Collections;
using System.Collections.Generic;
using RiptideNetworking;
using UnityEngine;

public class BulletManager : MonoBehaviour
{
    private static BulletManager _singleton;

    public static BulletManager Singleton
    {
        get => _singleton;

        private set
        {
            if(_singleton == null)
                _singleton = value;
            else if(_singleton != value)
            {
                Debug.Log($"{nameof(BulletManager)} instance already exists");
                Destroy(value);
            }
        }
    }

    private void Awake()
    {
        Singleton = this;
    }

    public GameObject[] allBullet;

    public static Dictionary<ushort, GameObject> activeBullets = new Dictionary<ushort, GameObject>();

    private void RemoveBullet(ushort delId)
    {
        Destroy(activeBullets[delId]);
        activeBullets.Remove(delId);
    }

    [MessageHandler((ushort)ServerToClientId.bulletSpawn)]
    private static void SpawnBullet(Message message)
    {
        ushort iD = message.GetUShort();

        GameObject newBull = Instantiate(Singleton.allBullet[message.GetUShort()], message.GetVector2(), Quaternion.identity);

        activeBullets.Add(iD, newBull);
    }

    [MessageHandler((ushort)ServerToClientId.bulletMove)]
    private static void MoveBullet(Message message)
    {
        ushort id = message.GetUShort();
        if(activeBullets.ContainsKey(id))
        {
            activeBullets[id].transform.position = message.GetVector2();
        }
    }

    [MessageHandler((ushort)ServerToClientId.bulletKill)]
    private static void DestroyBullet(Message message)
    {
        ushort id = message.GetUShort();
        if(activeBullets.ContainsKey(id))
        {
            Singleton.RemoveBullet(id);
        }
    }

    
}
