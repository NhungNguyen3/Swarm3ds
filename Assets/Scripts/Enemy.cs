
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Jobs;
using UnityEngine.Pool;
using UnityEngine.UIElements;

public class Enemy : MonoBehaviour, IDamageable
{
    public float speed;
    public Rigidbody rb;
    public GameObject playerHead;
    public GameObject player;
    public GameObject firePoint;
    public float hp;
    public float maxHP = 10;
    public float attackRange = 3f;
    private IObjectPool<Enemy> objectPool;
    public IObjectPool<Enemy> ObjectPool { set => objectPool = value; }

    public EnemyType type;
    public GameObject meleeModel;
    public GameObject rangeModel;
    public float minDis;
    public float detectionRadius;
    public LayerMask enemyLayer;
    public event System.Action<Enemy> OnSlow;
    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        hp = maxHP;
    }


    public void SetSkin()
    {
        switch (type)
        {
            case EnemyType.Melee:
                meleeModel.SetActive(true);
                minDis = 2f;
                speed = 4f;
                break;
            case EnemyType.Range:
                rangeModel.SetActive(true);
                speed = 2f;
                minDis = 5f;
                break;
        }
    }

    public void Damage(float dmg)
    {
        GetDmg(dmg);
        /* Dead();*/

    }
    float timer;
    public float attackSpeed = 1f;
    bool canAttack = false;
    private void Update()
    {
        /*        MyJob move = new MyJob
                {
                    transform = this.transform.position,
                    target = player.transform.position,
                    spd = speed,
                    deltaTime = Time.deltaTime
                };*/

        //StartCoroutine(Move());
        timer -= Time.deltaTime;
        if (timer <= 0)
        {

            canAttack = true;
           // StartCoroutine(ShootBulletRocket(player.transform.position));
        }
        float distace = Vector3.Distance(player.transform.position, this.transform.position);
        if (distace <= attackRange && canAttack && type == EnemyType.Range)
        {
            timer = attackSpeed;
            canAttack = false;
            StartCoroutine(ShootBulletRocket(playerHead.transform.position));
            /*speed = 0;

            IDamageable damageable = player.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.Damage(1);
            }
            Debug.Log("Attack");*/
            /*            Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionRadius, enemyLayer);

                        foreach (Collider col in hitColliders)
                        {
                            IDamageable damageable = col.GetComponent<IDamageable>();
                            if (damageable != null)
                            {
                                damageable.Damage(1);
                            }
                        }*/
        }
        else return;

    }


    IEnumerator Move()
    {
        float spd = speed;
        Vector3 target = player.transform.position;
        float distace = Vector3.Distance(this.transform.position, target);
        if (distace <= minDis)
        {
            spd = 0;
        }
        else spd = speed;
        this.transform.position = Vector3.MoveTowards(transform.position, target, spd * Time.deltaTime);

        Quaternion targetRotation = Quaternion.LookRotation(target - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 8 * Time.deltaTime);
        yield return null;  
    }



    [BurstCompile]
    public struct MyJob : IJob
    {
        public Vector3 transform;
        public Vector3 target;
        public float spd;
        public float deltaTime;

        public void Execute()
        {
            transform = Vector3.MoveTowards(transform, target, spd * deltaTime);
        }
    }



    public IEnumerator ShootBulletRocket(Vector3 pos)
    {
        BulletObject bulletObject = BulletsManager.Instance.objectPoolEnemy.Get();
        bulletObject.transform.SetLocalPositionAndRotation(firePoint.transform.position, Quaternion.Euler(0, 0, 0));
        Rigidbody rb = bulletObject.GetComponent<Rigidbody>();

        bulletObject.transform.DOMove(pos, 1f);
        bulletObject.transform.DOLookAt(pos, 0f);
        yield return null;
    }

    public void GetDmg(float dmg)
    {
        hp -= dmg;
/*        speed -= .5f;
        OnSlow?.Invoke(this);*/
        if (hp <= 0)
        {
            if (!isDead)
            {
                isDead = true;
                StartCoroutine(Dead());
            }
            else return;
        }
    }
    bool isDead = false;
    public GameObject bloodFx;
    public GameObject model;
    public IEnumerator Dead()
    {
        Debug.Log("Dead");
        SoundManager.Instance.PlaySound(SoundName.Blood_Splat_1, 1f);
        bloodFx.SetActive(true);
        model.SetActive(false);
        yield return new WaitForSeconds(1f);
        Vector3 position = new Vector3(Random.Range(-100f, 100f),0, Random.Range(-100f, 100f));
        this.transform.position = position;
        bloodFx.SetActive(false);
        model.SetActive(true);
        hp = maxHP + 5;
        isDead = false;
    }
}

public enum EnemyType
{
    Melee,
    Range
}
