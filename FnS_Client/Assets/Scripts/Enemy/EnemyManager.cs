using RiptideNetworking;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    private static EnemyManager _singleton;

    public static EnemyManager Singleton
    {
        get => _singleton;

        private set
        {
            if(_singleton == null)
                _singleton = value;
            else if(_singleton != value)
            {
                Debug.Log($"{nameof(EnemyManager)} instance already exists");
                Destroy(value);
            }
        }
    }

    public static Dictionary<ushort, GameObject> activeEnemies = new Dictionary<ushort, GameObject>();

    [SerializeField]
    private GameObject[] allPossibleEnemies, allPossibleBosses;

    private void Awake()
    {
        Singleton = this;
    }

    private void RemoveEnemy(ushort delId)
    {
        Destroy(activeEnemies[delId]);
        activeEnemies.Remove(delId);
    }

    [MessageHandler((ushort)ServerToClientId.enemySpawned)]
    private static void SpawnEnemy(Message message)
    {
        //type pos index

        ushort bossOrN = message.GetUShort();

        GameObject newEnemy;

        if(bossOrN == 0)
            newEnemy = Instantiate(Singleton.allPossibleEnemies[message.GetUShort()], message.GetVector2(), Quaternion.identity);
        else    
            newEnemy = Instantiate(Singleton.allPossibleBosses[message.GetUShort()], message.GetVector2(), Quaternion.identity);

        ushort Id = message.GetUShort();

        activeEnemies.Add(Id, newEnemy);

    }

    [MessageHandler((ushort)ServerToClientId.enemyMove)]
    private static void MoveBullet(Message message)
    {
        ushort id = message.GetUShort();
        if(activeEnemies.ContainsKey(id))
        {
            activeEnemies[id].transform.position = message.GetVector2();
        }
    }

    [MessageHandler((ushort)ServerToClientId.enemyKill)]
    private static void DestroyEnemy(Message message)
    {
        ushort id = message.GetUShort();
        if(activeEnemies.ContainsKey(id)) Singleton.RemoveEnemy(id);
    }

    [MessageHandler((ushort)ServerToClientId.flipEnemy)]
    private static void FlipEnemy(Message message)
    {
        ushort id = message.GetUShort();
        if(activeEnemies.ContainsKey(id)) activeEnemies[id].transform.RotateAround(activeEnemies[id].transform.position, activeEnemies[id].transform.up, 180f);
    }
}