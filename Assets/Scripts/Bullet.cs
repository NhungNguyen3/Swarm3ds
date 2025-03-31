
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class BulletObject : MonoBehaviour
{
    private IObjectPool<BulletObject> objectPool;
    public IObjectPool<BulletObject> ObjectPool { set => objectPool = value; }

    public Gun gun;

    public Vector3 pos;
    public float speed = 20f;
    public Rigidbody rb;
    public CapsuleCollider sl;
    public float maxDistance;
    public float lifeTime = 4f;
    float timer;
    bool collide = false;

    [SerializeField] BulletType bulletType;
    [SerializeField] float explodeRadius;

    public GameObject model;
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
        timer = lifeTime;
    }

    bool isExplode = false;
    private void Update()
    {
        /*        float distance = Vector2.Distance(transform.position, pos);
                if(distance > maxDistance) objectPool.Release(this);*/
        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            if(this.bulletType == BulletType.Rocket)
            {
                if (!isExplode)
                {
                    isExplode = true;
                    StartCoroutine(Explode());
                }
                else return;
            }
            else
            {
                objectPool.Release(this);
                timer = lifeTime;
            }

        }
    }

    public float detectionRadius = 5f;
    public LayerMask enemyLayer;
    public ParticleSystem explodeFx;
    public float bulletDmg = 3f;
    IEnumerator Explode()
    {
        SoundManager.Instance.PlaySound(SoundName.Explosion_1, 1f);
        rb.isKinematic = false;
        model.SetActive(false);
        explodeFx.gameObject.SetActive(true);
       // explodeFx.Play();
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionRadius, enemyLayer);
        float closestDistance = detectionRadius;
        foreach (Collider col in hitColliders)
        {
            IDamageable damageable = col.GetComponent<IDamageable>();
            if(damageable!=null)
            {
                damageable.Damage(bulletDmg);
            }
        }
        yield return new WaitForSeconds(1f);
        explodeFx.gameObject.SetActive(false);
        rb.isKinematic = true;
        timer = lifeTime;
        objectPool.Release(this);
        model.SetActive(true);
        isExplode = false;
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

            if (collision.TryGetComponent<IDamageable>(out var damageable))
            {
                damageable.Damage(bulletDmg);
                Debug.Log("====" + collision.gameObject.name);
            }
            timer = lifeTime;
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