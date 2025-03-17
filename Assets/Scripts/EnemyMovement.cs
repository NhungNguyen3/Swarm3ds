
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
    [SerializeField] ListObjectVariable listEnemy;
    public float speedSpawn = .4f;
    float timer;
    public Enemy enemyPrefabs;
    public List<Enemy> enemies = new();
    public Transform spawnPoint;

    bool startSpawn = false;

    private TransformAccessArray transformAccessArray;
    private NativeArray<EnemyData> enemyDatas;
    GameObject player;

    [SerializeField] float minDistance = 2.4f;

    private struct EnemyData
    {
        public float3 position;
        public float3 velocity;
        public float3 target;  
    }


    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        var enemyCount = listEnemy.enemies.Count;
        transformAccessArray = new TransformAccessArray(enemyCount);
        enemyDatas = new NativeArray<EnemyData>(enemyCount, Allocator.Persistent);

        for (int i = 0; i < enemyCount; i++)
        {
            float speed = Random.Range(1f, 5f);
            transformAccessArray.Add(listEnemy.enemies[i].transform);
            enemyDatas[i] = new EnemyData
            {
                position = listEnemy.enemies[i].transform.position,
                velocity = listEnemy.enemies[i].transform.forward,
                target = player.transform.position,
            };
        }
    }

    private void Update()
    {

        var enemyMovementJob = new EnemyMovementJob
        {
            enemies = enemyDatas,
            deltaTime = Time.deltaTime,
            pos = player.transform.position,
            minDis = minDistance
            //speed = 2f
        };

        JobHandle enemyMovementJobHandle = enemyMovementJob.Schedule(transformAccessArray);
        enemyMovementJobHandle.Complete();
    }

    [BurstCompile]
    private struct EnemyMovementJob : IJobParallelForTransform
    {
        public NativeArray<EnemyData> enemies;
        public float deltaTime;
        public float speed;
        public float minDis;
        public Vector3 pos;
        public void Execute(int index, TransformAccess transform)
        {
            Vector3 position = enemies[index].position;
            Vector3 target = enemies[index].target;
            Vector3 direction = target - position;
            float distace = Vector3.Distance(pos, position);
            if (distace <= minDis) speed = 0;
            else speed = 4f;
            transform.position = Vector3.MoveTowards(transform.position, target, speed * deltaTime);



            Quaternion targetRotation = Quaternion.LookRotation(target - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 8 * deltaTime);

                enemies[index] = new EnemyData
                {
                    position = transform.position,
                    velocity = direction,
                    target = pos,
                };
        }
    }

    private void OnDestroy()
    {
        transformAccessArray.Dispose();
        enemyDatas.Dispose();
    }



/*    public void SpawnEnemies()
    {

        var newEnemy = LeanPool.Spawn(enemyPrefabs, spawnPoint);
        int x = Random.Range(-20, 20);
        int z = Random.Range(-20, 20);
        Vector3 pos = new(x, z, 0);

        newEnemy.transform.position = pos;

        enemies.Add(newEnemy);
    }

    public void OnClickStart()
    {
        startSpawn = !startSpawn;
    }*/
}
