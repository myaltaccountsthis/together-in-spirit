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
    
    
    protected override void Awake()
    {
        StartCoroutine(firing());
        fixedDirection = fixedDirection.normalized;
    }

    IEnumerator firing()
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            if (!enabled) continue;
            projectilePrefab.isHoming = isHoming;
            projectilePrefab.homingTarget = homingTarget;
            projectilePrefab.direction = fixedDirection;
            Projectile proj = Instantiate(projectilePrefab, transform.position, Quaternion.identity);

        }
    }

    public void SetEnabled(bool enabled)
    {
        this.isEnabled = enabled;
    }
}