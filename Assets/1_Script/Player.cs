using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[SerializeField]
public class Player : MonoBehaviour
{
    public static int speed;           // 플레이어 이동 속도
    public static int power;    // 총알 업그레이드 1,2,3
    public static int life;     // 플레이어 생명 갯수

    public bool isTouchTop;     // 위쪽 화면 외곽 콜라이더 설정
    public bool isTouchBottom;  // 아래쪽
    public bool isTouchRight;   // 오른쪽
    public bool isTouchLeft;    // 왼쪽

    public float bulletSpeed;     // 총알 이동 속도
    public int bulletType;        // 1: 직사형,  2: 방사형
    public int boomCount;  // 폭탄 갯 수

    public float maxBulletShootTime;    // 총알 발사 시간 설정
    public float curBulletShootTime;    // 현재 총알 발사 시간 누적

    public float maxPowerPoint;         // 총알 업그래이드 되는 값 설정
    public float curPowerPoint;         // 플레이어 Power를 Up 하기위한 
                                        // 점수 2000이상이면 Power +1

    public static bool isPlayerDead;    // 플레이어 죽음 설정
    public bool isBoom;                 // 폭탄 사용 설정
    public bool isShield;               // 쉴드 사용 설정

    public bool isPetL;                 // 왼쪽   펫 사용 확인
    public bool isPetR;                 // 오른쪽 펫 사용 확인

    public GameObject petLObj;          // 왼쪽   펫 오브젝트
    public GameObject petRObj;          // 오른쪽 펫 오브젝트

    public GameObject shield;           // 쉴드 오프젝트
    public GameObject laser;            // 레이저 오브젝트 가져오기
    public Laser laserCode;             // 레이지 스크립트 가져오기

    public Image powerGauge;            // Power 게이지 

    public GameManager gameManager;     // 게임 메니저 가져오기
    public ObjectManager objectManager; // 오브젝트 메니저 가져오기
    public BackGroundSound backGroundSoundCode; // 백그라운드 싸운드 가져오기

    public int PlayerBulletA_int;
    public int PlayerBulletB_int;
    public GameObject PlayerBulletA;
    public GameObject PlayerBulletB;
    public GameObject Boom;

    void Start()
    {
        life = 3;
        power = 1;
        speed = 3;
        isPlayerDead = false;
        maxPowerPoint = 2000f;
    }

    void Update()
    {
        PlayerMove();       
        BulletShoot();      
        Reload();           
        HotKey();
        PowerCoolTime();
    }

    void HotKey()
    {
        if (Input.GetKeyDown(KeyCode.F1)) bulletType = 1;
        else if (Input.GetKeyDown(KeyCode.F2)) bulletType = 2;
        else if (Input.GetKeyDown(KeyCode.F3)) LaserFull();     // 레이저 풀 충전
        else if (Input.GetKeyDown(KeyCode.F5)) power = 1;       // 총알 power 1
        else if (Input.GetKeyDown(KeyCode.F6)) power = 2;       // 총알 power 2
        else if (Input.GetKeyDown(KeyCode.F7)) power = 3;       // 총알 power 3
        else if (Input.GetKeyDown(KeyCode.F9)) ShieldShow();    // 쉴드 사용
        else if (Input.GetKeyDown(KeyCode.F10)) backGroundSoundCode.BGSoundOnOff(); // 배경음악 On / Off
        else if (Input.GetKeyDown(KeyCode.F12)) GameManager.GameScoreUp(1000000); // 보스 생성을 위해 점수 설정
        else if (Input.GetKeyDown(KeyCode.L)) { laserCode.isLaserShoot = true; laserCode.LaserShoot(); } // 레이저 발사
        else if (Input.GetKeyDown(KeyCode.F)) ShieldHotKey();   // 쉴드 사용        
        else if (Input.GetKeyDown(KeyCode.P)) PetCreate();      // 펫 사용
        else if (Input.GetKeyDown(KeyCode.B)) if (boomCount > 0 && GameManager.Instance.Boomstate == false) BoomShow();    // 폭탄 사용
        //else if (Input.GetKeyDown(KeyCode.Escape)) gameManager.ShowGameInfo();  // 게임 정보 표시
    }


