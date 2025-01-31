using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class Bullet : MonoBehaviour
{
    [SerializeField] private int dmg;
    public int Dmg
    {
        get { return dmg; }
    }
    [SerializeField] private float speed;

    [SerializeField] private bool isRotate;


    private void Update()
    {
        if (isRotate)
        {
            transform.Rotate(Vector3.forward * 5 * Time.timeScale);
        }  
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("BorderPlayer"))
        {
            gameObject.SetActive(false);
        }
    }
}
