using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spirit : User
{
    private const float MOVE_SPEED = 5f;

    [SerializeField] private Player player;
    private new Rigidbody2D rigidbody;

    protected override void Awake() {
        base.Awake();
        rigidbody = GetComponent<Rigidbody2D>();
    }

    protected override void FixedUpdate() {
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
}