    void PlayerMove()
    {
        float h = Input.GetAxisRaw("Horizontal");
        if ((isTouchRight && h == 1) || (isTouchLeft && h == -1))
            h = 0;

        float v = Input.GetAxisRaw("Vertical");
        if ((isTouchTop && v == 1) || (isTouchBottom && v == -1))
            v = 0;

        Vector3 curPos = transform.position;

        Vector3 nextPos = new Vector3(h, v, 0) * speed * Time.deltaTime;

        transform.position = curPos + nextPos;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "BorderPlayer")
        {
            switch (collision.gameObject.name)  
            {
                case "Top":                
                    isTouchTop = true;         
                    break;

                case "Bottom":             
                    isTouchBottom = true;    
                    break;

                case "Right":                
                    isTouchRight = true;      
                    break;

                case "Left":              
                    isTouchLeft = true;   
                    break;
            }
        }
        else if (isShield && collision.gameObject.CompareTag("Enemy") || isShield && collision.gameObject.CompareTag("EnemyBullet"))
        {
            Destroy(collision.gameObject);

            if (collision.CompareTag("Enemy")) ScoreUp(100);
            else ScoreUp(10);
        }
        else if (collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("EnemyBullet"))
        {
            curPowerPoint = 0;
            isPlayerDead = true;

            gameObject.SetActive(false);

            objectManager.deadPlayerSound.Play();

            if (petLObj.gameObject.activeSelf == true)
                petLObj.gameObject.SetActive(false);
            if (petRObj.gameObject.activeSelf == true)
                petRObj.gameObject.SetActive(false);

            GameObject eff = Instantiate(objectManager.deadPlayerEffect, transform.position, transform.rotation);
            Destroy(eff, 1.5f);

            if (power > 1)
            {
                power--;
            }

            BulletDestroy();

            life--;
            gameManager.PlayerLifeSet(life);

            if (life <= 0) gameManager.GameOver();
            else Invoke("ReloadPlayer", 2f);

        }
        else if (isShield && (collision.gameObject.CompareTag("EnemyBullet")))
        {
            Destroy(collision.gameObject);

            if (collision.gameObject.CompareTag("Enemy"))
                ScoreUp(100);
            else
                ScoreUp(10);
        }
        else if (collision.gameObject.CompareTag("ItemLife"))
        {
            life++;
            objectManager.itmeTouchSound[2].Play();
            Destroy(collision.gameObject);

            if (life > 3)
            {
                life = 3;
                ScoreUp(500);
            }
            else
            {
                gameManager.PlayerLifeSet(life);
            }
        }
        else if (collision.gameObject.CompareTag("ItemBoom"))
        {
            boomCount++;
            objectManager.itmeTouchSound[0].Play();
            Destroy(collision.gameObject);

            if (boomCount > 3)
            {
                boomCount = 3;
                ScoreUp(500);
            }
            else
            {
                gameManager.PlayerBoomSet(boomCount);
            }
        }
        else if (collision.gameObject.CompareTag("ItemCoin"))
        {
            objectManager.itmeTouchSound[1].Play();
            Destroy(collision.gameObject);
            ScoreUp(500);
        }
        else if (collision.gameObject.CompareTag("ItemShield"))
        {
            objectManager.itmeTouchSound[4].Play();
            Destroy(collision.gameObject);
            ShieldShow();
        }
        else if (collision.gameObject.tag == "ItemPet")
        {
            if (petLObj.gameObject.activeSelf == false)
                petLObj.gameObject.SetActive(true);
            else if (petRObj.gameObject.activeSelf == false)
                petRObj.gameObject.SetActive(true);
            else GameManager.GameScoreUp(500);

            objectManager.itmeTouchSound[3].Play();

            Destroy(collision.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("BorderPlayer"))
        {
            switch (collision.gameObject.name)
            {
                case "Top":
                    isTouchTop = false;
                    break;

                case "Bottom":
                    isTouchBottom = false;
                    break;

                case "Right":
                    isTouchRight = false;
                    break;

                case "Left":
                    isTouchLeft = false;
                    break;
            }
        }
        
    }

