using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

public class BossKeySystem : MonoBehaviour
{
    private const float INITIAL_DELAY = 5f;
    private static readonly Key[] keys = new Key[] { Key.LeftArrow, Key.UpArrow, Key.DownArrow, Key.RightArrow };
    
    public Spirit spirit;
    public BossKey bossKeyPrefab;
    public GameObject warning;
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
        timeUntilNextKeys = INITIAL_DELAY;
        timeToEnterKeys = 0;
        bossKeys = new();
    }
    
    void Update() {
        float prevTimeToEnterKeys = timeToEnterKeys;
        timeUntilNextKeys -= Time.deltaTime;
        timeToEnterKeys -= Time.deltaTime;
        if (bossKeys.Count == 0) {
            if (timeUntilNextKeys <= 0) {
                warning.SetActive(false);
                GenerateKeys();
            }
            else if (timeUntilNextKeys <= 1.5f) {
                warning.SetActive(true);
            }
        }
        
        if (currentKeyIndex >= bossKeys.Count)
            return;
        
        if (timeToEnterKeys <= 0) {
            if (Mathf.FloorToInt(prevTimeToEnterKeys) != Mathf.FloorToInt(timeToEnterKeys)) {
                // Take damage every second
                OnFail();
            }
        }
        
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
                break;
            }
        }
    }

    void OnDestroy() {
        Hide();
    }

    private void OnFail() {
        // Spirit should take damage
        spirit.TakeDamage(5 * (++wrongInputs) + successfulInputs);
    }

    private void OnSuccess() {
        successfulInputs++;
        wrongInputs = 0;
        Hide();
    }

    private float GetNextKeysTime() {
        return Mathf.Max(10f - successfulInputs * .6f, GetKeyCount() * 1f);
    }

    private float GetTimeToEnterKeys() {
        return Mathf.Max(bossKeys.Count * 1f - successfulInputs * .4f, 2f + bossKeys.Count * .25f);
    }

    private int GetKeyCount() {
        if (successfulInputs < 3)
            return 4;
        return (successfulInputs / 3) + 4;
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
