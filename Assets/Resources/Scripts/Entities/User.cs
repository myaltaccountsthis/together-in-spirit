using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Base class for Player and Spirit, can interact
/// </summary>
public abstract class User : LivingEntity
{
    private const float TWEEN_DURATION = .3f;

    [SerializeField] protected float attackCooldown;
    protected float attackTimer;
    public RectTransform interactUI;
    protected HashSet<Interactable> currentInteractables;
    private Vector2 interactOpenPosition, interactClosedPosition;
    protected float lastAngle;

    protected override void Awake()
    {
        base.Awake();
        interactOpenPosition = interactUI.anchoredPosition;
        interactClosedPosition = new Vector2(interactOpenPosition.x, -interactUI.sizeDelta.y - interactOpenPosition.y - 10);
        attackTimer = 0;
        lastAngle = 0;
    }

    protected override void Start()
    {
        base.Start();
        currentInteractables = new();
        interactUI.anchoredPosition = interactClosedPosition;
    }

    protected override void Update()
    {
        base.Update();
        attackTimer = Mathf.Max(0, attackTimer - Time.deltaTime);
    }

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        base.OnTriggerEnter2D(other);
        if (other.TryGetComponent(out Interactable interactable) && CanInteractWith(interactable)) {
            if (interactable.AutoInteract) {
                interactable.Interact(this);
                return;
            }
            currentInteractables.Add(interactable);
            if (currentInteractables.Count == 1)
                ShowInteract();
        }
    }
    protected override void OnTriggerExit2D(Collider2D other)
    {
        base.OnTriggerExit2D(other);
        if (other.TryGetComponent(out Interactable interactable) && CanInteractWith(interactable) && !interactable.AutoInteract) {
            currentInteractables.Remove(interactable);
            if (currentInteractables.Count == 0)
                HideInteract();
        }
    }

    protected void ShowInteract() {
        LeanTween.move(interactUI, interactOpenPosition, TWEEN_DURATION).setEaseOutQuad();
    }
    protected void HideInteract() {
        LeanTween.move(interactUI, interactClosedPosition, TWEEN_DURATION).setEaseInQuad();
    }

    public void Interact(InputAction.CallbackContext context) {
        if (!context.performed)
            return;
        if (currentInteractables.Count > 0) {
            Interactable first = currentInteractables.First();
            first.Interact(this);
            currentInteractables.Remove(first);
            HideInteract();
        }
    }

    protected abstract bool CanInteractWith(Interactable interactable);

    public override void TakeDamage(int damage) {
        base.TakeDamage(damage);
        // Do some post processing vignette
    }

    public virtual bool Attack() {
        if (attackTimer > 0)
            return false;
        attackTimer = attackCooldown;
        return true;
    }

    public void DoAttack(InputAction.CallbackContext context) {
        if (!context.performed)
            return;
        Attack();
    }

    public void Knockback(Vector3 origin) {
        Vector2 direction = (transform.position - origin).normalized;
        rigidbody.AddForce(direction * 2, ForceMode2D.Impulse);
    }

    public override void Die() {
        Debug.Log("Generic user died");
    }
}