    void BulletShoot()
    {
        if (!Input.GetKey("space"))
            return;

        if (curBulletShootTime < maxBulletShootTime)
            return;

        if (bulletType == 1)
        {
            bulletSpeed = 1.0f + (0.5f * power);
            maxBulletShootTime = 0.6f - (0.1f * power);
        }
        else
        {
            bulletSpeed = 2 + power;
            maxBulletShootTime = 0.5f - (0.1f * power);
        }

        objectManager.bulletShootSound.Play();

        if (bulletType == 1 && power == 1) 
        {
            Player_B_Pooling();
            GameObject bullet_01 = Object_Pooling.Instance.PlayerBulletB_Objects[PlayerBulletB_int];
            ++PlayerBulletB_int; 
            
            Rigidbody2D rigid_01 = bullet_01.GetComponent<Rigidbody2D>();             
            rigid_01.AddForce(Vector3.up * bulletSpeed, ForceMode2D.Impulse);           

        }
        else if (bulletType == 2 && power == 1) 
        {
            Player_A_Pooling();
            GameObject bullet_01R = Object_Pooling.Instance.PlayerBulletA_Objects[PlayerBulletA_int];
            ++PlayerBulletA_int;

            Player_A_Pooling();
            GameObject bullet_01C = Object_Pooling.Instance.PlayerBulletA_Objects[PlayerBulletA_int];
            ++PlayerBulletA_int;

            Player_A_Pooling();
            GameObject bullet_01L = Object_Pooling.Instance.PlayerBulletA_Objects[PlayerBulletA_int];
            ++PlayerBulletA_int;

            Rigidbody2D rigid_01R = bullet_01R.GetComponent<Rigidbody2D>();
            Rigidbody2D rigid_01C = bullet_01C.GetComponent<Rigidbody2D>();
            Rigidbody2D rigid_01L = bullet_01L.GetComponent<Rigidbody2D>();

            rigid_01R.AddForce((Vector3.up + new Vector3(-0.2f, 0, 0)) * bulletSpeed, ForceMode2D.Impulse);
            rigid_01C.AddForce(Vector3.up * bulletSpeed, ForceMode2D.Impulse);
            rigid_01L.AddForce((Vector3.up + new Vector3(0.2f, 0, 0)) * bulletSpeed, ForceMode2D.Impulse);

            Vector3 rotVec_01R = Vector3.forward * 12f;
            Vector3 rotVec_01L = Vector3.forward * -12f;
            bullet_01R.transform.Rotate(rotVec_01R);
            bullet_01L.transform.Rotate(rotVec_01L);
        }
        else if (bulletType == 1 && power == 2)
        {
            Player_B_Pooling();
            GameObject bullet_02R = Object_Pooling.Instance.PlayerBulletB_Objects[PlayerBulletB_int];
            bullet_02R.transform.Translate(new Vector3(-0.2f, 0, 0));
            ++PlayerBulletB_int;

            Player_B_Pooling();
            GameObject bullet_02L = Object_Pooling.Instance.PlayerBulletB_Objects[PlayerBulletB_int];
            bullet_02L.transform.Translate(new Vector3(0.2f, 0, 0));
            ++PlayerBulletB_int;

            Rigidbody2D rigid_02R = bullet_02R.GetComponent<Rigidbody2D>();
            Rigidbody2D rigid_02L = bullet_02L.GetComponent<Rigidbody2D>();
            rigid_02R.AddForce(Vector3.up * bulletSpeed, ForceMode2D.Impulse);
            rigid_02L.AddForce(Vector3.up * bulletSpeed, ForceMode2D.Impulse);
        }
        else if (bulletType == 2 && power == 2)
        {
            Player_A_Pooling();
            GameObject bullet_02R = Object_Pooling.Instance.PlayerBulletA_Objects[PlayerBulletA_int];
            ++PlayerBulletA_int;

            Player_B_Pooling();
            GameObject bullet_02C = Object_Pooling.Instance.PlayerBulletB_Objects[PlayerBulletB_int];
            ++PlayerBulletB_int;

            Player_A_Pooling();
            GameObject bullet_02L = Object_Pooling.Instance.PlayerBulletA_Objects[PlayerBulletA_int];
            ++PlayerBulletA_int;

            Rigidbody2D rigid_02R = bullet_02R.GetComponent<Rigidbody2D>();
            Rigidbody2D rigid_02C = bullet_02C.GetComponent<Rigidbody2D>();
            Rigidbody2D rigid_02L = bullet_02L.GetComponent<Rigidbody2D>();
            rigid_02R.AddForce((Vector3.up + new Vector3(-0.2f, 0, 0)) * bulletSpeed, ForceMode2D.Impulse);
            rigid_02C.AddForce(Vector3.up * bulletSpeed, ForceMode2D.Impulse);
            rigid_02L.AddForce((Vector3.up + new Vector3(0.2f, 0, 0)) * bulletSpeed, ForceMode2D.Impulse);

            Vector3 rotVec_02R = Vector3.forward * 12f;
            Vector3 rotVec_02L = Vector3.forward * -12f;
            bullet_02R.transform.Rotate(rotVec_02R);
            bullet_02L.transform.Rotate(rotVec_02L);
        }
        else if (bulletType == 1 && power == 3)
        {
            Player_A_Pooling();
            GameObject bullet_03RR = Object_Pooling.Instance.PlayerBulletA_Objects[PlayerBulletA_int];
            bullet_03RR.transform.Translate(new Vector3(-0.5f, 0, 0));
            ++PlayerBulletA_int;

            Player_B_Pooling();
            GameObject bullet_03R = Object_Pooling.Instance.PlayerBulletB_Objects[PlayerBulletB_int];
            bullet_03R.transform.Translate(new Vector3(-0.2f, 0, 0));
            ++PlayerBulletB_int;

            Player_B_Pooling();
            GameObject bullet_03L = Object_Pooling.Instance.PlayerBulletB_Objects[PlayerBulletB_int];
            bullet_03L.transform.Translate(new Vector3(0.2f, 0, 0));
            ++PlayerBulletB_int;

            Player_A_Pooling();
            GameObject bullet_03LL = Object_Pooling.Instance.PlayerBulletA_Objects[PlayerBulletA_int];
            bullet_03LL.transform.Translate(new Vector3(0.5f, 0, 0));
            ++PlayerBulletA_int;

            Rigidbody2D rigid_03RR = bullet_03RR.GetComponent<Rigidbody2D>();
            Rigidbody2D rigid_03R = bullet_03R.GetComponent<Rigidbody2D>();
            Rigidbody2D rigid_03L = bullet_03L.GetComponent<Rigidbody2D>();
            Rigidbody2D rigid_03LL = bullet_03LL.GetComponent<Rigidbody2D>();
            rigid_03RR.AddForce(Vector3.up * bulletSpeed, ForceMode2D.Impulse);
            rigid_03R.AddForce(Vector3.up * bulletSpeed, ForceMode2D.Impulse);
            rigid_03L.AddForce(Vector3.up * bulletSpeed, ForceMode2D.Impulse);
            rigid_03LL.AddForce(Vector3.up * bulletSpeed, ForceMode2D.Impulse);
        }
        else if (bulletType == 2 && power == 3)
        {

            Player_A_Pooling();
            GameObject bullet_03R = Object_Pooling.Instance.PlayerBulletA_Objects[PlayerBulletA_int];
            ++PlayerBulletA_int;

            Player_B_Pooling();
            GameObject bullet_03CC = Object_Pooling.Instance.PlayerBulletB_Objects[PlayerBulletB_int];
            bullet_03CC.transform.Translate(new Vector3(-0.2f, 0, 0));
            ++PlayerBulletB_int;

            Player_B_Pooling();
            GameObject bullet_03C = Object_Pooling.Instance.PlayerBulletB_Objects[PlayerBulletB_int];
            bullet_03C.transform.Translate(new Vector3(0.2f, 0, 0));
            ++PlayerBulletB_int;

            Player_A_Pooling();
            GameObject bullet_03L = Object_Pooling.Instance.PlayerBulletA_Objects[PlayerBulletA_int];
            ++PlayerBulletA_int;

            Rigidbody2D rigid_03R = bullet_03R.GetComponent<Rigidbody2D>();
            Rigidbody2D rigid_03CC = bullet_03CC.GetComponent<Rigidbody2D>();
            Rigidbody2D rigid_03C = bullet_03C.GetComponent<Rigidbody2D>();
            Rigidbody2D rigid_03L = bullet_03L.GetComponent<Rigidbody2D>();
            rigid_03R.AddForce((Vector3.up + new Vector3(-0.2f, 0, 0)) * bulletSpeed, ForceMode2D.Impulse);
            rigid_03CC.AddForce(Vector3.up * bulletSpeed, ForceMode2D.Impulse);
            rigid_03C.AddForce(Vector3.up * bulletSpeed, ForceMode2D.Impulse);
            rigid_03L.AddForce((Vector3.up + new Vector3(0.2f, 0, 0)) * bulletSpeed, ForceMode2D.Impulse);

            Vector3 rotVec_03R = Vector3.forward * 12f;
            Vector3 rotVec_03L = Vector3.forward * -12f;
            bullet_03R.transform.Rotate(rotVec_03R);
            bullet_03L.transform.Rotate(rotVec_03L);
        }

        curBulletShootTime = 0;
    }

