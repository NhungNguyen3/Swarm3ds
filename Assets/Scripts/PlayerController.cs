
using UnityEngine;
using Unity.Collections;
using System.Collections;
using UnityEngine.EventSystems;
using DG.Tweening;
using Unity.Burst;

public class PlayerController : MonoBehaviour
{
    public float detectionRadius = 50f;
    public LayerMask enemyLayer;

    public float speed;
    public VariableJoystick variableJoystick;
   public Rigidbody rb;

    public Transform firePoint;
    public float attackSpeed = .2f;
    public Bullet bullet;
    public Gun gun;
    float timer;
    public bool isLookAtEnemy = false;
    public GameObject cursor;


    private void OnTriggerEnter(Collider collision)
    {
        IInteractable interactable = collision.GetComponent<IInteractable>();
        if(interactable != null)
        {
            interactable.Pickup();
        }    
    }
    public bool canAttack;

    public void FixedUpdate()
    {
        Vector3 direction = Vector3.forward * variableJoystick.Vertical * speed + Vector3.right * variableJoystick.Horizontal *speed;
        rb.velocity = direction;

        //transform.DOMove(direction, 1f);

        this.transform.rotation = Quaternion.LookRotation(direction);
        //rb.AddForce(direction * speed * Time.fixedDeltaTime, ForceMode.VelocityChange);
        /*        if (variableJoystick.Horizontal != 0 || variableJoystick.Vertical != 0)
                {
                    if(!isLookAtEnemy)
                }*/

        //   gun.transform.up = rb.velocity;

        /* if(variableJoystick.Horizontal < 0) transform.rotation = Quaternion.Euler(0,180,0);
         else if (variableJoystick.Horizontal > 0) transform.rotation = Quaternion.Euler(0, 0, 0);*/
    }

    private void Update()
    {

        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            timer = attackSpeed;
            canAttack = true;
            FindClosetEnemy();
        }
    }

    

    Transform closestEnemy = null; 
    [BurstCompile]
    public void FindClosetEnemy()
    {

        Transform closestEnemy = null;
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionRadius, enemyLayer);
        float closestDistance = detectionRadius;



        foreach (Collider col in hitColliders)
        {
            float distance = Vector2.Distance(transform.position, col.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestEnemy = col.transform;
            }
        }

        if (closestEnemy != null)
        {
            Fire(closestEnemy.position);
            Debug.Log(closestEnemy.transform.position);
        }
        else return;

    }

    IEnumerator AttackAnim()
    {
        isLookAtEnemy = true;
        Fire(closestEnemy.position);

        timer = attackSpeed;

        yield return new WaitForSeconds(2f);
        isLookAtEnemy = false;
        closestEnemy = null;
    }


    public void Fire(Vector3 pos)
    {
        if (canAttack)
        {
            StartCoroutine(gun.ShootBulletRocket(pos));
            canAttack = false;
        }
        else return;
        /*        var nbullet = LeanPool.Spawn(bullet);
                nbullet.transform.position = firePoint.position;
                nbullet.pos = firePoint.position;
                nbullet.Target(pos);*/
        // gun.Fire(pos);
      // StartCoroutine(gun.ShootBullets1(this.transform,pos));
       //  StartCoroutine(gun.Shooting2());
       //  StartCoroutine(gun.Fire(pos));
        
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(this.transform.position, detectionRadius);
    }
}
