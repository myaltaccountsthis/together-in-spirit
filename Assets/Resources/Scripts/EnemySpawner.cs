using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class EnemySpawner : MonoBehaviour
{
    private const float START_DELAY = 3f;

    public AudioClip alarmAudio;
    // Sizes should be the same
    [SerializeField] private Enemy[] enemyPrefabs;
    [SerializeField] private string[] spawnCountInfo;
    private int[][] spawnCount;
    [SerializeField] private int waveInterval;
    [SerializeField] private Vector2[] spawnLocations;
    [SerializeField] private UnityEvent onSpawn;
    [SerializeField] private UnityEvent onVictory;
    private Collider2D activationArea;
    private TextMeshPro warningText;
    [SerializeField] public Transform entranceWall, exitWall;
    private CameraSystem cameraSystem;
    private int currentWave;
    private int waveCount;
    private List<Enemy> enemies;
    private bool active;
    private bool playedWarning;
    private float intervalTimer;

    void Awake() {
        activationArea = GetComponent<Collider2D>();
        warningText = transform.Find("WarningText").GetComponent<TextMeshPro>();
        cameraSystem = Camera.main.GetComponent<CameraSystem>();
        entranceWall.gameObject.SetActive(false);
        exitWall.gameObject.SetActive(true);
        currentWave = 0;
        waveCount = spawnCountInfo.Length;
        enemies = new List<Enemy>();
        spawnCount = new int[waveCount][];
        for (int i = 0; i < waveCount; i++) {
            spawnCount[i] = System.Array.ConvertAll(spawnCountInfo[i].Split(','), int.Parse);
        }
        active = false;
        playedWarning = false;
        warningText.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (active) {
            bool showWarning = false;
            if (enemies.Count > 0) {
                enemies.RemoveAll(enemy => enemy == null);
            }
            else if (currentWave == waveCount) {
                active = false;
                entranceWall.gameObject.SetActive(false);
                exitWall.gameObject.SetActive(false);
                onVictory.Invoke();
            }
            else if (intervalTimer == 0) {
                SpawnWave();
            }
            else {
                intervalTimer = Mathf.Max(0, intervalTimer - Time.deltaTime);
                showWarning = true;
                if (!playedWarning) {
                    playedWarning = true;
                    AudioSource.PlayClipAtPoint(alarmAudio, transform.position);
                }
            }
            warningText.enabled = showWarning;
        }
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.TryGetComponent(out User user) && !active && currentWave == 0 &&
            activationArea.IsTouching(cameraSystem.player.hitbox) && activationArea.IsTouching(cameraSystem.spirit.hitbox)) {
            user.transform.position = Vector3.MoveTowards(user.transform.position, activationArea.bounds.center, .5f);
            active = true;
            intervalTimer = START_DELAY;
            entranceWall.gameObject.SetActive(true);
            // Redundant but ok
            exitWall.gameObject.SetActive(true);
        }
    }

    private void SpawnWave() {
        for (int i = 0; i < spawnCount[currentWave].Length; i++) {
            for (int j = 0; j < spawnCount[currentWave][i]; j++) {
                Enemy enemy = Instantiate(enemyPrefabs[i], spawnLocations[Random.Range(0, spawnLocations.Length)]
                + new Vector2(Random.Range(-.1f, .1f), Random.Range(-.1f, .1f)), Quaternion.identity, transform);
                enemies.Add(enemy);
            }
        }

        onSpawn.Invoke();
        currentWave++;
        intervalTimer = waveInterval;
        playedWarning = false;
    }
}
