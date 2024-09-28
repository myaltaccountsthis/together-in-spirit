using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSystem : MonoBehaviour
{
    private new Camera camera;
    [SerializeField] private Player player;
    [SerializeField] private Spirit spirit;

    void Awake() {
        camera = Camera.main;
    }

    void Update() {
        UpdateCamera();
    }

    private void UpdateCamera() {
        Vector2 playerPos = player.transform.position;
        camera.transform.position = new Vector3(playerPos.x, playerPos.y, camera.transform.position.z);
    }
}
