using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : MonoSingleton<ObjectManager>
{
    public GameObject playerBulletObjA;
    public GameObject playerBulletObjB;

    public GameObject petBulletObj;

    public GameObject enemyBulletObjL;
    public GameObject enemyBulletObjM;
    public GameObject enemyBulletObjS;
    public GameObject enemyBulletObjB;

    public AudioSource backgroundSound;
    public AudioSource victoryFanfareSound;
    public GameObject[] enemyObj;
    public AudioSource deadPlayerSound;
    public AudioSource[] deadEnemySound;

    public GameObject[] deadEnemyEffect;
    public GameObject deadPlayerEffect;
    public GameObject boomEffect;

    public GameObject[] itemObjs;

    public AudioSource boomPlayerSound;
    public AudioSource bulletShootSound;
    public AudioSource laserShootSound;
    public AudioSource shieldShootSound;

    public AudioSource showBossSound;
    public AudioSource powerUpSound;

    public AudioSource[] itmeTouchSound;
}
