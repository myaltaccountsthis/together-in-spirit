using System.Collections;
using UnityEngine;

public class FiringDevice : Entity
{   
    public Projectile projectilePrefab;
    public Vector2 fixedDirection;
    public bool isEnabled = true;
    public float delay;
    public bool isHoming;
    public User homingTarget;
    public AudioSource fireSound;
    
    [SerializeField] private Sprite inactiveSprite;
    [SerializeField] private Sprite readySprite;
    [SerializeField] private Sprite shootingSprite;
    private Sprite normalSprite;
    private SpriteRenderer spriteRenderer;
    private Coroutine coroutine;
    
    
    protected override void Awake()
    {
        base.Awake();
        fixedDirection = fixedDirection.normalized;
    }

    protected override void Start()
    {
        base.Start();
        spriteRenderer = GetComponent<SpriteRenderer>();
        normalSprite = spriteRenderer.sprite;
        coroutine = StartCoroutine(firing());
    }

    IEnumerator firing()
    {
        while (true)
        {
            if (enabled)
                spriteRenderer.sprite = readySprite;
            yield return new WaitForSeconds(delay / 4);
            if (!enabled) continue;
            projectilePrefab.isHoming = isHoming;
            projectilePrefab.homingTarget = homingTarget;
            projectilePrefab.direction = fixedDirection;
            Projectile proj = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
            AudioSource audio = Instantiate(fireSound, transform.position, Quaternion.identity);
            audio.Play();
            Destroy(audio.gameObject, audio.clip.length);
            spriteRenderer.sprite = shootingSprite;
            yield return new WaitForSeconds(delay / 2);
            spriteRenderer.sprite = normalSprite;
            yield return new WaitForSeconds(delay / 4);
        }
    }

    public void SetEnabled(bool enabled)
    {
        this.isEnabled = enabled;
        if (!enabled) {
            spriteRenderer.sprite = inactiveSprite;
            StopCoroutine(coroutine);
        }
    }
}