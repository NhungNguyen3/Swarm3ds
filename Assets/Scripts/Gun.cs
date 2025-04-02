using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Jobs;
using UnityEngine.Pool;
using Random = UnityEngine.Random;
using DG.Tweening;
using static UnityEngine.GraphicsBuffer;

public class Gun : MonoBehaviour
{
    public int numberOfBullets = 8; // Number of bullets to shoot
    public float angleBetweenBullets = 45f; // Angle between each bullet in degrees
    public float bulletSpeed = 10f;
    public int bulletCount = 47;

    public int bulletCount2 = 4;
    public float bulletSubSpeed = 20f;
    public GameObject subGun1;
    public GameObject subGun2;
    public GameObject subGun3;
    public GameObject subGun4;

    public GameObject gunRocket;


    //Rays Gun
    [BurstCompile]
    public IEnumerator ShootBullets2(GameObject subGun)
    {
        var bullet = BulletsManager.Instance.objectPool.Get();
        bullet.transform.SetLocalPositionAndRotation(subGun.transform.position, Quaternion.Euler(0, 0, 0));
        Vector3 direction = subGun.transform.forward;

        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        rb.velocity = direction * bulletSubSpeed;

        yield return new WaitForSeconds(0.05f);
    }

    public IEnumerator Shooting2()
    {
        SoundManager.Instance.PlaySound(SoundName.Gun_Shoot_2, .4f);
        StartCoroutine(ShootBullets2(subGun1));
        StartCoroutine(ShootBullets2(subGun2));
        StartCoroutine(ShootBullets2(subGun3));
        StartCoroutine(ShootBullets2(subGun4));
        yield return null;
    }

    //Rocket Gun
    [BurstCompile]
    public IEnumerator ShootBulletRocket(Vector3 pos)
    {
        yield return new WaitForSeconds(2f);
        SoundManager.Instance.PlaySound(SoundName.Missile_Shoot_1, .8f);
        BulletObject bulletObject = BulletsManager.Instance.objectPoolRocket.Get();
        bulletObject.transform.SetLocalPositionAndRotation(gunRocket.transform.position, Quaternion.Euler(0, 0, 0));
        Rigidbody rb = bulletObject.GetComponent<Rigidbody>();

        bulletObject.sl.enabled = false;

        bulletObject.transform.DOMove(pos, 1f);
        bulletObject.transform.DOLookAt(pos, 0f);
        yield return null;
    }

    //Drone Gun
    public IEnumerator DroneShoot(Transform firePoint, Vector3 target)
    {
        var bullet = BulletsManager.Instance.objectPoolAngle.Get();
        SoundManager.Instance.PlaySound(SoundName.Gun_Shoot_3, 1f);
        bullet.transform.SetLocalPositionAndRotation(firePoint.transform.position, Quaternion.Euler(0, 0, 0));
        bullet.transform.rotation = firePoint.rotation;
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        bullet.transform.DOMove(target, .5f);
        bullet.transform.DOLookAt(target, 0f);
        yield return new WaitForSeconds(0.05f);
    }


    //Angle Gun
    public IEnumerator ShootingNormal(Transform firePoint)
    {

        var bullet = BulletsManager.Instance.objectPoolAngle.Get();

        bullet.transform.SetLocalPositionAndRotation(firePoint.transform.position, Quaternion.Euler(0, 0, 0));
        bullet.transform.rotation = firePoint.rotation;
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        // rb.AddForce(rb.velocity * bulletSpeed);
        rb.velocity = firePoint.forward * bulletSpeed;

        transform.up = rb.velocity;
        yield return new WaitForSeconds(0.05f);
    }


    //FireBreath Gun
    public float detectionRadius = 50f;
    public GameObject point2;
    public LayerMask enemyLayer;

    public int timeEnable = 4;
    public GameObject fireVfx;
    public float fireDmg = 2f;
    public IEnumerator FireBreath()
    {
        fireVfx.SetActive(true);
        for(int i = 0; i < timeEnable; i++)
        {
            Collider[] hitColliders = Physics.OverlapCapsule(transform.position, point2.transform.position, detectionRadius, enemyLayer);

            foreach (Collider col in hitColliders)
            {
                IDamageable damageable = col.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    damageable.Damage(fireDmg);
                }
            }

            yield return new WaitForSeconds(.2f);
        }
        yield return new WaitForEndOfFrame();
        fireVfx.SetActive(false);
    }
}


public enum GunType
{
    Shortgun,
    Shortgun_Meteor,
    Rocket_Barrage,
    Flamethrower_Inferno,
}