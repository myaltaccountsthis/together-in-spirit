using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : Entity
{
    private const float MOVE_SPEED = 4f;
    private const float TWEEN_DURATION = .5f;

    // Important component references
    public DialogueManager dialogueManager;
    public Spirit spirit;
    public RectTransform interactUI;
    private new Rigidbody2D rigidbody;

    // Variable variables lol
    private HashSet<Interactable> currentInteractables;
    
    // Other vars
    private Vector2 interactOpenPosition, interactClosedPosition;

    void Awake() {
        rigidbody = GetComponent<Rigidbody2D>();
        dialogueManager.gameObject.SetActive(true);
        interactOpenPosition = interactUI.anchoredPosition;
        interactClosedPosition = new Vector2(interactOpenPosition.x, -interactUI.sizeDelta.y - interactOpenPosition.y - 10);
    }

    void Start() {
        currentInteractables = new();
        interactUI.anchoredPosition = interactClosedPosition;
    }

    void FixedUpdate() {
        Vector2 myPosition = transform.position;
        Vector2 movement = new(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        if (movement.magnitude > 1)
            movement.Normalize();
        rigidbody.MovePosition(myPosition + MOVE_SPEED * Time.deltaTime * new Vector2(movement.x, movement.y));
    }

    // Collision detection for triggers
    void OnTriggerEnter2D(Collider2D other) {
        if (other.TryGetComponent(out Interactable interactable)) {
            currentInteractables.Add(interactable);
            if (currentInteractables.Count == 1)
                ShowInteract();
        }
    }
    void OnTriggerExit2D(Collider2D other) {
        if (other.TryGetComponent(out Interactable interactable)) {
            currentInteractables.Remove(interactable);
            if (currentInteractables.Count == 0)
                HideInteract();
        }
    }

    // Interactable stuff
    private void ShowInteract() {
        LeanTween.move(interactUI, interactOpenPosition, TWEEN_DURATION).setEaseOutQuad();
    }
    private void HideInteract() {
        LeanTween.move(interactUI, interactClosedPosition, TWEEN_DURATION).setEaseInQuad();
    }
    public void Interact(InputAction.CallbackContext context) {
        if (!context.performed)
            return;
        if (currentInteractables.Count > 0) {
            Interactable first = currentInteractables.First();
            first.Interact();
            currentInteractables.Remove(first);
            HideInteract();
        }
    }

    public void OpenUITest(InputAction.CallbackContext context) {
        if (context.performed)
            dialogueManager.Show(new Dialogue("Test Sender", new string[] { "This is a test dialogue.", "Next dialogue test lmfao" }));
    }
}
