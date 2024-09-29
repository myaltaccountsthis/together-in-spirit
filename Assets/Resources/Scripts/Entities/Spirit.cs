using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Spirit : User
{
    private const float MOVE_SPEED = 4f;

    [SerializeField] private Player player;
    [SerializeField] private SpiritAttack attackHitbox;

    /// <summary> Is the spirit trapped? (Boss fight) </summary>
    private bool trapped;

    protected override void Start()
    {
        base.Start();
        trapped = false;
    }

    protected override void FixedUpdate() {
        if (!CanMove || trapped) {
            animator.SetFloat("Horizontal", 0f);
            animator.SetFloat("Vertical", 0f);
            return;
        }
        base.FixedUpdate();
        Vector2 myPosition = transform.position;
        Vector2 movement = new(Input.GetAxis("SpiritHorizontal"), Input.GetAxis("SpiritVertical"));
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
        return interactable.CanSpiritInteract;
    }

    public void DoTest(InputAction.CallbackContext context) {
        if (!context.performed)
            return;
        TrapSpirit();
    }

    /// <summary> Does not begin the cutscene. Call that separately </summary>
    public void TrapSpirit() {
        trapped = true;
        CanMove = false;
    }

    public override bool Attack()
    {
        if (!base.Attack())
            return false;
        SpiritAttack newAttack = Instantiate(attackHitbox, transform.position, Quaternion.identity);
        newAttack.SetDirection(lastAngle);
        newAttack.transform.localPosition = transform.position + new Vector3(Mathf.Cos(lastAngle), Mathf.Sin(lastAngle), 0) * newAttack.getDistanceOffset();
        return true;
    }

    public void ShowAttackInstructions()
    {
        dialogueManager.Show(new Dialogue("What is that thing?", new string[]{"Your spirit can use the right shift key to fire a ranged attack. Your player can also do a slash attack with Q"}));
    }

    public override void Respawn()
    {
        base.Respawn();
        trapped = false;
        CanMove = true;
    }
}