    void Reload()
    {
        curBulletShootTime += Time.deltaTime;
    }

    void ReloadPlayer()
    {
        transform.position = new Vector3(0, -4, 0);
        gameObject.SetActive(true);
        isPlayerDead = false;
    }

    void BulletDestroy()
    {

        if (GameManager.gameScore < gameManager.BossStagePoint)
        {
            GameObject[] enemyDes = GameObject.FindGameObjectsWithTag("Enemy");
            for(int i=0; i<enemyDes.Length; i++)
            {
                Destroy(enemyDes[i]);
            }
        }

        GameObject[] enemyBulletDes = GameObject.FindGameObjectsWithTag("EnemyBullet");
        for (int i = 0; i < enemyBulletDes.Length; i++)
        {
            Destroy(enemyBulletDes[i]);
        }
    }

    void ScoreUp(int score)
    {
        GameManager.gameScore += score;
        PowerUpPoint(score);
    }

    public void PowerUpPoint(int enemyScore)
    {
        if(power < 3)
        {
            curPowerPoint += (float)enemyScore;
            if(curPowerPoint > 2000 && power == 1)
            {
                power++;
                curPowerPoint = 0;
                maxPowerPoint = 5000;
                powerGauge.fillAmount = 0;
            }
            else if(curPowerPoint > 5000 && power == 2)
            {
                power++;
                curPowerPoint = 0;
                powerGauge.fillAmount = 0;
            }
        }
    }

