using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SpiritAttack : AttackHitbox
{
    [SerializeField] private float velocity;

    protected override void Update()
    {
        base.Update();
        transform.position += new Vector3(velocity * Time.deltaTime * Mathf.Cos(direction), velocity * Time.deltaTime * Mathf.Sin(direction), 0);
    }

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        base.OnTriggerEnter2D(other);
        if (other.TryGetComponent(out Enemy enemy))
        {
            enemy.TakeDamage(damage);
        }
        if (other.TryGetComponent(out Player player)) {
            return;
        }
        Destroy(gameObject);
    }

}
