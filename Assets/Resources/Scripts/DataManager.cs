using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public struct Data {
    public int score;
    public float startTime;
};

public class DataManager : MonoBehaviour
{
    public Data currentData;
    
    void Awake() {
        currentData = GetDefaultData();
    }

    private Data GetDefaultData() {
        return new Data() {
            score = 10000,
            startTime = Time.time
        };
    }

    public void EnterGame(InputAction.CallbackContext context) {
        if (!context.performed)
            return;
        EnterGame();
    }
    public void EnterGame() {
        SwitchScene("Main");
    }

    public void SwitchScene(string sceneName) {
        SceneManager.LoadScene(sceneName);
    }

    public void ExitGame() {
        Application.Quit();
    }
}
