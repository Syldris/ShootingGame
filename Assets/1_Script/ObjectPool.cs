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

    public static ObjectPool Instance; // �̱��� �ν��Ͻ�

    private void Awake() => Instance = this;

    public List<Pool> pools; // Ǯ ��� (Inspector���� ����)
    private Dictionary<string, Queue<GameObject>> poolDictionary;

    private void Start()
    {
        poolDictionary = new Dictionary<string, Queue<GameObject>>();

        // �� Ǯ�� �ʱ�ȭ�Ͽ� ��ųʸ��� ����
        foreach (Pool pool in pools)
        {
            Queue<GameObject> objectQueue = new Queue<GameObject>();

            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab);
                obj.SetActive(false);
                objectQueue.Enqueue(obj); //ť�� �߰�
            }

            poolDictionary.Add(pool.tag, objectQueue); //��ųʸ��� �߰�
        }
    }

    public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
    {
        GameObject objectToSpawn = poolDictionary[tag].Dequeue(); //ť���� ������
        objectToSpawn.SetActive(true);
        objectToSpawn.transform.SetPositionAndRotation(position, rotation);
        poolDictionary[tag].Enqueue(objectToSpawn); //�ٽ� ť�� �߰��Ͽ� ����

        return objectToSpawn;
    }
}

