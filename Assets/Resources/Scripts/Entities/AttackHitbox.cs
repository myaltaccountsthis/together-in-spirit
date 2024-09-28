using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackHitbox : Entity
{
    protected Collider2D hitbox;
    [SerializeField] protected int damage;
    [SerializeField] protected float lifeTime;
    [SerializeField] private float distanceOffset;
    protected float timeLeft;
    protected float direction;

    protected override void Awake()
    {
        base.Awake();
        timeLeft = lifeTime;
        hitbox = GetComponent<Collider2D>();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        timeLeft -= Time.deltaTime;
        if (timeLeft <= 0)
        {
            Destroy(gameObject);
        }
    }

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        base.OnTriggerEnter2D(other);
        if (other.TryGetComponent(out Enemy enemy))
        {
            enemy.TakeDamage(damage);
        }
    }

    // Direction is in radians (unit circle)
    public void SetDirection(float direction)
    {
        this.direction = direction;
        transform.rotation = Quaternion.Euler(0, 0, -(90 - direction * Mathf.Rad2Deg));
    }

    public float getDistanceOffset()
    {
        return distanceOffset;
    }
}
