using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class BulletsManager : Singleton<BulletsManager>
{
    [SerializeField] BulletObject bulletPrefab;
    [SerializeField] BulletObject bulletPrefabAngle;
    [SerializeField] BulletObject bulletRocket;
    [SerializeField] BulletObject bulletEnemy;

    [Header("Player bullets")]
    public IObjectPool<BulletObject> objectPool;
    public IObjectPool<BulletObject> objectPoolAngle;
    public IObjectPool<BulletObject> objectPoolRocket;

    [Header("Enemy bullets")]
    public IObjectPool<BulletObject> objectPoolEnemy;
    [SerializeField] int defaultCapacity = 20;

    [SerializeField] int maxSize = 100;
    bool collectionCheck = true;

    private void Awake()
    {
        objectPool = new ObjectPool<BulletObject>(CreateBullet, OnGetFromPool, OnReleaseToPool,
            OnDestroyPooledObject, collectionCheck, defaultCapacity, maxSize);
        objectPoolAngle = new ObjectPool<BulletObject>(CreateBulletAngle, OnGetFromPool, OnReleaseToPool,
            OnDestroyPooledObject, collectionCheck, defaultCapacity, maxSize);
        objectPoolRocket = new ObjectPool<BulletObject>(CreateBulletRocket, OnGetFromPool, OnReleaseToPool,
            OnDestroyPooledObject, collectionCheck, defaultCapacity, maxSize);
        objectPoolEnemy = new ObjectPool<BulletObject>(CreateBulletEnemy, OnGetFromPool, OnReleaseToPool,
            OnDestroyPooledObject, collectionCheck, defaultCapacity, maxSize);
    }

    #region Pooling
    private BulletObject CreateBullet()
    {
        BulletObject bulletInstance = Instantiate(bulletPrefab);
        bulletInstance.ObjectPool = objectPool;
        return bulletInstance;
    }

    private BulletObject CreateBulletAngle()
    {
        BulletObject bulletInstance = Instantiate(bulletPrefabAngle);
        bulletInstance.ObjectPool = objectPoolAngle;
        return bulletInstance;
    }

    private BulletObject CreateBulletRocket()
    {
        BulletObject bulletInstance = Instantiate(bulletRocket);
        bulletInstance.ObjectPool = objectPoolRocket;
        return bulletInstance;
    }
    private BulletObject CreateBulletEnemy()
    {
        BulletObject bulletInstance = Instantiate(bulletEnemy);
        bulletInstance.ObjectPool = objectPoolEnemy;
        return bulletInstance;
    }

    private void OnGetFromPool(BulletObject pooledObject)
    {
        pooledObject.gameObject.SetActive(true);
    }

    private void OnReleaseToPool(BulletObject pooldedObject)
    {
        pooldedObject.gameObject.SetActive(false);

    }

    private void OnDestroyPooledObject(BulletObject pooldedObject)
    {
        Destroy(pooldedObject.gameObject);
    }
    #endregion
}
