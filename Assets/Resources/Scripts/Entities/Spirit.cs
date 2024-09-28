using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Spirit : User
{
    private const float MOVE_SPEED = 5f;

    [SerializeField] private Player player;
    private new Rigidbody2D rigidbody;

    /// <summary> Is the spirit trapped? (Boss fight) </summary>
    private bool trapped;

    protected override void Awake() {
        base.Awake();
        rigidbody = GetComponent<Rigidbody2D>();
    }

    protected override void Start()
    {
        base.Start();
        trapped = false;
    }

    protected override void FixedUpdate() {
        if (!CanMove || trapped)
            return;
        base.FixedUpdate();
        Vector2 myPosition = transform.position;
        Vector2 movement = new(Input.GetAxis("SpiritHorizontal"), Input.GetAxis("SpiritVertical"));
        if (movement.magnitude > 1)
            movement.Normalize();
        rigidbody.MovePosition(myPosition + MOVE_SPEED * Time.deltaTime * new Vector2(movement.x, movement.y));
    }

    public override void Die() {
        Debug.Log("Spirit should die here");
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

    public void TrapSpirit() {
        trapped = true;
        CanMove = true;
        StartCoroutine(cameraSystem.StartTrapSpiritAnimation());
    }
}
