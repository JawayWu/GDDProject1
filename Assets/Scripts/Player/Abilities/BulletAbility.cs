using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletAbility : Ability
{
    [SerializeField]
    private float m_Speed;
    [SerializeField]
    private ParticleSystem m_DeathExplosion;

    private Rigidbody cc_Rb;

    private void Awake()
    {
        cc_Rb = GetComponent<Rigidbody>();
    }

    private void OnTriggerEnter(Collider other) // other being the object colliding with it
    {
        if(other.CompareTag("Player"))
        {
            other.GetComponent<PlayerController>().DecreaseHealth(Power);
        }
        if (other.CompareTag("Enemy"))
        {
            other.GetComponent<EnemyController>().DecreaseHealth(Power);
        }
        if (other.CompareTag("Ranged"))
        {
            other.GetComponent<RangedController>().DecreaseHealth(Power);
        }

        ParticleSystem ps = Instantiate(m_DeathExplosion, transform.position, Quaternion.identity);
        Destroy(ps.gameObject, 3f); // 3f = 3 seconds
        Destroy(gameObject, 0.1f);
    }

    public override void Use(Vector3 playerPos, Vector3 hitPos)
    {
        cc_Rb.velocity = transform.forward * m_Speed;
    }
}
