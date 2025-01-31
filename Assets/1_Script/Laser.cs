using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Laser : MonoBehaviour
{
    public int laserDmg;
    public float maxLaserScale;
    public float laserScale;
    public bool isLaserShoot;
    public bool isHitEnemy;
    public float maxLaserCoolTime;
    public float curLaserCoolTime;

    float laserShowTime;
    public Image laserGauge;

    public GameObject player;

    public ObjectManager objectmanager;
    public GameManager gameManager;

    private LineRenderer lr;

    private void Start()
    {
        lr = GetComponent<LineRenderer>();

        maxLaserCoolTime = 30f;
    }

    private void Update()
    {
        if(isLaserShoot)
        {
            LaserLengthenTime();
            LaserLineRenderer();
        }

        LaserCoolTime();
    }

    public void LaserShoot()
    {
        if(!Player.isPlayerDead && isLaserShoot)
        {
            if (curLaserCoolTime > maxLaserCoolTime)
            {
                laserShowTime = 9f;
            }
            else if (curLaserCoolTime > maxLaserCoolTime / 1.5f)
            {
                laserShowTime = 6f;
            }
            else if (curLaserCoolTime > maxLaserCoolTime / 3f)
            {
                laserShowTime = 3f;
            }
            else return;

            isLaserShoot = true;
            curLaserCoolTime = 0;
            lr.enabled = isLaserShoot;
            objectmanager.laserShootSound.Play();
            Invoke("LaserHide", laserShowTime);
        }
    }

    public void LaserHide()
    {
        curLaserCoolTime = 0;
        isLaserShoot = false;
        lr.enabled = isLaserShoot;
    }

    void LaserLineRenderer()
    {
        if(!isLaserShoot)
        {
            laserScale = 0;
            return;
        }

        lr.SetPosition(0, new Vector3(player.transform.position.x, player.transform.position.y + 0.6f));

        RaycastHit2D hit = Physics2D.Raycast(new Vector2(player.transform.position.x, transform.position.y + 0.6f), transform.up, laserScale);
        
        Debug.DrawRay(player.transform.position, transform.up * laserScale, Color.red, 0.1f);

        if(hit.collider)
        {
            if (hit.collider.CompareTag("Enemy") || hit.collider.CompareTag("EnemyB"))
            {
                if (hit.collider.CompareTag("Enemy"))
                {
                    Enemy enemy = hit.collider.GetComponent<Enemy>();
                    enemy.isLaserHit = true;
                }
                else
                {
                    EnemyBoss enemyB = hit.collider.GetComponent<EnemyBoss>();
                    enemyB.isLaserHit = true;
                }
                lr.SetPosition(1, hit.point);
            }
            else if (hit.collider.CompareTag("EnemyBullet"))
            {
                Destroy(hit.collider.gameObject);
            }
            else lr.SetPosition(1, new Vector3(player.transform.position.x, player.transform.position.y + laserScale, 0));
        }
        else lr.SetPosition(1, new Vector3(player.transform.position.x, player.transform.position.y + laserScale, 0));
    }
    
    void LaserLengthenTime()
    {
        laserScale += Time.deltaTime * 100f;
    }

    void LaserCoolTime()
    {
        if(Player.isPlayerDead == true || isLaserShoot == true)
        {
            curLaserCoolTime = 0f;
            laserGauge.fillAmount = 0f;
        }
        curLaserCoolTime += Time.deltaTime;
        laserGauge.fillAmount = curLaserCoolTime / maxLaserCoolTime;
    }
}
