using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Base class for Player and Spirit, can interact
/// </summary>
public abstract class User : Entity
{
    private const float TWEEN_DURATION = .3f;

    public RectTransform interactUI;
    protected HashSet<Interactable> currentInteractables;
    private Vector2 interactOpenPosition, interactClosedPosition;
    protected CameraSystem cameraSystem;

    private bool canMoveInternal = true;
    public bool CanMove { get => canMoveInternal; set {
        canMoveInternal = value;
        if (!canMoveInternal)
            HideInteract();
    } }

    protected override void Awake()
    {
        base.Awake();
        interactOpenPosition = interactUI.anchoredPosition;
        interactClosedPosition = new Vector2(interactOpenPosition.x, -interactUI.sizeDelta.y - interactOpenPosition.y - 10);
        cameraSystem = Camera.main.GetComponent<CameraSystem>();
    }

    protected override void Start()
    {
        base.Start();
        currentInteractables = new();
        interactUI.anchoredPosition = interactClosedPosition;
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

    public virtual void Die() {
        Debug.Log("Generic user died");
    }
}
