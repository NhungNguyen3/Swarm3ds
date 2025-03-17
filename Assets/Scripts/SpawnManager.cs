
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Jobs;
using Random = UnityEngine.Random;
using UnityEngine.Pool;
public class SpawnManager : MonoBehaviour
{
    [SerializeField] private ListObjectVariable objects;
    [SerializeField] private Enemy objectsPrefabs;
    [SerializeField] private int objectCount = 10;
    [SerializeField] int defaultCapacity = 400;
    [SerializeField] int maxSize = 600;
    bool collectionCheck = true;
    private IObjectPool<Enemy> objectPool;

    private void Awake()
    {
        Application.targetFrameRate = 60;
        if (objects.enemies.Count > 0) objects.enemies.Clear();

        objectPool = new ObjectPool<Enemy>(CreateBullet, OnGetFromPool, OnReleaseToPool,
            OnDestroyPooledObject, collectionCheck, defaultCapacity, maxSize);


        for (int i=0; i< objectCount; i++)
        {
            Vector3 position = new Vector3(Random.Range(0f, 300f), 0, Random.Range(0f, 300f));
            var newEnemy = objectPool.Get();
            newEnemy.transform.position = position;
            objects.enemies.Add(newEnemy.transform);
        }
    }

    private void Start()
    {

    }

    public void SpawnEnemies()
    {

            Vector3 position = new Vector2(Random.Range(-20f, 20f), Random.Range(-20f, 20f));
            var newEnemy = objectPool.Get();
            newEnemy.transform.position = position;
            objects.enemies.Add(newEnemy.transform);

    }


    #region Pooling
    private Enemy CreateBullet()
    {
        Enemy bulletInstance = Instantiate(objectsPrefabs);
        bulletInstance.ObjectPool = objectPool;
        return bulletInstance;
    }


    private void OnGetFromPool(Enemy pooledObject)
    {
        pooledObject.gameObject.SetActive(true);
    }

    private void OnReleaseToPool(Enemy pooldedObject)
    {
        pooldedObject.gameObject.SetActive(false);

    }

    private void OnDestroyPooledObject(Enemy pooldedObject)
    {
        Destroy(pooldedObject.gameObject);
    }
    #endregion
}
