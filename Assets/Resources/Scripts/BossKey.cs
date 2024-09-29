using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BossKey : MonoBehaviour
{
    private static readonly Color[] colors = new Color[] { new(1, .3f, .3f), Color.yellow, new(.35f, .35f, 1), Color.green };
    private static readonly string[] texts = new string[] { "<", "^", "v", ">" };

    // Must set index, keyIndex, bossKeySystem when Instantiating
    public int index;
    public int keyIndex;
    public BossKeySystem bossKeySystem;
    private Image image;
    private TextMeshProUGUI text;

    void Awake() {
        image = GetComponent<Image>();
        text = GetComponentInChildren<TextMeshProUGUI>();
    }

    void Start() {
        // Index should be set before this is called
        image.color = colors[keyIndex];
        text.text = texts[keyIndex];
    }

    public void OnCorrect() {
        image.color = Color.Lerp(colors[keyIndex], Color.black, .5f);
    }
}