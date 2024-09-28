using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public struct Data {
    public int roomIndex;
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
    }

    private void Save() {
        PlayerPrefs.SetInt("roomIndex", currentData.roomIndex);
        Debug.Log("Save successful");
    }

    private Data GetDefaultData() {
        return new Data() {
            roomIndex = 0
        };
    }

    public void SwitchScene(string sceneName) {
        Save();
        SceneManager.LoadScene(sceneName);
    }
}
