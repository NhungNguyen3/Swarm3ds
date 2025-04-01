
using UnityEngine;
using Unity.Collections;
using System.Collections;
using UnityEngine.EventSystems;
using DG.Tweening;
using Unity.Burst;

public class PlayerController : MonoBehaviour, IDamageable
{
    public float detectionRadius = 50f;
    public LayerMask enemyLayer;

    public float speed;
    public VariableJoystick variableJoystick;
   public Rigidbody rb;

    public Transform firePoint;
    public float attackSpeed = .05f;
    public Gun gun;
    float timer;
    public bool isLookAtEnemy = false;
    public GameObject cursor;

    public AnimationController1 playerAnimController;
    public GameObject model;
    public GameObject drone;

    public float hpShield = 50f;
    private void Start()
    {
        hp = maxHP;
        isPlaying = false;
    }

    bool isPlaying;
    public void SetSkin(GameObject character)
    {
        var player = Instantiate(character,model.transform);
        playerAnimController = player.GetComponent<AnimationController1>();
        gun.gameObject.SetActive(true);
        drone.SetActive(true);
        shieldFx.SetActive(true);
        isPlaying = true;
    }

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
        if (!isPlaying) return;
        else
        {
            Vector3 direction = Vector3.forward * variableJoystick.Vertical * speed + Vector3.right * variableJoystick.Horizontal * speed;
            rb.velocity = direction;

            //transform.DOMove(direction, 1f);
            if (variableJoystick.Horizontal != 0 || variableJoystick.Horizontal != 0)
            {
                playerAnimController.PlayAnimation("isMoving", true);
            }
            else
            {
                playerAnimController.PlayAnimation("isMoving", false);
            }

            this.transform.rotation = Quaternion.LookRotation(direction);
            //rb.AddForce(direction * speed * Time.fixedDeltaTime, ForceMode.VelocityChange);
            /*        if (variableJoystick.Horizontal != 0 || variableJoystick.Vertical != 0)
                    {
                        if(!isLookAtEnemy)
                    }*/

            //   gun.transform.up = rb.velocity;

        }
    }


    public void GetHP(float value)
    {
        hp += value;
        if (hp >= maxHP) hp = maxHP;
    }


    public void Damage(float dmg)
    {
        Debug.Log("====TAKE DMG");
        GetDmg(dmg);
        /* Dead();*/

    }
    
    public float hp;
    public float maxHP;
    public GameObject shieldFx;
    public void GetDmg(float dmg)
    {
        if(hpShield >=0 )
        {
            hpShield -= dmg;
        }
        else
        {
            shieldFx.SetActive(false);
            hp -= dmg;
            if(hp<=0 )
            {
                Debug.Log(" YOU DEad");
            }
        }

    }



    public IEnumerator ShieldRegen()
    {
        yield return new WaitForSeconds(4f);
        shieldFx.SetActive(true);
        hpShield = 50f;
    }
    float timer2 = 2f;
    bool canRocket = false;
    private void Update()
    {
        timer -= Time.deltaTime;
        timer2 -= Time.deltaTime;
        if(timer2 <= 0)
        {
            timer2 = 2f;
            canRocket = true;
            FindClosetEnemy2();
        }
        if (timer <= 0)
        {
            timer = attackSpeed;
            canAttack = true;
            FindClosetEnemy();
        }
    }

    public void Gun2()
    {
        StartCoroutine(gun.FireBreath());
    }
    
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
            gun.transform.DOLookAt(closestEnemy.position, 0f);
            Fire(closestEnemy.position);
        }
        else return;

    }


    [BurstCompile]
    public void FindClosetEnemy2()
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
            Fire2(closestEnemy.position);
        }
        else return;

    }


    public void Fire2(Vector3 pos)
    {
        if (canRocket)
        {

            StartCoroutine(gun.ShootBulletRocket(pos));
            canRocket = false;
        }
        else return;
    }
    public void Fire(Vector3 pos)
    {
        if (canAttack)
        {
            StartCoroutine(gun.Shooting2());
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
