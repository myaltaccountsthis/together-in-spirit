using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

public class BossKeySystem : MonoBehaviour
{
    private static readonly Key[] keys = new Key[] { Key.LeftArrow, Key.UpArrow, Key.DownArrow, Key.RightArrow };
    
    public Spirit spirit;
    public BossKey bossKeyPrefab;
    private List<BossKey> bossKeys;
    private RectTransform rectTransform;
    private Keyboard keyboard;
    private int currentKeyIndex;
    private int successfulInputs;
    private int wrongInputs;
    private float timeToEnterKeys;
    private float timeUntilNextKeys;

    void Awake() {
        rectTransform = GetComponent<RectTransform>();
        bossKeys = new();
        keyboard = Keyboard.current;
    }

    void Start() {
        successfulInputs = 0;
        wrongInputs = 0;
        timeUntilNextKeys = 0;
        timeToEnterKeys = 0;
    }
    
    void Update() {
        timeUntilNextKeys -= Time.deltaTime;
        timeToEnterKeys -= Time.deltaTime;
        if (timeUntilNextKeys <= 0 && bossKeys.Count == 0) {
            GenerateKeys();
        }
        
        if (currentKeyIndex >= bossKeys.Count)
            return;
        
        if (timeToEnterKeys <= 0)
            OnFail();
        
        for (int i = 0; i < keys.Length; i++) {
            if (keyboard[keys[i]].wasPressedThisFrame) {
                if (bossKeys[currentKeyIndex].keyIndex == i) {
                    // Shade the current key and increment currentKeyIndex
                    bossKeys[currentKeyIndex].OnCorrect();
                    currentKeyIndex++;
                    if (currentKeyIndex >= bossKeys.Count)
                        OnSuccess();
                }
                else {
                    OnFail();
                }
            }
        }
    }

    private void OnFail() {
        // Spirit should take damage
        spirit.TakeDamage(5 * (1 + ++wrongInputs));
        Hide();
    }

    private void OnSuccess() {
        successfulInputs++;
        Hide();
    }

    private float GetNextKeysTime() {
        return Mathf.Max(10f - successfulInputs, GetKeyCount() * .5f);
    }

    private float GetTimeToEnterKeys() {
        return Mathf.Max(bossKeys.Count * 1f - successfulInputs * .5f, 3f);
    }

    private int GetKeyCount() {
        if (successfulInputs < 3)
            return 4;
        return ((successfulInputs + 1) / 2) + 3;
    }

    /// <summary> Generate new keys, assuming the previous keys were destroyed </summary>
    public void GenerateKeys() {
        currentKeyIndex = 0;
        int count = GetKeyCount();
        for (int i = 0; i < count; i++) {
            BossKey newKey = Instantiate(bossKeyPrefab, rectTransform).GetComponent<BossKey>();
            newKey.GetComponent<RectTransform>().anchoredPosition = new Vector2(i * .5f + .025f, 0);
            newKey.index = i;
            newKey.keyIndex = Random.Range(0, 4);
            newKey.bossKeySystem = this;
            bossKeys.Add(newKey);
        }
        rectTransform.sizeDelta = new Vector2(count, 1) * .5f;
        timeToEnterKeys = GetTimeToEnterKeys();
    }

    public void Hide() {
        foreach (Transform child in rectTransform)
            Destroy(child.gameObject);
        bossKeys.Clear();
        timeUntilNextKeys = GetNextKeysTime();
    }
}
