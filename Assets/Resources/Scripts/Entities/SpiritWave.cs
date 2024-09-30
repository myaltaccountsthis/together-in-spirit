using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiritWave : DamagePart
{
    private const float LIFETIME = 5f;
    private const float ANIMATION_FRAME_DELAY = .2f;

    public Sprite[] sprites;
    private SpriteRenderer spriteRenderer;
    private int animIndex;

    protected override void Awake() {
        Destroy(gameObject, LIFETIME);
        spriteRenderer = GetComponent<SpriteRenderer>();
        animIndex = 0;
    }

    protected override void Update() {
        if (Time.time % ANIMATION_FRAME_DELAY < Time.deltaTime) {
            animIndex = (animIndex + 1) % sprites.Length;
            spriteRenderer.sprite = sprites[animIndex];
        }
    }

    public void SetDirection(Vector2 direction)
    {
        transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
        GetComponent<Rigidbody2D>().velocity = direction;
    }

    public override void Interact(User user) {}

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        base.OnTriggerEnter2D(other);
        // Can only damage player
        if (other.TryGetComponent(out Player user))
        {
            user.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}
