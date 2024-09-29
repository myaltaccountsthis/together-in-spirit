using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    const float redAmount = .25f, yellowAmount = .5f, greenAmount = .75f;
    
    public Transform attachment;
    public float scale = 1f;

    private Transform inner;
    private Image bar;

    public float Health {
        set {
            bar.fillAmount = value;
            bar.color = GetColor(value);
        }
    }

    void Awake() {
        inner = transform.Find("Inner");
        bar = inner.Find("Bar").GetComponent<Image>();
    }

    void Start() {
        transform.localScale = scale * Vector3.one;
    }

    void Update() {
        transform.position = attachment.position + .5f * scale * Vector3.up;
    }
    
    private Color GetColor(float value) {
        if (value >= greenAmount)
            return Color.green;
        if (value <= redAmount)
            return Color.red;
        return Color.Lerp(Color.red, Color.yellow, (value - redAmount) / (yellowAmount - redAmount));
    }
}
