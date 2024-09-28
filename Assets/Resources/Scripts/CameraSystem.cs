using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSystem : MonoBehaviour
{
    // Note that camera.orthographicSize is the half-size of vertical axis
    // so these are in units of half-size
    private const float MIN_SIZE = 3f, MAX_SIZE = 6f;
    private const float PADDING = 2f;

    public Player player;
    public Spirit spirit;
    private new Camera camera;

    void Awake() {
        camera = Camera.main;
        camera.orthographicSize = MIN_SIZE;
    }

    void Update() {
        UpdateCamera();
    }

    private void UpdateCamera() {
        Vector2 playerPos = player.transform.position, spiritPos = spirit.transform.position;
        Vector2 size = new(Mathf.Abs(playerPos.x - spiritPos.x) + PADDING, Mathf.Abs(playerPos.y - spiritPos.y) + PADDING);
        Vector2 position = (playerPos + spiritPos) / 2f;
        float targetSize = Mathf.Clamp(Mathf.Max(size.x / camera.aspect, size.y) / 2f, MIN_SIZE, MAX_SIZE);
        float mySize = camera.orthographicSize;
        float distance = targetSize - mySize;
        float delta = Mathf.Sign(distance) * (Mathf.Abs(distance) * 3f + .25f) * Time.deltaTime;
        if (Mathf.Abs(delta) > Mathf.Abs(distance))
            delta = distance;
        camera.orthographicSize += delta;
        camera.transform.position = new Vector3(position.x, position.y, camera.transform.position.z);
    }
}
