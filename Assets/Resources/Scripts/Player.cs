using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    private const float MOVE_SPEED = 4f;

    // Important component references
    private new Camera camera;
    private new Rigidbody2D rigidbody;
    [SerializeField] private DialogueManager dialogueManager;
    [SerializeField] private Spirit spirit;

    // Variable variables lol
    private Interactable currentInteractable;

    void Awake() {
        camera = Camera.main;
        rigidbody = GetComponent<Rigidbody2D>();
    }

    void Start() {
        currentInteractable = null;
    }

    void FixedUpdate() {
        Vector2 myPosition = transform.position;
        Vector2 movement = new(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        if (movement.magnitude > 1)
            movement.Normalize();
        rigidbody.MovePosition(myPosition + MOVE_SPEED * Time.deltaTime * new Vector2(movement.x, movement.y));
    }

    public void OpenUITest(InputAction.CallbackContext context) {
        if (context.performed)
            dialogueManager.Show(new Dialogue("Test Sender", new string[] { "This is a test dialogue.", "Next dialogue test lmfao" }));
    }

    public void Interact() {
        if (currentInteractable != null)
            currentInteractable.Interact();
    }
}
