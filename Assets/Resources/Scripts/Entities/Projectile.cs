// Write projectile class dictating movement mainly

using UnityEngine;

public class Projectile : Interactable
{
    public float speed;
    public Vector2 direction;
    public float lifeTime;
    public float damage;
    public bool isHoming;
    public float homingStrength;
    public float rotationOffset;
    public User homingTarget;
    
    protected override void Awake()
    {
        base.Awake();
        direction = direction.normalized;
        transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + rotationOffset);
        Destroy(gameObject, lifeTime);
    }
    
    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        if (isHoming)
        {
            Vector2 directionToPlayer = (Vector2)homingTarget.transform.position - (Vector2)transform.position;
            direction = Vector2.Lerp(direction, directionToPlayer.normalized, homingStrength * Time.deltaTime).normalized;
        }
        transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + rotationOffset);
        transform.Translate(direction * (speed * Time.deltaTime), Space.World);
    }

    public override void Interact(User user)
    {
        // Edit with damage
        user.Die();
        Destroy(gameObject);
    }
}