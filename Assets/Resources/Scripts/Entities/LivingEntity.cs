using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class LivingEntity : Entity
{
    [SerializeField] private int MaxHealth;
    public int Health { get; protected set; }
    public AudioClip damageSound;
    public AudioClip deathSound;
    public Collider2D hitbox;
    protected new Rigidbody2D rigidbody;
    private HealthBar healthBar;
    [SerializeField] private float healthBarScale = 1f;

    protected override void Awake()
    {
        base.Awake();
        hitbox = GetComponent<Collider2D>();
        rigidbody = GetComponent<Rigidbody2D>();
    }

    protected override void Start()
    {
        base.Start();
        healthBar = Instantiate(Resources.Load<HealthBar>("Prefabs/HealthBar"),
            GameObject.Find("WorldCanvas").transform);
        healthBar.attachment = transform;
        healthBar.scale = healthBarScale;
        Respawn();
    }

    public virtual void TakeDamage(int damage)
    {
        if (Health < 0)
            return;
        Health -= damage;
        healthBar.Health = (float)Health / MaxHealth;
        healthBar.gameObject.SetActive(true);
        AudioSource.PlayClipAtPoint(damageSound, transform.position);
        if (Health <= 0)
        {
            Health = -1;
            AudioSource.PlayClipAtPoint(deathSound, transform.position);
            Die();
        }
    }

    public virtual void Die()
    {
    }

    void OnDestroy()
    {
        if (healthBar != null)
            Destroy(healthBar.gameObject);
    }

    public virtual void Respawn()
    {
        healthBar.gameObject.SetActive(false);
        Health = MaxHealth;
        healthBar.Health = 1f;
    }

}
