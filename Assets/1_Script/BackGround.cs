using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackGround : MonoBehaviour
{
    public string bgName;
    public static float speed;
    public float pos;

    public int startIndex;
    public int endIndex;

    public Transform[] bgImgs;

    private void Start()
    {
        speed = 1f;
    }

    private void Update()
    {
        if (bgName == "Land") pos = 18.8f;
        else pos = Random.Range(40, 60);

        Vector3 curPos = transform.position;

        Vector3 nextPos = Vector3.down * speed * Time.deltaTime;

        transform.position = curPos + nextPos;

        if(bgImgs[startIndex].position.y < - pos)
        {
            Vector3 endPos = bgImgs[endIndex].localPosition;

            bgImgs[startIndex].localPosition = endPos + Vector3.up * pos;

            int temp = startIndex;
            startIndex = endIndex;
            endIndex = temp;
        }
    }
}
