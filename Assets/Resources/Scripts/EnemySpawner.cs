using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class EnemySpawner : MonoBehaviour
{
    private const float START_DELAY = 5f;
    
    // Sizes should be the same
    [SerializeField] private Enemy[] enemyPrefabs;
    [SerializeField] private string[] spawnCountInfo;
    private int[][] spawnCount;
    [SerializeField] private int waveInterval;
    [SerializeField] private Vector2[] spawnLocations;
    [SerializeField] private UnityEvent onSpawn;
    private Collider2D activationArea;
    private TextMeshPro warningText;
    private Transform wallFolder;
    private List<Transform> walls;
    private int currentWave;
    private int waveCount;
    private List<Enemy> enemies;
    private bool active;
    private float intervalTimer;

    void Awake() {
        activationArea = GetComponent<Collider2D>();
        warningText = transform.Find("WarningText").GetComponent<TextMeshPro>();
        wallFolder = transform.Find("Walls");
        walls = new();
        foreach (Transform wall in wallFolder) {
            walls.Add(wall);
            wall.gameObject.SetActive(false);
        }
        currentWave = 0;
        waveCount = spawnCountInfo.Length;
        enemies = new List<Enemy>();
        spawnCount = new int[waveCount][];
        for (int i = 0; i < waveCount; i++) {
            spawnCount[i] = System.Array.ConvertAll(spawnCountInfo[i].Split(','), int.Parse);
        }
        active = false;
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
                UpdateWalls();
            }
            else if (intervalTimer == 0) {
                SpawnWave();
            }
            else {
                intervalTimer = Mathf.Max(0, intervalTimer - Time.deltaTime);
                showWarning = true;
            }
            warningText.enabled = showWarning;
        }
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.TryGetComponent(out User user) && !active && currentWave == 0) {
            Vector3 pos = Vector3.MoveTowards(user.transform.position, activationArea.bounds.center, 1f);
            active = true;
            intervalTimer = START_DELAY;
            CameraSystem cameraSystem = Camera.main.GetComponent<CameraSystem>();
            // Only dip to black if the player and spirit are not in the activation area
            if (!activationArea.OverlapPoint(cameraSystem.player.transform.position) || !activationArea.OverlapPoint(cameraSystem.spirit.transform.position)) {
                cameraSystem.PlayDipToBlack(() => {
                    cameraSystem.player.transform.position = pos + (Vector3)Random.insideUnitCircle * .5f;
                    cameraSystem.spirit.transform.position = pos + (Vector3)Random.insideUnitCircle * .5f;
                    UpdateWalls();
                });
            }
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
    }

    private void UpdateWalls() {
        foreach (Transform wall in walls) {
            wall.gameObject.SetActive(active);
        }
    }
}
