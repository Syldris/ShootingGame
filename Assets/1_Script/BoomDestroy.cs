using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoomDestroy : MonoBehaviour
{
    void Start()
    {
        GameManager.Instance.Boomstate = true;
        StartCoroutine(Boom());
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("EnemyBullet"))
        {
            Destroy(collision.gameObject);
        }
    }
    IEnumerator Boom()
    {
        yield return new WaitForSeconds(5.9f);
        GameManager.Instance.Boomstate = false;
        Destroy(gameObject);
    }
}
