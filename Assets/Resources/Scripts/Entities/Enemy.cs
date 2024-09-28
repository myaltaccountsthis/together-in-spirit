using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : LivingEntity
{
    [SerializeField] protected int damage;
    [SerializeField] protected float speed;
    protected bool active;
    [SerializeField] protected Player player;
    [SerializeField] protected Spirit spirit;

    protected override void Awake()
    {
        base.Awake();
        active = true;
        player = FindObjectOfType<Player>();
        spirit = FindObjectOfType<Spirit>();
    }

    protected override void Update()
    {
        base.Update();
        if (active)
        {
            Move();
        }
    }

    private void Move()
    {
        Vector2 closerPos = player.transform.position;
        if ((transform.position - spirit.transform.position).magnitude < (transform.position - player.transform.position).magnitude)
        {
            closerPos = spirit.transform.position;
        }
        transform.position = Vector2.MoveTowards(transform.position, closerPos, speed * Time.deltaTime);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent(out User user))
        {
            user.TakeDamage(damage);
            user.Knockback(transform.position);
        }
    }

    public override void Die()
    {
        Destroy(gameObject);
    }
}
