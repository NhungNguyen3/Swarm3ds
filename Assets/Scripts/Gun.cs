using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Jobs;
using UnityEngine.Pool;
using Random = UnityEngine.Random;
using DG.Tweening;
using static UnityEngine.GraphicsBuffer;

public class Gun : MonoBehaviour
{
    [SerializeField] Bullet bulletPrefab;
    [SerializeField] Bullet bulletPrefabAngle;
    [SerializeField] Bullet bulletRocket;
    [SerializeField] Transform FirePoint;

    private IObjectPool<Bullet> objectPool;
    private IObjectPool<Bullet> objectPoolAngle;
    private IObjectPool<Bullet> objectPoolRocket;
    [SerializeField] int defaultCapacity = 20;
    [SerializeField] int maxSize = 100;
    bool collectionCheck = true;

    public int numberOfBullets = 8; // Number of bullets to shoot
    public float angleBetweenBullets = 45f; // Angle between each bullet in degrees
    public float bulletSpeed = 10f;

    private TransformAccessArray transformAccessArray;
    private NativeArray<BulletData> bulletDatas;
    [SerializeField] public List<Transform> bullets = new List<Transform>(); 

    private struct BulletData
    {
        public float3 position;
        public float3 velocity;
    }


    private void Awake()
    {
        objectPool = new ObjectPool<Bullet>(CreateBullet, OnGetFromPool, OnReleaseToPool,
            OnDestroyPooledObject, collectionCheck, defaultCapacity, maxSize);
        objectPoolAngle = new ObjectPool<Bullet>(CreateBulletAngle, OnGetFromPool, OnReleaseToPool,
            OnDestroyPooledObject, collectionCheck, defaultCapacity, maxSize);
        objectPoolRocket = new ObjectPool<Bullet>(CreateBulletRocket, OnGetFromPool, OnReleaseToPool,
            OnDestroyPooledObject, collectionCheck, defaultCapacity, maxSize);
    }

    #region Pooling
    private Bullet CreateBullet()
    {
        Bullet bulletInstance = Instantiate(bulletPrefab);
        bulletInstance.ObjectPool = objectPool;
        return bulletInstance;
    }

    private Bullet CreateBulletAngle()
    {
        Bullet bulletInstance = Instantiate(bulletPrefabAngle);
        bulletInstance.ObjectPool = objectPoolAngle;
        return bulletInstance;
    }

    private Bullet CreateBulletRocket()
    {
        Bullet bulletInstance = Instantiate(bulletRocket);
        bulletInstance.ObjectPool = objectPoolAngle;
        return bulletInstance;
    }

    private void OnGetFromPool(Bullet pooledObject)
    {
        pooledObject.gameObject.SetActive(true);
    }

    private void OnReleaseToPool(Bullet pooldedObject)
    {
        pooldedObject.gameObject.SetActive(false);

    }

    private void OnDestroyPooledObject(Bullet pooldedObject)
    {
        Destroy(pooldedObject.gameObject);
    }
    #endregion

    public void CreateData()
    {
        var bulletcount = bullets.Count;
/*        
        transformAccessArray = new TransformAccessArray(bulletcount);*/
        bulletDatas = new NativeArray<BulletData>(bulletcount, Allocator.Persistent);

        for (int i = 0; i < bulletcount; i++)
        {
            float speed = Random.Range(1f, 5f);
            transformAccessArray.Add(bullets[i].transform);
            bulletDatas[i] = new BulletData
            {
                position = bullets[i].transform.position,
                velocity = bullets[i].transform.forward,
            };
        }
    }

    public IEnumerator Fire(Vector3 pos)
    {
        Bullet bulletObject = objectPool.Get();
       // bulletObject.transform.SetLocalPositionAndRotation(transform.position, transform.rotation);
        bulletObject.transform.SetLocalPositionAndRotation(transform.position, Quaternion.Euler(0, 0, 0));
        Rigidbody rb = bulletObject.GetComponent<Rigidbody>();


        // Vector3 direction = transform.up;

        bulletObject.sl.enabled = false;
        //rb.velocity = direction * bulletSpeed;
        Vector3 dir = pos - bulletObject.transform.position;
        rb.velocity = new Vector2(dir.x, dir.y).normalized * bulletSpeed;
        transform.up = rb.velocity;
        yield return null;
    }


