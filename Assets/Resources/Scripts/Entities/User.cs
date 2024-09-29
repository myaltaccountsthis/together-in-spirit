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
    protected CameraSystem cameraSystem;
    protected Animator animator;

    private bool canMoveInternal = true;
    public bool CanMove { get => canMoveInternal; set {
        canMoveInternal = value;
        if (canMoveInternal) {
            HideInteract();
            rigidbody.constraints &= ~RigidbodyConstraints2D.FreezePositionX & ~RigidbodyConstraints2D.FreezePositionY;
        }
        else {
            rigidbody.constraints |= RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY;
        }
    } }
    protected float lastAngle;
    public DialogueManager dialogueManager;
    public AudioSource damageSound;
    
    protected override void Awake()
    {
        base.Awake();
        dialogueManager.gameObject.SetActive(true);
        interactOpenPosition = interactUI.anchoredPosition;
        interactClosedPosition = new Vector2(interactOpenPosition.x, -interactUI.sizeDelta.y - interactOpenPosition.y - 10);
        cameraSystem = Camera.main.GetComponent<CameraSystem>();
        animator = GetComponent<Animator>();
        dialogueManager.onDialogueBegin.AddListener(() => {
            HideInteract();
        });
        dialogueManager.onDialogueEnd.AddListener(() => {
            if (currentInteractables.Count > 0)
                ShowInteract();
        });
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
            if (!CanMove)
                return;
            currentInteractables.Add(interactable);
            if (!dialogueManager.IsActive && currentInteractables.Count == 1)
                ShowInteract();
        }
        else if (other.TryGetComponent(out Room room)) {
            // Try to enter a new room (don't reactivate previous rooms)
            if (!room.visited) {
                room.Activate();
                room.visited = true;
                if (cameraSystem.currentRoom == null || room.index > cameraSystem.currentRoom.index)
                    cameraSystem.currentRoom = room;
            }
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
        cameraSystem.FlashVignette();
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
        base.Die();
        animator.SetFloat("Horizontal", 0f);
        animator.SetFloat("Vertical", 0f);
        cameraSystem.PlayDeathAnimation();
        // Disable animator
        animator.enabled = false;
        // Set sprite to dead
    }

    public override void Respawn()
    {
        base.Respawn();
        animator.enabled = true;
    }
}
