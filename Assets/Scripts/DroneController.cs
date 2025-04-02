using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;


public class DroneController : MonoBehaviour
{
    public Gun gun;

    public float attackSpeed = 3f;
    public float detectionRadius = 50f;
    public LayerMask enemyLayer;
    public bool canAttack;

    public float shieldCD = 6f;
    public float healthCD = 4f;

    public static event Action<float> OnActiveShield;

    float timer;
    float timerShield;
    float timerHealth;

    private void Update()
    {
        timer -= Time.deltaTime;
        timerShield -= Time.deltaTime;
        timerHealth -= Time.deltaTime;

        if (timer <= 0)
        {
            timer = attackSpeed;
            canAttack = true;
            FindClosetEnemy();
        }

        if (timerShield <= 0)
        {
            timerShield = shieldCD;
            CreateShield();
        }
        
        if (timerHealth <= 0)
        {
            timerHealth = healthCD;
        }
    }


    #region Attack
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
           // SoundManager.Instance.PlaySound(SoundName.Gun_Shoot_1, .2f);
            StartCoroutine(gun.DroneShoot(this.transform, pos));
            canAttack = false;
        }
        yield return null;
    }
    #endregion

    #region Shield
    public float shieldRatio;

    public void CreateShield()
    {
        OnActiveShield?.Invoke(shieldRatio);
    }
    #endregion

    #region Health

    #endregion

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(this.transform.position, detectionRadius);
    }
}