    public BulletMovement bulletMove;
    public IEnumerator ShootBullets()
    {
        float startAngle = 0;
        float angleIncrement = 360f / numberOfBullets;

        for (int i = 0; i < numberOfBullets; i+=3)
        {
            List<Transform> bullets = new();
            Bullet bulletObjectAngle = objectPoolAngle.Get();
            bulletObjectAngle.transform.SetLocalPositionAndRotation(transform.position, transform.rotation);
            bulletObjectAngle.gun = this;
            bullets.Add(bulletObjectAngle.transform);
            //float angle = startAngle + i * angleIncrement;
            //Vector3 direction = Quaternion.Euler(0, 0, angle) * Vector3.right;
            /*            

                        float angle = startAngle + i * angleIncrement;
                        Vector3 direction = Quaternion.Euler(0, 0, angle) * Vector3.right;

                        transform.position += direction * 10 * Time.deltaTime;
                        transform.rotation = Quaternion.Euler(0, 0, angle - 90);*/
            /*            Rigidbody rb = bulletObjectAngle.GetComponent<Rigidbody>();
                        rb.velocity = direction * bulletSpeed;
                        bulletObjectAngle.transform.rotation = Quaternion.Euler(0, 0, angle - 90);*/
            bulletMove.CreateData(bullets);
            yield return new WaitForEndOfFrame();
        }
/*        var bulletcount = bullets.Count;
        transformAccessArray = new TransformAccessArray(bulletcount);
        CreateData();
        var enemyMovementJob = new EnemyMovementJob
        {
            bullets = bulletDatas,
            deltaTime = Time.deltaTime,
            speed = 10,
            startAngle = transform.eulerAngles.z,
            angleIncrement = 360f / numberOfBullets
        };

        JobHandle enemyMovementJobHandle = enemyMovementJob.Schedule(transformAccessArray);
        enemyMovementJobHandle.Complete();*/


    }

    /*    private void Update()
        {
          *//*  if(bullets.Count !=0)
            {
                CreateData();

                var enemyMovementJob = new EnemyMovementJob
                {
                    bullets = bulletDatas,
                    deltaTime = Time.deltaTime,
                    speed = 10,
                    startAngle = transform.eulerAngles.z,
                    angleIncrement = 360f / numberOfBullets
                };

                JobHandle enemyMovementJobHandle = enemyMovementJob.Schedule(transformAccessArray);
                enemyMovementJobHandle.Complete();
            }*/


    /*    private struct EnemyMovementJob : IJobParallelForTransform
        {
            public NativeArray<BulletData> bullets;
            public float deltaTime;
            public float speed;
            public float minDis;
            public Vector3 pos;

            public float startAngle;
            public float angleIncrement;
            public void Execute(int index, TransformAccess transform)
            {
                Vector3 position = bullets[index].position;
                float angle = startAngle + index * angleIncrement;
                Vector3 direction = Quaternion.Euler(0, 0, angle) * Vector3.right;

                transform.position += direction * speed * deltaTime;
                transform.rotation = Quaternion.Euler(0, 0, angle - 90);
                // transform.position = Vector3.MoveTowards(transform.position, target, speed * deltaTime);
    *//*            bullets[index] = new BulletData
                {
                    position = transform.position,
                    velocity = direction,
                };*//*
            }
        }
    */

    private struct BulletsData
    {
        public float3 position;
        public float3 velocity;
    }

    [BurstCompile]
    private struct BulletMoveJob : IJobParallelForTransform
    {
        public NativeArray<BulletsData> bulletsDatas;
        public float moveSpeed;
        public float deltaTime;
        public float angle;
        public float angleIncrement;
        public float3 direction;
        public void Execute(int index, TransformAccess transform)
        {
            Vector3 position = bulletsDatas[index].position;
            // Vector3 position = transform.position;
            // position += Vector3.right * moveSpeed * deltaTime;
            // float angle = startAngle + index * angleIncrement;

            // Vector3 direction = Quaternion.Euler(0, 0, angle) * Vector3.right;
            Vector3 dir = bulletsDatas[index].velocity;
            transform.position += dir * moveSpeed * deltaTime;
            //transform.position = position;

            bulletsDatas[index] = new BulletsData
            {
                position = transform.position,
                velocity = direction,
            };
        }
    }

