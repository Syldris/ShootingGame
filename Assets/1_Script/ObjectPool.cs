using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    [System.Serializable]
    public class Pool
    {
        public string tag;
        public GameObject prefab; 
        public int size;
    }

    public static ObjectPool Instance; // 싱글톤 인스턴스

    private void Awake() => Instance = this;

    public List<Pool> pools; // 풀 목록 (Inspector에서 설정)
    private Dictionary<string, Queue<GameObject>> poolDictionary;

    private void Start()
    {
        poolDictionary = new Dictionary<string, Queue<GameObject>>();

        // 각 풀을 초기화하여 딕셔너리에 저장
        foreach (Pool pool in pools)
        {
            Queue<GameObject> objectQueue = new Queue<GameObject>();

            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab);
                obj.SetActive(false);
                objectQueue.Enqueue(obj); //큐에 추가
            }

            poolDictionary.Add(pool.tag, objectQueue); //딕셔너리에 추가
        }
    }

    public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
    {
        GameObject objectToSpawn = poolDictionary[tag].Dequeue(); //큐에서 꺼내기
        objectToSpawn.SetActive(true);
        objectToSpawn.transform.SetPositionAndRotation(position, rotation);
        poolDictionary[tag].Enqueue(objectToSpawn); //다시 큐에 추가하여 재사용

        return objectToSpawn;
    }
}

