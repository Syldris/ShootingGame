using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class Object_Pooling : MonoSingleton<Object_Pooling>
{
    [SerializeField] private GameObject PlayerBulletA_Object;
    public GameObject[] PlayerBulletA_Objects;

    [SerializeField] private GameObject PlayerBulletB_Object;
    public GameObject[] PlayerBulletB_Objects;

    [SerializeField] private GameObject EnemyL_Object;
    public GameObject[] EnemyL_Objects;

    [SerializeField] private GameObject EnemyM_Object;
    public GameObject[] EnemyM_Objects;

    [SerializeField] private GameObject EnemyS_Object;
    public GameObject[] EnemyS_Objects;


    private void Start()
    {
        PlayerBulletA_Objects = new GameObject[70];
        for(int i = 0; i< PlayerBulletA_Objects.Length; i++)
        {
            GameObject gameObject = Instantiate(PlayerBulletA_Object);
            PlayerBulletA_Objects[i] = gameObject;
            gameObject.SetActive(false);
        }

        PlayerBulletB_Objects = new GameObject[70];
        for (int i = 0; i < PlayerBulletB_Objects.Length; i++)
        {
            GameObject gameObject = Instantiate(PlayerBulletB_Object);
            PlayerBulletB_Objects[i] = gameObject;
            gameObject.SetActive(false);
        }

        EnemyL_Objects = new GameObject[70];
        for (int i = 0; i < EnemyL_Objects.Length; i++)
        {
            GameObject gameObject = Instantiate(EnemyL_Object);
            EnemyL_Objects[i] = gameObject;
            gameObject.SetActive(false);
        }

        PlayerBulletB_Objects = new GameObject[70];
        for (int i = 0; i < PlayerBulletB_Objects.Length; i++)
        {
            GameObject gameObject = Instantiate(PlayerBulletB_Object);
            PlayerBulletB_Objects[i] = gameObject;
            gameObject.SetActive(false);
        }

        PlayerBulletB_Objects = new GameObject[70];
        for (int i = 0; i < PlayerBulletB_Objects.Length; i++)
        {
            GameObject gameObject = Instantiate(PlayerBulletB_Object);
            PlayerBulletB_Objects[i] = gameObject;
            gameObject.SetActive(false);
        }
    }
}
