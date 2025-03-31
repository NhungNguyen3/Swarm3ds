
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Jobs;
using Random = UnityEngine.Random;
using UnityEngine.Pool;
public class SpawnManager : Singleton<SpawnManager>
{
    [SerializeField] public List<Enemy> objects;
    [SerializeField] public List<Enemy> objectsRange;
    [SerializeField] private Enemy objectsPrefabs;
    [SerializeField] private int objectCount;
    [SerializeField] int defaultCapacity = 400;
    [SerializeField] int maxSize = 600;
    bool collectionCheck = true;
    private IObjectPool<Enemy> objectPool;

    private void Awake()
    {
        Application.targetFrameRate = 60;
        SpawnEnemies();
    }

    private void Start()
    {
        
    }

    public void SpawnEnemies()
    {
        if (objects.Count > 0) objects.Clear();
        if (objectsRange.Count > 0) objectsRange.Clear();

        objectPool = new ObjectPool<Enemy>(CreateBullet, OnGetFromPool, OnReleaseToPool,
            OnDestroyPooledObject, collectionCheck, defaultCapacity, maxSize);
        GameObject player = GameObject.FindGameObjectWithTag("Head");

        for (int i = 0; i < objectCount; i++)
        {
            Vector3 position = new Vector3(Random.Range(0f, 300f), 0, Random.Range(0f, 300f));
            var newEnemy = objectPool.Get();
            newEnemy.playerHead = player;
            newEnemy.transform.position = position;
            newEnemy.name = "enemy " + i;
            if (i % 2 == 0)
            {
                newEnemy.type = EnemyType.Melee;
            }
            else
            {
                newEnemy.type = EnemyType.Range;
            }
            newEnemy.SetSkin();
            objects.Add(newEnemy);
        }

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