    public int bulletCount = 47;
    [BurstCompile]
    public IEnumerator ShootBullets1(Transform firePoint, Vector3 target)
    {
        for (int i = 0; i < bulletCount; i++)
        {
            var bullet = objectPoolAngle.Get();
            float startAngle = 0;
            float angleIncrement = 7.5f;
            float angle = startAngle + i * angleIncrement;

            bullet.transform.SetLocalPositionAndRotation(firePoint.transform.position, Quaternion.Euler(0, 0, 0));
            Vector3 direction = Quaternion.Euler(0,  angle - 90,0) * bullet.transform.forward;
            bullet.transform.rotation = Quaternion.Euler(0,  angle - 90, 0);
            Rigidbody rb = bullet.GetComponent<Rigidbody>();
           // rb.AddForce(rb.velocity * bulletSpeed);
            rb.velocity = direction * bulletSpeed;

            transform.up = rb.velocity;
            yield return new WaitForSeconds(0.05f); 
        }
    }

    public int bulletCount2 = 4;
    public GameObject subGun1;
    public GameObject subGun2;
    public GameObject subGun3;
    public GameObject subGun4;
    public IEnumerator ShootBullets2(GameObject subGun)
    {
        for (int i = 0; i < bulletCount2; i++)
        {
            var bullet = objectPool.Get();
            float startAngle = 0;
            float angleIncrement = 7.5f;
            float angle = startAngle + i * angleIncrement;

            bullet.transform.SetLocalPositionAndRotation(subGun.transform.position, Quaternion.Euler(0, 0, 0));
            Vector3 direction = subGun.transform.up;

           // bullet.transform.rotation = Quaternion.Euler(0, 0, angle - 90);
            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            rb.velocity = direction * bulletSpeed;

            bullet.transform.up = rb.velocity;
            yield return new WaitForSeconds(0.05f);
        }
    }

    public IEnumerator Shooting2()
    {
        StartCoroutine(ShootBullets2(subGun1));
        StartCoroutine(ShootBullets2(subGun2));
        StartCoroutine(ShootBullets2(subGun3));
        StartCoroutine(ShootBullets2(subGun4));
        yield return new WaitForSeconds(.1f);
    }

    [BurstCompile]
    public IEnumerator ShootBulletRocket(Vector3 pos)
    {
        Debug.Log("pos");
        Bullet bulletObject = objectPoolRocket.Get();
        bulletObject.transform.SetLocalPositionAndRotation(this.transform.position, Quaternion.Euler(0, 0, 0));
        Rigidbody rb = bulletObject.GetComponent<Rigidbody>();

        bulletObject.sl.enabled = false;

        bulletObject.transform.DOMove(pos, 1f);
        bulletObject.transform.DOLookAt(pos, 0f);
        yield return null;
    }

    public IEnumerator DroneShoot(Transform firePoint, Vector3 target)
    {

        var bullet = objectPoolAngle.Get();

        bullet.transform.SetLocalPositionAndRotation(firePoint.transform.position, Quaternion.Euler(0, 0, 0));
        bullet.transform.rotation = firePoint.rotation;
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        // rb.AddForce(rb.velocity * bulletSpeed);
        // rb.velocity = firePoint.forward * bulletSpeed;
        bullet.transform.DOMove(target, .5f);
        bullet.transform.DOLookAt(target, 0f);
       // transform.up = rb.velocity;
        yield return new WaitForSeconds(0.05f);
    }

    public IEnumerator ShootingNormal(Transform firePoint)
    {

        var bullet = objectPoolAngle.Get();

        bullet.transform.SetLocalPositionAndRotation(firePoint.transform.position, Quaternion.Euler(0, 0, 0));
        bullet.transform.rotation = firePoint.rotation;
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        // rb.AddForce(rb.velocity * bulletSpeed);
        rb.velocity = firePoint.forward * bulletSpeed;

        transform.up = rb.velocity;
        yield return new WaitForSeconds(0.05f);
    }

}
