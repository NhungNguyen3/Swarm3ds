using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableItem : MonoBehaviour, IInteractable
{
    public InteractableItemType type;
    [Header("Powder Keg")]
    public ParticleSystem explodeFx;
    public float detectionRadius = 7f;
    public LayerMask enemyLayer;
    public GameObject model;
    public CapsuleCollider cl;
    public void Pickup()
    {
        switch (type)
        {
            case InteractableItemType.AmmoBox:
                Debug.Log("AMMO BOX");
                StartCoroutine(Respawn(false));
                gameObject.SetActive(false);
                break;
            case InteractableItemType.PowerKeg:
                Debug.Log("POWDER KEG");
                StartCoroutine(Explode());
                break;
            case InteractableItemType.HealthCollectible:
                Debug.Log("HEALTH COLLECTIBLE");
                StartCoroutine(Respawn(false));
                break;
        }
    }


    public IEnumerator Explode()
    {
        explodeFx.gameObject.SetActive(true);
        model.SetActive(false);
        cl.enabled = false;
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionRadius, enemyLayer);
        float closestDistance = detectionRadius;

        foreach (Collider col in hitColliders)
        {
            IDamageable damageable = col.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.Damage();
            }
        }
        yield return new WaitForSeconds(2.5f);
        StartCoroutine(Respawn(true));
    }

    public IEnumerator Respawn(bool isExploded)
    {
        yield return new WaitForEndOfFrame();
        Vector3 position = new Vector3(Random.Range(-50f, 50f),0, Random.Range(-50f, 50f));
        this.transform.position = position;
        model.SetActive(true);
        if(isExploded)
        {
            explodeFx.gameObject.SetActive(false);
            cl.enabled=true;
        }

    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(this.transform.position, detectionRadius);
    }
    /*    private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision != null)
            {
                if (collision.gameObject.CompareTag("Player"))
                {
                    this.gameObject.SetActive(false);
                }

            }
        }*/

    /*    private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision != null)
            {
                if (collision.gameObject.CompareTag("Player"))
                    Destroy(this.gameObject);
            }
        }*/
}

public enum InteractableItemType
{
    AmmoBox,
    PowerKeg,
    HealthCollectible
}