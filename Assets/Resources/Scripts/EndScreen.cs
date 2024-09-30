using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EndScreen : MonoBehaviour
{
    public TextMeshProUGUI scoreText, timeText;

    public void Show(int score, int time) {
        scoreText.text = score.ToString();
        timeText.text = $"{time / 60}:{time % 60:00}";
        gameObject.SetActive(true);
    }
}
