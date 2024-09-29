using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossLevel : MonoBehaviour
{
    private CameraSystem cameraSystem;
    private BossKeySystem bossKeySystem;

    void Awake() {
        cameraSystem = Camera.main.GetComponent<CameraSystem>();
        BossKeySystem prefab = Resources.Load<BossKeySystem>("Prefabs/BossKeySystem");
        bossKeySystem = Instantiate(prefab, GameObject.Find("WorldCanvas").transform);
        bossKeySystem.spirit = cameraSystem.spirit;
    }

    void Start() {
        
    }

    void OnDestroy() {
        if (bossKeySystem != null)
            Destroy(bossKeySystem.gameObject);
    }
}
