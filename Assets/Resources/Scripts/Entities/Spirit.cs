using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spirit : Entity
{
    private const float MOVE_SPEED = 5f;

    [SerializeField] private Player player;
    private new Rigidbody2D rigidbody;

    void Awake() {
        rigidbody = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate() {
        Vector2 myPosition = transform.position;
        Vector2 movement = new(Input.GetAxis("SpiritHorizontal"), Input.GetAxis("SpiritVertical"));
        if (movement.magnitude > 1)
            movement.Normalize();
        rigidbody.MovePosition(myPosition + MOVE_SPEED * Time.deltaTime * new Vector2(movement.x, movement.y));
        Debug.Log(movement);
    }

    public void Die() {
        Debug.Log("Spirit should die here");
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("SpiritDanger")) {
            Die();
        }
    }
}
