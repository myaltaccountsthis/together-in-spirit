using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class LivingEntity : Entity
{
    [SerializeField] protected int maxHealth;
    protected int health;
    protected Collider2D hitbox;
    protected new Rigidbody2D rigidbody;

    protected override void Awake()
    {
        health = maxHealth;
        hitbox = GetComponent<Collider2D>();
        rigidbody = GetComponent<Rigidbody2D>();
    }

    public virtual void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Die();
        }
    }

    public abstract void Die();
}
