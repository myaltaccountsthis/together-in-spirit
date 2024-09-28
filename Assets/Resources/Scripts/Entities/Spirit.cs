using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spirit : User
{
    private const float MOVE_SPEED = 5f;

    [SerializeField] private Player player;
    [SerializeField] private SpiritAttack attackHitbox;

    protected override void Awake() {
        base.Awake();
    }

    protected override void FixedUpdate() {
        base.FixedUpdate();
        Vector2 myPosition = transform.position;
        Vector2 movement = new(Input.GetAxis("SpiritHorizontal"), Input.GetAxis("SpiritVertical"));
        if (movement.magnitude > 1)
            movement.Normalize();
        if (movement.magnitude > 0)
            lastAngle = Mathf.Deg2Rad * Vector2.SignedAngle(new Vector2(1, 0), movement);
        rigidbody.position = myPosition + MOVE_SPEED * Time.deltaTime * new Vector2(movement.x, movement.y);
    }

    public override void Die() {
        Debug.Log("Spirit should die here");
    }

    protected override bool CanInteractWith(Interactable interactable)
    {
        return interactable.CanSpiritInteract;
    }

    public override bool Attack()
    {
        if (!base.Attack())
            return false;
        
        SpiritAttack newAttack = Instantiate<SpiritAttack>(attackHitbox, transform.position, Quaternion.identity);
        newAttack.SetDirection(lastAngle);
        newAttack.transform.localPosition = transform.position + new Vector3(Mathf.Cos(lastAngle), Mathf.Sin(lastAngle), 0) * newAttack.getDistanceOffset();
        return true;
    }
}
