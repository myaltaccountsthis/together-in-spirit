using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SpiritAttack : AttackHitbox
{
    const float animFrameTime = .15f;
    
    [SerializeField] private float velocity;

    public Sprite[] sprites;
    private SpriteRenderer spriteRenderer;
    private int animIndex;
    private float t;

    protected override void Awake()
    {
        base.Awake();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    protected override void Start()
    {
        base.Start();
        animIndex = 0;
        t = 0;
    }

    protected override void Update()
    {
        base.Update();
        transform.position += new Vector3(velocity * Time.deltaTime * Mathf.Cos(direction), velocity * Time.deltaTime * Mathf.Sin(direction), 0);
        t += Time.deltaTime;
        while (t >= animFrameTime) {
            t -= animFrameTime;
            animIndex = (animIndex + 1) % sprites.Length;
            spriteRenderer.sprite = sprites[animIndex];
        }
    }

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        base.OnTriggerEnter2D(other);
        if (other.TryGetComponent(out Player player)) {
            return;
        }
        Destroy(gameObject);
    }
}
