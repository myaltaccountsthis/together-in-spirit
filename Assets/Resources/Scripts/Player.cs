using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private const float MOVE_SPEED = 4f;

    private new Camera camera;
    private new Rigidbody2D rigidbody;

    void Awake() {
        camera = Camera.main;
        rigidbody = GetComponent<Rigidbody2D>();
    }

    void Update() {
        camera.transform.position = new Vector3(transform.position.x, transform.position.y, camera.transform.position.z);
    }

    void FixedUpdate() {
        Vector2 myPosition = transform.position;
        Vector2 movement = new(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        if (movement.magnitude > 1)
            movement.Normalize();
        rigidbody.MovePosition(myPosition + MOVE_SPEED * Time.deltaTime * new Vector2(movement.x, movement.y));
    }
}
