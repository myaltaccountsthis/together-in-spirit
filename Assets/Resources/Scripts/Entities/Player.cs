using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : User
{
    private const float MOVE_SPEED = 4f;
    private const float TWEEN_DURATION = .3f;

    // Important component references
    public DialogueManager dialogueManager;
    public Spirit spirit;
    public DataManager dataManager;
    private new Rigidbody2D rigidbody;

    protected override void Awake() {
        base.Awake();
        rigidbody = GetComponent<Rigidbody2D>();
        dialogueManager.gameObject.SetActive(true);
    }

    protected override void FixedUpdate() {
        if (!CanMove)
            return;
        Vector2 myPosition = transform.position;
        Vector2 movement = new(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        if (movement.magnitude > 1)
            movement.Normalize();
        rigidbody.MovePosition(myPosition + MOVE_SPEED * Time.deltaTime * new Vector2(movement.x, movement.y));
    }

    protected override bool CanInteractWith(Interactable interactable)
    {
        return interactable.CanPlayerInteract;
    }

    public void OpenUITest(InputAction.CallbackContext context) {
        if (context.performed)
            dialogueManager.Show(new Dialogue("Test Sender", new string[] { "This is a test dialogue.", "Next dialogue test lmfao" }));
    }

    public override void Die()
    {
        Debug.Log("Player should die here");
    }
}