    public void Player_A_Pooling()
    {
        if(PlayerBulletA_int >= Object_Pooling.Instance.PlayerBulletA_Objects.Length)
        {
            PlayerBulletA_int = 0;                   
        }

        Object_Pooling.Instance.PlayerBulletA_Objects[PlayerBulletA_int].SetActive(true);
        Object_Pooling.Instance.PlayerBulletA_Objects[PlayerBulletA_int].GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
        Object_Pooling.Instance.PlayerBulletA_Objects[PlayerBulletA_int].transform.position = new Vector3(transform.position.x, transform.position.y + 0.7f, transform.position.z);
    }

    public void Player_B_Pooling()
    {
        if (PlayerBulletB_int >= Object_Pooling.Instance.PlayerBulletB_Objects.Length)
        {
            PlayerBulletB_int = 0;
        }

        Object_Pooling.Instance.PlayerBulletB_Objects[PlayerBulletB_int].SetActive(true);
        Object_Pooling.Instance.PlayerBulletB_Objects[PlayerBulletB_int].GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
        Object_Pooling.Instance.PlayerBulletB_Objects[PlayerBulletB_int].transform.position = new Vector3(transform.position.x, transform.position.y + 0.7f, transform.position.z);
    }

    public void BoomShow()
    {
        boomCount--;
        gameManager.PlayerBoomSet(boomCount);
        Instantiate(Boom);
    }

    public void ShieldHotKey()
    {
        if(isShield == false)
        {
            isShield = true;
            shield.SetActive(true);
        }
        else
        {
            isShield = false;
            shield.SetActive(false);
        }
    }

    void ShieldShow()
    {
        shield.SetActive(true);
        isShield = true;

        if (isShield == true) Invoke("ShieldHide", 5f);
    }

    void ShieldHide()
    {
        isShield = false;
        shield.SetActive(false);
    }

    void PetCreate()
    {
        if (petLObj.gameObject.activeSelf == false)
            petLObj.gameObject.SetActive(true);
        else if (petRObj.gameObject.activeSelf == false)
            petRObj.gameObject.SetActive(true);

        objectManager.itmeTouchSound[3].Play();
    }

    void LaserFull()
    {
        Laser LaserCode = GameObject.Find("Laser").GetComponent<Laser>();
        LaserCode.maxLaserCoolTime = 0.1f;
    }

    void PowerCoolTime()
    {
        if(isPlayerDead == true)
        {
            curPowerPoint = 0;
            powerGauge.fillAmount = 1f;
        }

        powerGauge.fillAmount = curPowerPoint / maxPowerPoint;
    }
}
