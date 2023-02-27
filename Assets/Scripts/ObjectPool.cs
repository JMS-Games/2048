using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool instance;

    [SerializeField]
    private GameObject blockPrefab;

    Queue<Block> queue = new Queue<Block>();

    void Awake()
    {
        instance = this;

        for(int i = 0;i < 16; i++)
        {
            queue.Enqueue(CreateNewBlock());
        }
    }

    private Block CreateNewBlock()
    {
        var newBlock = Instantiate(blockPrefab).GetComponent<Block>();
        newBlock.gameObject.SetActive(false);
        newBlock.transform.SetParent(transform);
        return newBlock;
    }

    public static Block GetObject()
    {
        if (instance.queue.Count > 0)
        {
            var obj = instance.queue.Dequeue();
            obj.transform.SetParent(null);
            obj.gameObject.SetActive(true);
            return obj;
        } else
        {
            var newObj = instance.CreateNewBlock();
            newObj.gameObject.SetActive(true);
            newObj.transform.SetParent(null);
            return newObj;
        }
    }

    public static void ReturnObject(Block obj)
    {
        obj.transform.SetParent(instance.transform);
        obj.gameObject.SetActive(false);
        instance.queue.Enqueue(obj);
    }
}
