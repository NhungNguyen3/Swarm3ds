using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Jobs;
using Random = UnityEngine.Random;

public class BulletMovement : MonoBehaviour
{

    private TransformAccessArray transformAccessArray;
    private NativeArray<BulletData> bulletDatas;
    [SerializeField] public List<Transform> bullets = new List<Transform>();
    bool haveBullet = false;

    private void Start()
    {
        CreateData(bullets);
    }

    private struct BulletData
    {
        public float3 position;
        public float3 velocity;
    }




    public void CreateData(List<Transform> bullets)
    {
        if(bullets == null) return;
        else
        {
            var bulletcount = bullets.Count;
            Debug.Log(bulletcount);
            transformAccessArray = new TransformAccessArray(bulletcount);
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
            haveBullet = true;
        }

    }
    private void OnDestroy()
    {
        transformAccessArray.Dispose();
        bulletDatas.Dispose();

    }

    private void Update()
    {
        if (haveBullet)
        {
            var enemyMovementJob = new EnemyMovementJob
            {
                bullets = bulletDatas,
                deltaTime = Time.deltaTime,
                speed = 10,
                startAngle = transform.eulerAngles.z,
                angleIncrement = 360f / 12
            };

            JobHandle enemyMovementJobHandle = enemyMovementJob.Schedule(transformAccessArray);
            JobHandle.ScheduleBatchedJobs();
            enemyMovementJobHandle.Complete();
        }
        else return;
    }


    [BurstCompile]
    private struct EnemyMovementJob : IJobParallelForTransform
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
            NativeArray<BulletData> bullets1 = bullets;
            Vector3 position = bullets1[index].position;
            float angle = startAngle + index * angleIncrement;
            Vector3 direction = Quaternion.Euler(0, 0, angle) * Vector3.right;

            transform.position += direction * speed * deltaTime;
            transform.rotation = Quaternion.Euler(0, 0, angle - 90);
            // transform.position = Vector3.MoveTowards(transform.position, target, speed * deltaTime);
            bullets[index] = new BulletData
            {
                position = transform.position,
                velocity = direction,
            };
        }
    }
}
