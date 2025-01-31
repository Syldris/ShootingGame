using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
[SerializeField]
public class GameManager : MonoSingleton<GameManager>
{
    [SerializeField] private float maxSpawnTime;
    [SerializeField] private float curSpawnTime;
    [SerializeField] private float bossStagePoint;
    public float BossStagePoint{ get{ return bossStagePoint; }}

    public static int gameScore;

    [SerializeField] private bool isGameOver;
    public static bool isGameClear;
    [SerializeField] private bool isBossPlay;

    [SerializeField] private GameObject player;
    [SerializeField] private GameObject gameOverSet;
    [SerializeField] private GameObject gameStart;
    public GameObject gameClear;
    [SerializeField] private GameObject gameInfo;
    [SerializeField] private Slider gameProgressbar;

    [SerializeField] private Image[] lifeImages;
    [SerializeField] private Image[] boomImages;
    [SerializeField] private Text gameScoreText;

    [SerializeField] private ObjectManager objectManager;
    public Laser laserCode;

    public bool Boomstate;

    public SpriteRenderer bgimg1;
    public SpriteRenderer bgimg2;

    private void Start()
    {
        bossStagePoint = 10000f;

        gameProgressbar.value = gameScore;
    }

    private void Update()
    {
        GameScoreShow();
        if (!isBossPlay)
        {
            EnemySpawn();
            ReSpawn();
        }

        gameProgressbar.value = gameScore / bossStagePoint;
        if (isGameClear && gameClear.activeSelf == false) Invoke("GameClear",10f);

        ShowGameInfo();
    }

    public void colorRed()
    {
        BackGround.speed = 5.0f;
        bgimg1.color = new Color32(255, 64, 64, 255);
        bgimg2.color = new Color32(255, 64, 64, 255);
    }

    public void colorWhite()
    {
        BackGround.speed = 1.0f;
        bgimg1.color = new Color32(0, 0, 0, 255);
        bgimg2.color = new Color32(0, 0, 0, 255);
    }

    void EnemySpawn()
    {
        if (isGameOver)
            return;
        if (!player.activeSelf)
            return;
        if (curSpawnTime < maxSpawnTime)
            return;

        if(gameScore <= bossStagePoint)
        {
            maxSpawnTime = Random.Range(1.2f, 2.0f);

            int ran = Random.Range(0, 10);
            int index;

            if (ran < 7)
                index = 2;
            else if (ran < 9)
                index = 1;
            else 
                index = 0;

            Vector3 ops = new Vector3(Random.Range(-4f, 4f), 5, 0);

            Instantiate(objectManager.enemyObj[index], ops, transform.rotation);

            curSpawnTime = 0;
        }
        else
        {
            isBossPlay = true;
            EnemyDestroy();
            Invoke("EnemyBossSpawn", 2f);
        }
    }

    void EnemyDestroy()
    {
        GameObject[] enemyDes = GameObject.FindGameObjectsWithTag("Enemy");
        for (int i = 0; i < enemyDes.Length; i++)
            Destroy(enemyDes[i]);

        GameObject[] enemyBulletDes = GameObject.FindGameObjectsWithTag("EnemyBullet");
        for (int i = 0; i < enemyBulletDes.Length; i++)
            Destroy(enemyBulletDes[i]);
    }

    void ReSpawn()
    {
        curSpawnTime += Time.deltaTime;
    }

    public static void GameScoreUp(int score)
    {
        gameScore += score;
    }

    public void PlayerLifeSet(int life)
    {
        for (int i = 0; i < 3; i++)
        {
            lifeImages[i].color = new Color(1, 1, 1, 0);
        }

        for (int i = 0; i < life; i++)
        {
            lifeImages[i].color = new Color(1, 1, 1, 1);
        }
    }
    public void PlayerBoomSet(int boom)
    {
        for (int i = 0; i < 3; i++)
            boomImages[i].color = new Color(1, 1, 1, 0);

        for (int i = 0; i < boom; i++)
            boomImages[i].color = new Color(1, 1, 1, 1);
    }

    void EnemyBossSpawn()
    {
        Vector3 ops = new Vector3(0, 4.5f, 0);
        Instantiate(objectManager.enemyObj[3], ops, transform.rotation);
    }

    void GameSetting(string type)
    {
        Player.life = 3;
        Player.power = 1;
        Player.isPlayerDead = false;
        isGameOver = false;
        isGameClear = false;
        if (type == "start") gameScore = 0;
        gameStart.SetActive(false);
        PlayerLifeSet(3);
        if (gameOverSet.activeSelf == true) gameOverSet.SetActive(false);

        player.transform.position = new Vector3(0, -4, 0);
        player.SetActive(true);
    }

    public void GameStart()
    {
        gameStart.SetActive(false);
        GameSetting("start");
    }

    public void GameQuit()
    {
        Application.Quit();
    }

    public void GameOver()
    {
        isGameOver = true;
        gameOverSet.SetActive(true);
    }

    public void GameContinue()
    {
        GameSetting("continue");
    }

    public void GameReStart()
    {
        SceneManager.LoadScene(0);
        if(isGameClear)
        {
            isGameClear = false;
            gameClear.SetActive(false);
            Time.timeScale = 1;
        }
    }
    public void GameClear()
    {
        gameClear.SetActive(true);
        Time.timeScale = 0;
    }

    public void GameScoreShow()
    {
        if(!isGameClear)
        {
            gameScoreText.text = string.Format("{0:n0}", gameScore);
        }
        else
        {
            gameScoreText.transform.position = new Vector2(960f, 650f);
            gameScoreText.transform.localScale = new Vector2(2f, 2f);
            gameScoreText.text = string.Format("{0:n0}", gameScore);
        }
    }

    public void ShowGameInfo()
    {
        if(Input.GetButtonDown("Cancel"))
        {
            if(gameInfo.activeSelf)
            {
                Time.timeScale = 1f;
                gameInfo.SetActive(false);
            }
            else
            {
                Time.timeScale = 0;
                gameInfo.SetActive(true);
            }
        }
    }
}
