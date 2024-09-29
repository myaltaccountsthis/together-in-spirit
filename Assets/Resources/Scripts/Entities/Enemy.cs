using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : LivingEntity
{
    [SerializeField] protected int damage;
    [SerializeField] protected float speed;
    [SerializeField] protected int score;
    protected bool active;
    private Player player;
    private Spirit spirit;
    private Animator animator;

    protected override void Awake()
    {
        base.Awake();
        active = true;
        CameraSystem cameraSystem = Camera.main.GetComponent<CameraSystem>();
        player = cameraSystem.player;
        spirit = cameraSystem.spirit;
        animator = GetComponent<Animator>();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        if (active)
        {
            Move();
        }
        else {
            animator.SetFloat("Horizontal", 0f);
            animator.SetFloat("Vertical", 0f);
        }
    }

    private Vector2 GetTargetPosition() {
        Vector2 closerPos = player.transform.position;
        if (spirit.gameObject.activeSelf && (transform.position - spirit.transform.position).magnitude < (transform.position - player.transform.position).magnitude)
        {
            closerPos = spirit.transform.position;
        }
        return closerPos;
    }

    private void Move()
    {
        Vector2 closerPos = GetTargetPosition();
        rigidbody.position = Vector2.MoveTowards(transform.position, closerPos, speed * Time.deltaTime);
        Vector2 movement = closerPos - (Vector2)transform.position;
        animator.SetFloat("Horizontal", movement.x);
        animator.SetFloat("Vertical", movement.y);
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
        base.Die();
        GameObject.FindWithTag("GameController").GetComponent<DataManager>().currentData.score += score;
        Destroy(gameObject);
    }
}
