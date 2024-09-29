using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public struct Data {
    public int roomIndex;
    public int score;
};

public class DataManager : MonoBehaviour
{
    public Data currentData;

    void Awake() {
        Load();
    }

    void OnDestroy() {
        Save();
    }

    private void Load() {
        // Load data
        currentData = GetDefaultData();
        currentData.roomIndex = PlayerPrefs.GetInt("roomIndex", currentData.roomIndex);
        currentData.score = PlayerPrefs.GetInt("score", currentData.score);
    }

    private void Save() {
        PlayerPrefs.SetInt("roomIndex", currentData.roomIndex);
        PlayerPrefs.SetInt("score", currentData.score);
        PlayerPrefs.Save();
        Debug.Log("Save successful");
    }

    private Data GetDefaultData() {
        return new Data() {
            roomIndex = 0,
            score = 0
        };
    }

    public void SwitchScene(string sceneName) {
        Save();
        SceneManager.LoadScene(sceneName);
    }
}
