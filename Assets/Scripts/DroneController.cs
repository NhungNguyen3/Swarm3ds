using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


public class DroneController : MonoBehaviour
{
    public Gun gun;

    public float attackSpeed = 3f;
    float timer;
    public float detectionRadius = 50f;
    public LayerMask enemyLayer;
    public bool canAttack;

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
            this.transform.DOLookAt(closestEnemy.position, 0f);
            StartCoroutine(Attack(closestEnemy.position));
        }
        else return;

    }

    public IEnumerator Attack(Vector3 pos)
    {
        if(canAttack)
        {
            StartCoroutine(gun.DroneShoot(this.transform, pos));
            canAttack = false;
        }
        yield return null;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(this.transform.position, detectionRadius);
    }
}
