using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class LivingEntity : Entity
{
    [SerializeField] protected int maxHealth;
    protected int health;
    protected Collider2D hitbox;
    protected new Rigidbody2D rigidbody;
    private HealthBar healthBar;
    [SerializeField] private float healthBarScale = 1f;

    protected override void Awake()
    {
        base.Awake();
        hitbox = GetComponent<Collider2D>();
        rigidbody = GetComponent<Rigidbody2D>();
        healthBar = Instantiate(Resources.Load<HealthBar>("Prefabs/HealthBar"), GameObject.Find("WorldCanvas").transform);
        healthBar.attachment = transform;
        healthBar.scale = healthBarScale;
        healthBar.gameObject.SetActive(false);
    }

    protected override void Start()
    {
        base.Start();
        health = maxHealth;
        healthBar.Health = 1f;
    }

    public virtual void TakeDamage(int damage)
    {
        health -= damage;
        healthBar.Health = (float)health / maxHealth;
        healthBar.gameObject.SetActive(true);
        if (health <= 0)
        {
            Destroy(healthBar.gameObject);
            Die();
        }
    }

    public abstract void Die();
}
