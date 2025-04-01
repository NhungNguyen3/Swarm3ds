
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Jobs;
using Unity.Burst;
using Random = UnityEngine.Random;
using UnityEngine.EventSystems;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] List<Enemy> listEnemy;
    private TransformAccessArray transformAccessArray;
    private NativeArray<EnemyData> enemyDatas;
    GameObject player;

        

    private struct EnemyData
    {
        public float3 position;
        public float3 velocity;
        public float3 target;
        public float speed;
        public float minDistance;
    }


    private void Start()
    {
        SpawnManager.Instance.OnEnemiesSpawned += Initialize;
        isPlaying = false;
    }

    bool isPlaying;
    public void Initialize()
    {
        listEnemy = SpawnManager.Instance.objects;
        player = GameObject.FindGameObjectWithTag("Player");
        var enemyCount = listEnemy.Count;
        transformAccessArray = new TransformAccessArray(enemyCount);
        enemyDatas = new NativeArray<EnemyData>(enemyCount, Allocator.Persistent);

        for (int i = 0; i < enemyCount; i++)
        {
            float speed = Random.Range(1f, 5f);
            transformAccessArray.Add(listEnemy[i].transform);
            listEnemy[i].OnSlow += OnEnemySlow;
            enemyDatas[i] = new EnemyData
            {
                position = listEnemy[i].transform.position,
                velocity = listEnemy[i].transform.forward,
                speed = listEnemy[i].speed,
                target = player.transform.position,
                minDistance = listEnemy[i].minDis
            };
        }
        isPlaying = true;

    }

    private void OnEnemySlow(Enemy obj)
    {
        var tmp = enemyDatas[listEnemy.IndexOf(obj)];
        tmp.speed = obj.speed;
        enemyDatas[listEnemy.IndexOf(obj)] =tmp;

        //enemyDatas[]
    }

    public float detectionRadius;
    public LayerMask enemyLayer;
    private void Update()
    {
        if (isPlaying)
        {
            var enemyMovementJob = new EnemyMovementJob
            {
                enemies = enemyDatas,
                deltaTime = Time.deltaTime,
                pos = player.transform.position,
            };

            JobHandle enemyMovementJobHandle = enemyMovementJob.Schedule(transformAccessArray);
            enemyMovementJobHandle.Complete();
        }
        else return;

    }

    [BurstCompile]
    private struct EnemyMovementJob : IJobParallelForTransform
    {
        public NativeArray<EnemyData> enemies;
        public float deltaTime;
        public float speed;
        public float detectionRadius;
        public LayerMask enemyLayer;
        public Vector3 pos;
        public void Execute(int index, TransformAccess transform)
        {
            Vector3 position = enemies[index].position;
            Vector3 target = enemies[index].target;
            Vector3 direction = target - position;
            float distace = Vector3.Distance(pos, position);
            float spd = enemies[index].speed;
            float minDis = enemies[index].minDistance;
            if (distace <= minDis) 
            {
                spd = 0;
            }
          
            transform.position = Vector3.MoveTowards(transform.position, target, spd * deltaTime);



            Quaternion targetRotation = Quaternion.LookRotation(target - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 8 * deltaTime);

                enemies[index] = new EnemyData
                {
                    position = transform.position,
                    velocity = direction,
                    speed = spd,
                    target = pos,
                    minDistance = minDis,
                };
        }
    }

    private void OnDestroy()
    {
        transformAccessArray.Dispose();
        enemyDatas.Dispose();
    }

}
