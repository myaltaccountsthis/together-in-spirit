using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    private const float START_DELAY = 3f;
    
    // Sizes should be the same
    [SerializeField] private Enemy[] enemyPrefabs;
    [SerializeField] private string[] spawnCountInfo;
    private int[][] spawnCount;
    [SerializeField] private int waveInterval;
    [SerializeField] private Vector2[] spawnLocations;
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
            spawnCount[i] = Array.ConvertAll(spawnCountInfo[i].Split(','), int.Parse);
        }
        active = false;
    }

    void Start() {
        intervalTimer = START_DELAY;
    }

    // Update is called once per frame
    void Update()
    {
        if (active) {
            bool showWarning = true;
            if (enemies.Count > 0) {
                enemies.RemoveAll(enemy => enemy == null);
                showWarning = false;
            }
            else if (intervalTimer == 0) {
                if (currentWave == waveCount) {
                    active = false;
                    UpdateWalls();
                }
                else {
                    SpawnWave();
                }
                showWarning = false;
            }
            else {
                intervalTimer = Mathf.Max(0, intervalTimer - Time.deltaTime);
            }
            warningText.enabled = showWarning;
        }
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.TryGetComponent(out User user) && !active) {
            active = true;
            UpdateWalls();
        }
    }

    private void SpawnWave() {
        for (int i = 0; i < spawnCount[currentWave].Length; i++) {
            for (int j = 0; j < spawnCount[currentWave][i]; j++) {
                Enemy enemy = Instantiate(enemyPrefabs[i], spawnLocations[UnityEngine.Random.Range(0, spawnLocations.Length)]
                + new Vector2(UnityEngine.Random.Range(-.1f, .1f), UnityEngine.Random.Range(-.1f, .1f)), Quaternion.identity, transform);
                enemies.Add(enemy);
            }
        }
        currentWave++;
        intervalTimer = waveInterval;
    }

    private void UpdateWalls() {
        foreach (Transform wall in walls) {
            wall.gameObject.SetActive(active);
        }
    }
}
