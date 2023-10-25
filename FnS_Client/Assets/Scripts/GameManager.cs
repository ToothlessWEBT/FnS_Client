using RiptideNetworking;
using UnityEngine.SceneManagement;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _singleton;

    public static GameManager Singleton
    {
        get => _singleton;

        private set
        {
            if(_singleton == null)
                _singleton = value;
            else if(_singleton != value)
            {
                Debug.Log($"{nameof(GameManager)} instance already exists");
                Destroy(value);
            }
        }
    }

    [SerializeField]
    private GameObject hubSprite;

    [SerializeField]
    private GameObject GameOverScreen;

    private void Awake()
    {
        Singleton = this;
    }

    private static void EndGame()
    {
        _singleton.GameOverScreen.SetActive(true);

        EnemyManager.activeEnemies.Clear();
        BulletManager.activeBullets.Clear();
        WeaponManager.allActiveWeapons.Clear();
        Player.list.Clear();

        _singleton.Invoke(nameof(ResetGame), 7f);

    }

    private void ResetGame() =>SceneManager.LoadScene("SampleScene");

    [MessageHandler((ushort)ServerToClientId.startGame)]
    private static void StartGame(Message message)
    {
        _singleton.hubSprite.SetActive(false);
    }

    [MessageHandler((ushort)ServerToClientId.gameOver)]
    private static void GameOver(Message message)
    {
        EndGame();
    }
}
