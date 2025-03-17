
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class Bullet : MonoBehaviour
{
    private IObjectPool<Bullet> objectPool;
    public IObjectPool<Bullet> ObjectPool { set => objectPool = value; }

    public Gun gun;

    public Vector3 pos;
    public float speed = 20f;
    public Rigidbody rb;
    public CapsuleCollider sl;
    public float maxDistance;
    public float lifeTime = 4f;
    bool collide = false;

    [SerializeField] BulletType bulletType;
    [SerializeField] float explodeRadius;
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    private void OnEnable()
    {
        collide = false;
        if (bulletType == BulletType.Rocket)
        {
            this.sl.enabled = false;
            this.rb.isKinematic = true;
        }

    }

    private void Update()
    {
/*        float distance = Vector2.Distance(transform.position, pos);
        if(distance > maxDistance) objectPool.Release(this);*/
        lifeTime-=Time.deltaTime;

        if (lifeTime <= 0)
        {
            if(this.bulletType == BulletType.Rocket)
            {
                StartCoroutine(Explode());
            }
            else
            {
                objectPool.Release(this);
                lifeTime = .5f;
            }

        }
    }

    public float detectionRadius = 5f;
    public LayerMask enemyLayer;
    public ParticleSystem explodeFx;
    IEnumerator Explode()
    {
        rb.isKinematic = false;
        explodeFx.gameObject.SetActive(true);
       // explodeFx.Play();
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionRadius, enemyLayer);
        float closestDistance = detectionRadius;

        foreach (Collider col in hitColliders)
        {
            IDamageable damageable = col.GetComponent<IDamageable>();
            if(damageable!=null)
            {
                damageable.Damage();
            }
        }
        yield return new WaitForSeconds(1f);
        explodeFx.gameObject.SetActive(false);
        rb.isKinematic = true;
        lifeTime = 1f;
        objectPool.Release(this);
    }

    public void Target(Vector3 target)
    {
        Vector3 dir = target - this.transform.position;
        rb.velocity = new Vector2(dir.x, dir.y).normalized * speed;
        transform.up = rb.velocity;
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject && !collide)
        {
            lifeTime = .5f;
            collide = true;
            objectPool.Release(this);
            // LeanPool.Despawn(this);
            //LeanPool.Despawn(this);
           // gun.bullets.Remove(this.transform);
        }         
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(this.transform.position, detectionRadius);
    }


}

public enum BulletType
{
    Normal, 
    Rocket
}