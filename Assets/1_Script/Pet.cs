using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pet : MonoBehaviour
{
    public float maxBulletShootTime;
    public float curBulletShootTime;

    public Player playerCode;
    public ObjectManager objectManager;

    private void Update()
    {
        PetBullet();
        Reload();
    }


    void PetBullet()
    {
        if (!Input.GetKey(KeyCode.Space)) return;
        if (gameObject.activeSelf == false) return;
        if (curBulletShootTime < maxBulletShootTime) return;

        if(playerCode.bulletSpeed == 1)
        {
            playerCode.bulletSpeed = 2 + Player.power;
            maxBulletShootTime = 0.5f - (0.1f * Player.power);
        }
        else
        {
            playerCode.bulletSpeed = 1.0f + (0.5f * Player.power);
            maxBulletShootTime = 0.6f - (0.1f * Player.power);
        }

        GameObject bullet_01 = Instantiate(objectManager.petBulletObj, transform.position + Vector3.up * 0.7f, transform.rotation);
        Rigidbody2D rigid_01 = bullet_01.GetComponent<Rigidbody2D>();

        rigid_01.AddForce(Vector3.up * playerCode.bulletSpeed, ForceMode2D.Impulse);

        curBulletShootTime = 0;
    
    }

    void Reload()
    {
        curBulletShootTime += Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("EnemyBullet"))
        {
            gameObject.SetActive(false);

            objectManager.deadPlayerSound.Play();

            GameObject eff = Instantiate(objectManager.deadPlayerEffect, transform.position, transform.rotation);
            Destroy(eff, 1.5f);
        }
    }
}
