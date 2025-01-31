using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class EnemyBoss : MonoBehaviour
{
    public float speed;             // ���� �̵� �ӵ�
    public int hp;                  // ���� ü��
    public int enemyScore;          // ���� ���̸� ��� ����
    public string enemyName;        // ���� �̸�

    public float maxShootTime;      // �Ѿ� �߻� �ð� ����
    public float curShootTime;      // �Ѿ� �߻� �ð� ����

    public float bulletSpeed;       // �Ѿ� �ӵ�
    public int patternIndex;        // ���� ���� ���� �ε���
    public int curPatternCount;     // ���� ���� ���� ���ؽ�
    public int[] maxPatternCount;   // ���� ĭ����(�ϳ��� ������ ��� �ݺ��� ���ΰ�? ����)

    public Slider HPbar;            // ������ HPbar
    public Slider HpBar_Basic;      // ���������� �Ҵ� �� HpBar

    public Sprite[] sprites;        // �÷��̾� �Ѿ˿� �´� ȿ����

    public bool isLaserHit;         // �÷��̾� Laser�� ���� �� Ȯ��
    public float laserDelay;        // Laser�� ������ Delay �ð� ���� HP ����

    public GameObject gameManager;          // ���Ӹ޴��� GameObject ��������
    public GameObject player;               // �÷��̾� GameObject ��������
    public Player playerCode;               // �÷��̾� Logic ��������
    public ObjectManager objectManager;     // ObjectManager Logic ��������
    public SpriteRenderer spriteRenderer;   // SpriteRenderer Component ��������


    Bullet bulletCode;              // �Ѿ� Logic ��������
    Laser laserCode;                // ������ Logic ��������


    void Awake()
    {
        GameManager.Instance.colorRed();
        objectManager = GameObject.Find("ObjectManager").GetComponent<ObjectManager>();

        spriteRenderer = GetComponent<SpriteRenderer>();

        gameManager = GameObject.FindWithTag("GameController");

        player = GameObject.FindWithTag("Player");
        playerCode = GameObject.Find("Player").GetComponent<Player>();

        laserCode = GameObject.Find("Laser").GetComponent<Laser>();

        // ������ �����Ǹ� �Ʒ� �������� speed �ӵ��� �̵�
        Rigidbody2D rigid = GetComponent<Rigidbody2D>();
        rigid.velocity = Vector3.down * speed;
        //animator = GetComponent<Animator>();
    }

    void Start()
    {
        // HPbar ����
        HPbar = Instantiate(HpBar_Basic) as Slider;
        // ķ������ �θ�� ����
        HPbar.transform.SetParent(GameObject.Find("EnemyHpBarCanvas").transform);
        // �ٸ� ������Ʈ ���� HPbar�� ǥ��
        HPbar.transform.SetAsFirstSibling();
        // HPbar ũ�� ����
        HPbar.transform.localScale = new Vector3(0.01f, 0.02f, 0);
        // HPbar ���� �ʱ�ȭ
        HPbar.transform.localRotation = Quaternion.Euler(0, 0, 0);
        // HPbar �� ����
        HPbar.maxValue = hp;

        Think();    // ���� ���� ���� ���� �� �����̽ð�
    }

    void Update()
    {
        HpBar_Setting();        // ���� HpBar �ʱ�ȭ
        EnemyIsHitLaser();      // 
    }

    void Think()
    {
        patternIndex = patternIndex == 3 ? 0 : patternIndex + 1;
        curPatternCount = 0;

        switch (patternIndex)
        {
            case 0:
                FireFoward();
                break;
            case 1:
                FireShot();
                break;
            case 2:
                FireArc();
                break;
            case 3:
                FireAround();
                break;
        }
    }

    // ������ 4�� �߻�
    void FireFoward()
    {
        Debug.Log("FireFoward() ȣ�� : ������ 4�� �߻�");

        bulletSpeed = 6.0f;
        float posX;

        if (curPatternCount % 2 == 0) posX = 0.8f;
        else posX = 0.7f;

        for(int i = 0; i < 5; i++)
        {
            posX -= 0.2f;
            GameObject bullet_RR = Instantiate(objectManager.enemyBulletObjL, transform.position, transform.rotation);
            Rigidbody2D rigid_RR = bullet_RR.GetComponent<Rigidbody2D>();
            rigid_RR.AddForce((Vector2.down + new Vector2(posX, 0)) * bulletSpeed, ForceMode2D.Impulse);
        }

        curPatternCount++;

        if (curPatternCount <= maxPatternCount[patternIndex])
            Invoke("FireFoward", 1.5f);
        else
            Invoke("Think", 2.0f);
    }

    // �÷��̾� �������� ����
    void FireShot()
    {
        Debug.Log("FireShot() ȣ�� : �÷��̾� �������� ����");
        bulletSpeed = 6.0f;
        float posX = 0.6f;

        for (int i = 0; i < 5; i++)
        {
            posX -= 0.2f;
            GameObject bullet = Instantiate(objectManager.enemyBulletObjL, transform.position, transform.rotation);
            Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
            Vector2 playerPos = player.transform.position - transform.position;
            rigid.AddForce((playerPos.normalized + new Vector2(posX, 0)) * bulletSpeed, ForceMode2D.Impulse);
        }

        curPatternCount++;

        if (curPatternCount < maxPatternCount[patternIndex])
            Invoke("FireShot", 3.5f);
        else
            Invoke("Think", 3f);
    }

    // ��ä������� �߻�
    void FireArc()
    {
        Debug.Log("FireArc() ȣ�� : ��ä������� �߻�");
        bulletSpeed = 6.0f;

        GameObject bullet = Instantiate(objectManager.enemyBulletObjS, transform.position, transform.rotation);
        bullet.transform.position = transform.position;
        bullet.transform.rotation = Quaternion.identity;
        
        Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
        Vector2 dirVec = new Vector2(Mathf.Cos(Mathf.PI * 10 * curPatternCount / maxPatternCount[patternIndex]), -1);
        rigid.AddForce(dirVec.normalized * bulletSpeed, ForceMode2D.Impulse);

        curPatternCount++;

        if (curPatternCount < maxPatternCount[patternIndex])
            Invoke("FireArc", 0.15f);
        else
            Invoke("Think", 6f);
    }

    // �� ���·� ��ü ����
    void FireAround()
    {
        Debug.Log("FireAround() ȣ�� : �� ���·� ��ü ����");

        int roundNum;
        if (curPatternCount % 2 == 0)
            roundNum = 30;
        else
            roundNum = 40;

        bulletSpeed = 2f;

        for(int index = 0; index<roundNum; index ++)
        {
            GameObject bullet = Instantiate(objectManager.enemyBulletObjB, transform.position, transform.rotation);
            bullet.transform.position = transform.position;

            bullet.transform.rotation = Quaternion.identity;
            Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
            Vector2 dirVec = new Vector2(Mathf.Cos(Mathf.PI * 2 * index /roundNum), Mathf.Sin(Mathf.PI * 2 * index /roundNum));
            rigid.AddForce(dirVec.normalized * bulletSpeed, ForceMode2D.Impulse);

            Vector3 rotVec = Vector3.forward * 360 * index / roundNum + Vector3.forward * 90;
            bullet.transform.Rotate(rotVec);
        }

        curPatternCount++;

        if (curPatternCount < maxPatternCount[patternIndex]) Invoke("FireAround", 1f);
        else Invoke("Think", 3f);
    }

    // ������ Ȱ��ȭ �Ǹ� 2�� �Ŀ� ����
    void OnEnable()
    {
        // �������� ���� ���� �Լ� ȣ��
        Invoke("EnemyBossStop", 1.3f);    // ���� �̵� ����
    }


    // �������� ���� ����
    void EnemyBossStop()
    {
        Rigidbody2D rigid = GetComponent<Rigidbody2D>();
        rigid.velocity = Vector2.zero;
    }


    // �� HpBar �ʱ�ȭ
    void HpBar_Setting()
    {
        HPbar.value = hp;
        HPbar.transform.position = new Vector3(gameObject.transform.position.x,
                                                gameObject.transform.position.y + 1.8f,
                                                gameObject.transform.position.z);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (hp <= 0) return;

        if (collision.gameObject.tag == "PlayerBullet")
        {
            // �÷��̾� �Ѿ˿� ������
            spriteRenderer.sprite = sprites[1];  // �÷��̾� �Ѿ˿� �´� ��½�̴� ȿ��
            Invoke("EnemySpriteSwap", 0.1f);     // ��½�� �ٽ� �����·� ���ƿ���

            Effect("H"); // Hit Effect
            ScoreUp(10);  // ���� ����

            collision.gameObject.SetActive(false);  // �Ѿ� �Ҹ�

            bulletCode = collision.gameObject.GetComponent<Bullet>();
            hp -= bulletCode.Dmg;           // hp ����

            if (hp <= 0)
            {
                Effect("D");                // Dead Effect
                Destroy(HPbar.gameObject);
                Destroy(gameObject);

                ScoreUp(enemyScore);        // ���� ����
                ItemDrop();                 // ������ ���� ����
                GameManager.isGameClear = true;
            }
        }
    }

    // �������� ������ �浹 �����̸�
    // ���� �������� ���� �����̸�
    void EnemyIsHitLaser()
    {
        if(isLaserHit)
        {
            if(laserDelay > 0.3f)
            {
                spriteRenderer.sprite = sprites[1];
                Invoke("EnemySpriteSwap",0.1f);

                laserCode = GameObject.Find("Laser").GetComponent<Laser>();
                hp -= laserCode.laserDmg;

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
                GameManager.Instance.colorWhite();
                ItemDrop();
                GameManager.isGameClear = true;
            }

            isLaserHit = false;
        }
    }

    // ������ ���� ����
    void ItemDrop()
    {
        for (int i = 0; i < 10; i++)
        {
            // ������ Ȯ���� ������ ���
            int ran = Random.Range(0, 10);
            int itemIndex = 0;
            if (ran < 2) itemIndex = 0;
            else if (ran < 4) itemIndex = 1;
            else if (ran < 6) itemIndex = 2;
            else if (ran < 8) itemIndex = 3;
            else itemIndex = 4;

            float posX = Random.Range(-2.0f, 2.0f);
            float posY = Random.Range(-2.0f, 2.0f);

            Instantiate(objectManager.itemObjs[itemIndex],
                         transform.position + Vector3.up * posX + Vector3.left * posY,
                         transform.rotation);

        }
        // ���ʽ� ���� : Coin ������ 100��(���� �������) 
        for (int j = 0; j < 100; j++)
        {
            float posX = Random.Range(-3.0f, 3.0f);
            float posY = Random.Range(-3.0f, 2.0f); // 0.00 0.00

            Instantiate(objectManager.itemObjs[4],
                            transform.position + Vector3.up * posX + Vector3.left * posY,
                            transform.rotation);
        }

    }

    // �Ѿ��̳� �������� ������ ��½�̴� ȿ���� �ֱ����� ��������Ʈ ����
    void EnemySpriteSwap()
    {
        spriteRenderer.sprite = sprites[0];
    }

    // �� �ı� ����Ʈ 
    void Effect(string type)
    {
        int index;
        float desTime;

        if (type == "D") // �׾��� �� ����Ʈ
        {
            index = 0;
            desTime = 1.5f;
        }
        else            // Hit �� ����Ʈ
        {
            index = 3;
            desTime = 1.0f;
        }
        // Hit ȿ������ ����, �� L, M, S�� ���� �ı��� ȿ���� �����Ͽ� ���
        if (type != "H") objectManager.deadEnemySound[2].Play();
        // �ı� ����Ʈ 
        GameObject deadEff = Instantiate(objectManager.deadEnemyEffect[index], transform.position, transform.rotation);
        deadEff.transform.localScale = new Vector3(3f, 3f, 0);
        Destroy(deadEff, desTime);  // 1.5�� �� �ı� ����Ʈ �Ҹ�
    }

    void ScoreUp(int score)
    {
        GameManager.gameScore += score;    // ���� ���� ����
        playerCode.PowerUpPoint(score);    // �÷��̾� �Ѿ� ���׷��̵� �� ����
    }

}
