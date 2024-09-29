using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossLevel : MonoBehaviour
{
    private const float ACTIVATE_DELAY = 1f;

    private CameraSystem cameraSystem;
    private BossKeySystem bossKeySystem;

    void Awake() {
        cameraSystem = Camera.main.GetComponent<CameraSystem>();
        // Starts out inactive
        BossKeySystem prefab = Resources.Load<BossKeySystem>("Prefabs/BossKeySystem");
        bossKeySystem = Instantiate(prefab, GameObject.Find("WorldCanvas").transform);
        bossKeySystem.spirit = cameraSystem.spirit;
    }

    void Start() {
        Activate();
    }

    void OnDestroy() {
        if (bossKeySystem != null)
            Destroy(bossKeySystem.gameObject);
        cameraSystem.InBossFight = false;
    }

    public void Activate() {
        StartCoroutine(ActivateBossSequence());
    }

    private IEnumerator ActivateBossSequence() {
        yield return new WaitForSeconds(ACTIVATE_DELAY);
        cameraSystem.spirit.TrapSpirit();
        StartCoroutine(cameraSystem.StartTrapSpiritAnimation(() => {
            bossKeySystem.gameObject.SetActive(true);
            cameraSystem.InBossFight = true;
        }));
    }
}
