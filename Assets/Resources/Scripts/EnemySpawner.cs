using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    // Sizes should be the same
    [SerializeField] private Enemy[] enemyPrefabs;
    [SerializeField] private string[] spawnCountInfo;
    private int[][] spawnCount;
    [SerializeField] private int waveInterval;
    [SerializeField] private Vector2[] spawnLocations;
    private Collider2D activationArea;
    private int currentWave;
    private int waveCount;
    private List<Enemy> enemies;
    private bool active;
    private float intervalTimer;

    void Awake() {
        activationArea = GetComponent<Collider2D>();
        currentWave = 0;
        waveCount = spawnCountInfo.Length;
        enemies = new List<Enemy>();
        spawnCount = new int[waveCount][];
        for (int i = 0; i < waveCount; i++) {
            spawnCount[i] = Array.ConvertAll(spawnCountInfo[i].Split(','), int.Parse);
        }
        active = false;
        intervalTimer = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (active) {
            if (enemies.Count > 0) {
                enemies.RemoveAll(enemy => enemy == null);
            }
            else if (intervalTimer == 0) {
                if (currentWave == waveCount) {
                    active = false;
                }
                else {
                    SpawnWave();
                }
            }
            else {
                intervalTimer = Mathf.Max(0, intervalTimer - Time.deltaTime);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.TryGetComponent(out User user)) {
            active = true;
        }
    }

    void SpawnWave() {
        for (int i = 0; i < spawnCount[currentWave].Length; i++) {
            for (int j = 0; j < spawnCount[currentWave][i]; j++) {
                Enemy enemy = Instantiate(enemyPrefabs[i], spawnLocations[UnityEngine.Random.Range(0, spawnLocations.Length)], Quaternion.identity);
                enemies.Add(enemy);
            }
        }
        currentWave++;
        intervalTimer = waveInterval;
    }
}
