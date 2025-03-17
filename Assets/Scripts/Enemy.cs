
using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using UnityEngine;
using UnityEngine.Jobs;
using UnityEngine.Pool;

public class Enemy : MonoBehaviour, IDamageable
{
    public float speed = 10f;
    public Rigidbody rb;
    GameObject player;
    float cd = 2f;
    bool isDead = false;

    private IObjectPool<Enemy> objectPool;
    public IObjectPool<Enemy> ObjectPool { set => objectPool = value; }
    private void Start()
    {
        
    }


    public void Damage()
    {

        Dead();

    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Projectile"))
        {
            Dead();
        }
    }

    public void Dead()
    {
        Debug.Log("Dead");
        Vector3 position = new Vector3(Random.Range(-100f, 100f),0, Random.Range(-100f, 100f));
        this.transform.position = position;
    }
}
