using Scripts.Projectiles;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Scripts.ObjectPools
{
    [System.Serializable]
    public enum PoolType
    {
        standardMissile,
        homingMissile
    }

    [System.Serializable]
    public abstract class GenericObjectPool : MonoBehaviour
    {
        [System.Serializable]
        public class PoolInfo
        {
            public GameObject prefab;
            public int size;
        }

        public static GenericObjectPool Instance { get; private set; }


        [SerializeField] List<PoolInfo> poolTypes = null;
        Dictionary<Type, Queue<IPoolObject>> pools = new Dictionary<Type, Queue<IPoolObject>>();

        private void Awake()
        {
            Instance = this;

            foreach (PoolInfo pinfo in poolTypes)
            {
                Type poolType = pinfo.prefab.GetComponent<IPoolObject>().GetType();
                if (poolType == null)
                {
                    Debug.LogWarning("object " + pinfo.prefab.name + " does not implement a IPoolObject interface!");
                    continue;
                }

                if (pools.ContainsKey(poolType))
                    continue;

                Queue<IPoolObject> qObjs = new Queue<IPoolObject>();

                for (int i = 0; i < pinfo.size; i++)
                {
                    GameObject obj = Instantiate(pinfo.prefab);
                    var poolObj = obj.GetComponent<IPoolObject>();
                    obj.SetActive(false);
                    qObjs.Enqueue(poolObj);
                }

                pools.Add(poolType, qObjs);
            }
        }

        public IPoolObject GetFromPool(Type key, Vector3 position, Quaternion rotation)
        {
            if (!pools.ContainsKey(key))
            {
                Debug.LogWarning("object key is not in the object pool");
                return null;
            }

            IPoolObject poolObj = pools[key].Dequeue();
            GameObject obj = poolObj.GetGameObject();

            obj.transform.position = position;
            obj.transform.rotation = rotation;
            pools[key].Enqueue(poolObj);
            return poolObj;
        }

        public GameObject GetFromPool(Type key)
        {
            if (!pools.ContainsKey(key))
            {
                Debug.LogWarning("object key is not in the object pool");
                return null;
            }

            IPoolObject poolObj = pools[key].Dequeue();
            GameObject obj = poolObj.GetGameObject();

            pools[key].Enqueue(poolObj);
            return obj;
        }
    }
}