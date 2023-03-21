using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    private Dictionary<int, Queue<GameObject>> poolDictionary = new Dictionary<int, Queue<GameObject>>();

    public GameObject GetObject(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        int key = prefab.GetInstanceID();
        if (!poolDictionary.ContainsKey(key))
        {
            poolDictionary.Add(key, new Queue<GameObject>());
        }

        Queue<GameObject> objectQueue = poolDictionary[key];
        GameObject obj;

        if (objectQueue.Count > 0)
        {
            obj = objectQueue.Dequeue();
            obj.SetActive(true);
            obj.transform.position = position;
            obj.transform.rotation = rotation;
        }
        else
        {
            obj = Instantiate(prefab, position, rotation);
        }

        return obj;
    }

    public void ReturnObject(GameObject obj)
    {
        int key = obj.GetInstanceID();
        if (poolDictionary.ContainsKey(key))
        {
            obj.SetActive(false);
            poolDictionary[key].Enqueue(obj);
        }
        else
        {
            Destroy(obj);
        }
    }
}