using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : User
{
    private const float MOVE_SPEED = 3f;

    [SerializeField] private PlayerAttack attackHitbox;

    // Important component references
    public DialogueManager dialogueManager;
    public Spirit spirit;
    public DataManager dataManager;
    private Animator animator;

    protected override void Awake() {
        base.Awake();
        dialogueManager.gameObject.SetActive(true);
        animator = GetComponent<Animator>();
    }

    protected override void FixedUpdate() {
        if (!CanMove) {
            animator.SetFloat("Horizontal", 0f);
            animator.SetFloat("Vertical", 0f);
            return;
        }
        base.FixedUpdate();
        Vector2 myPosition = transform.position;
        Vector2 movement = new(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        animator.SetFloat("Horizontal", movement.x);
        animator.SetFloat("Vertical", movement.y);
        if (movement.magnitude > 1)
            movement.Normalize();
        if (movement.magnitude > 0)
            lastAngle = Mathf.Deg2Rad * Vector2.SignedAngle(new Vector2(1, 0), movement);
        rigidbody.position = myPosition + MOVE_SPEED * Time.deltaTime * new Vector2(movement.x, movement.y);
    }

    protected override bool CanInteractWith(Interactable interactable)
    {
        return interactable.CanPlayerInteract;
    }

    public override bool Attack()
    {
        if (!base.Attack())
            return false;
        // TODO: Impl attack
        PlayerAttack newAttack = Instantiate<PlayerAttack>(attackHitbox, transform.position, Quaternion.identity);
        newAttack.SetDirection(lastAngle);
        newAttack.transform.localPosition = transform.position + new Vector3(Mathf.Cos(lastAngle), Mathf.Sin(lastAngle), 0) * newAttack.getDistanceOffset();
        return true;
    }

    public override void TakeDamage(int damage) {
        base.TakeDamage(damage);
    }

    public override void Die() {
        Debug.Log("Player should die here");
    }

    public void DoTest(InputAction.CallbackContext context) {
        if (context.performed)
            cameraSystem.FlashVignette();
    }


}
