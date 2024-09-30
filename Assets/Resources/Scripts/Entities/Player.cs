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
    public Spirit spirit;
    public DataManager dataManager;
    public AudioClip walkingSound;
    public float walkingSoundCooldown;
    private float tempCooldown = 0;

    protected override void Awake() {
        base.Awake();
        dialogueManager.gameObject.SetActive(true);
        transform.position = cameraSystem.currentRoom.spawnLocation.position;
        dialogueManager.Show(new Dialogue("Case File 864", new string[]{"Witnesses have reported unsettling noises coming from inside the abandoned mansion. Investigate the area and report back to me."}));
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
        {
            lastAngle = Mathf.Deg2Rad * Vector2.SignedAngle(new Vector2(1, 0), movement);
            tempCooldown -= Time.deltaTime;
            if (tempCooldown < 0)
            {
                AudioSource.PlayClipAtPoint(walkingSound, transform.position);
                tempCooldown = walkingSoundCooldown;
            }
        }
        else
        {
            tempCooldown = 0;
        }

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
        PlayerAttack newAttack = Instantiate(attackHitbox, transform.position, Quaternion.identity);
        newAttack.SetDirection(lastAngle);
        newAttack.transform.localPosition = transform.position + new Vector3(Mathf.Cos(lastAngle), Mathf.Sin(lastAngle), 0) * newAttack.getDistanceOffset();
        return true;
    }

    public void DoTest(InputAction.CallbackContext context) {
        if (context.performed)
            dialogueManager.Show(new("Bro lmfao", new string[] {"test dialogue 123"}));
    }


}
