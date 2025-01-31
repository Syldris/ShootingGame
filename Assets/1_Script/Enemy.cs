using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[System.Serializable]
public class Enemy : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private int hp;
    [SerializeField] private int enemyScore;
    [SerializeField] private string enemyName;

    [SerializeField] private float maxShootTime;
    [SerializeField] private float curShoootTime;

    [SerializeField] private Slider HPbar;
    [SerializeField] private Slider HPbar_Basic;

    [SerializeField] private Sprite[] sprites;

    [SerializeField] private GameObject[] deadEnemyEffect;
    [SerializeField] private GameObject deadPlayerEffect;
    [SerializeField] private GameObject boomEffect;

    [SerializeField] private bool isHit;
    public bool isLaserHit;
    [SerializeField] private float laserDelay;

    [SerializeField] private GameObject player;
    [SerializeField] private Player playerCode;

    public static bool playerDead;

    [SerializeField] private ObjectManager objectManager;
    [SerializeField] private SpriteRenderer spriteRenderer;

    Bullet bulletCode;
    Laser laserCode;

    private void Start()
    {
        objectManager = GameObject.Find("ObjectManager").GetComponent<ObjectManager>();

        spriteRenderer = GetComponent<SpriteRenderer>();

        player = GameObject.FindWithTag("Player");
        playerCode = GameObject.Find("Player").GetComponent<Player>();
        Rigidbody2D rigid = GetComponent<Rigidbody2D>();
        rigid.velocity = Vector3.down * speed;

        HPbar = Instantiate(HPbar_Basic) as Slider;
        HPbar.transform.SetParent(GameObject.Find("EnemyHpBarCanvas").transform);
        HPbar.transform.SetAsFirstSibling();
        HPbar.transform.localScale = new Vector3(0.005f, 0.016f, 0);
        HPbar.transform.localRotation = Quaternion.Euler(0, 0, 0);

        HPbar.maxValue = hp;

    }

    private void Update()
    {
        BulletShoot();
        ReloadShoot();

        HpBar_Setting();
        Hpbar_SetActive();

        EnemyIsHitLaser();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("BorderEnemy") || collision.CompareTag("Player") || collision.CompareTag("Pet"))
        {
            Destroy(gameObject);
            Destroy(HPbar.gameObject);
        }

        else if(collision.CompareTag("PlayerBullet"))
        {
            if (hp == 0) return;

            if (!isHit) isHit = true;

            ScoreUp(10);
            playerCode.curPowerPoint += 10;
            collision.gameObject.SetActive(false);
            Effect("H");

            bulletCode = collision.gameObject.GetComponent<Bullet>();
            hp -= bulletCode.Dmg;

            Debug.Log("hp : " + hp);

            spriteRenderer.sprite = sprites[1];
            Invoke("EnemySpriteSwap", 0.1f);

            if(hp <= 0)
            {
                ItemDrop();
                Destroy(gameObject);
                Destroy(HPbar.gameObject);
                ScoreUp(enemyScore);
                playerCode.curPowerPoint += enemyScore;
                Effect("D");
            }
        }
        else if (collision.CompareTag("Boom"))
        {
            if (hp == 0) return;

            ScoreUp(10);
            Effect("H");

            hp -= 50;

            Debug.Log("hp : " + hp);

            if (hp <= 0)
            {
                ItemDrop();
                Destroy(gameObject);
                Destroy(HPbar.gameObject);
                Effect("D");
            }
        }
    }

    void EnemySpriteSwap()
    {
        spriteRenderer.sprite = sprites[0];
    }


    void Effect(string type)
    {
        float desTime = 1.5f;
        int index = 0;
        if (enemyName == "L") index = 0;
        if (enemyName == "M") index = 1;
        if (enemyName == "S") index = 2;
        if (type == "H")
        {
            index = 3;
            desTime = 1.0f;
        }

        if (type != "H") objectManager.deadEnemySound[index].Play();

        GameObject deadEff = Instantiate(objectManager.deadEnemyEffect[index], transform.position + Vector3.down * 0.5f, transform.rotation);

        Destroy(deadEff, desTime);
    }

    void BulletShoot()
    {
        if (!player.activeSelf)
            return;
        if (curShoootTime < maxShootTime)
            return;
        if(enemyName == "S")
        {
            GameObject bulletS = Instantiate(objectManager.enemyBulletObjS, transform.position, transform.rotation);
            Rigidbody2D rigidS = bulletS.gameObject.GetComponent<Rigidbody2D>();
            Vector3 playerPos = player.transform.position - transform.position;
            rigidS.AddForce(playerPos.normalized * 2, ForceMode2D.Impulse);
        }
        else if (enemyName == "M")
        {
            GameObject bulletM = Instantiate(objectManager.enemyBulletObjM, transform.position, transform.rotation);
            Rigidbody2D rigidM = bulletM.gameObject.GetComponent<Rigidbody2D>();
            Vector3 playerPos = player.transform.position - transform.position;
            rigidM.AddForce(playerPos.normalized * 2, ForceMode2D.Impulse);
        }
        else if (enemyName == "L")
        {
            GameObject bulletL = Instantiate(objectManager.enemyBulletObjS, transform.position, transform.rotation);
            Rigidbody2D rigidL = bulletL.gameObject.GetComponent<Rigidbody2D>();
            Vector3 playerPos = player.transform.position - transform.position;
            rigidL.AddForce(playerPos.normalized * 2, ForceMode2D.Impulse);
        }

        maxShootTime = Random.Range(2.0f, 3.0f);
        curShoootTime = 0;
    }

    void ReloadShoot()
    {
        curShoootTime += Time.deltaTime;
    }

    void ScoreUp(int score)
    {
        GameManager.gameScore += score;
    }

    void ItemDrop()
    {
        int ran = Random.Range(0, 10);
        int itemIndex = 0;
        if (ran < 2) return;
        else if (ran < 4) itemIndex = 0;
        else if (ran < 6) itemIndex = 1;
        else if (ran < 8) itemIndex = 2;
        else itemIndex = 3;

        Instantiate(objectManager.itemObjs[itemIndex], transform.position, transform.rotation);
    }

    void HpBar_Setting()
    {
        float pos = 0;
        if (enemyName == "L") pos = 1.0f;
        else if (enemyName == "M") pos = 0.8f;
        else if (enemyName == "S") pos = 0.6f;

        HPbar.value = hp;
        HPbar.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y + pos, gameObject.transform.position.z);
    }

    void Hpbar_SetActive()
    {
        HPbar.gameObject.SetActive(false);

        if (isHit) HPbar.gameObject.SetActive(true);
    }

    void EnemyIsHitLaser()
    {
        if(isLaserHit)
        {
            if(laserDelay > 0.1f)
            {
                laserCode = GameObject.Find("Laser").GetComponent<Laser>();
                hp -= laserCode.laserDmg;

                spriteRenderer.sprite = sprites[1];
                Invoke("EnemySpriteSwap", 0.1f);

                Effect("H");
                laserDelay = 0f;
            }
            else
            {
                laserDelay += Time.deltaTime;
            }

            if(hp<=0)
            {
                Destroy(gameObject);
                Destroy(HPbar.gameObject);
                ScoreUp(enemyScore);
                Effect("D");
                ItemDrop();
            }

            isLaserHit = false;
        }
    }    
}
