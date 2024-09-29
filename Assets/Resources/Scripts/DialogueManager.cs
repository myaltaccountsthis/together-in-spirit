using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class Dialogue {
    public string header;
    public string[] body;
    public Dialogue(string header, string[] body) {
        this.header = header;
        this.body = body;
    }
}

public class DialogueManager : MonoBehaviour, IPointerClickHandler
{
    private const float TWEEN_DURATION = .5f;
    private const float TYPEWRITER_DELAY = .05f;

    public UnityEvent onDialogueBegin, onDialogueEnd;
    public bool IsActive => active;
    private RectTransform rectTransform, inner;
    private TextMeshProUGUI header, body, arrow;
    private Dialogue currentDialogue;
    private int dialogueIndex = 0;
    private int typewriterIndex = 0;
    private bool active = false, debounce = false, shouldSkipTypewriter = false;

    private Vector2 openPosition, closedPosition;

    private Coroutine typewriter;

    void Awake() {
        rectTransform = GetComponent<RectTransform>();
        inner = transform.Find("Inner").GetComponent<RectTransform>();
        header = inner.Find("Header").GetComponent<TextMeshProUGUI>();
        body = inner.Find("Body").GetComponent<TextMeshProUGUI>();
        arrow = inner.Find("Arrow").GetComponent<TextMeshProUGUI>();
        openPosition = new Vector2(0, 30);
        closedPosition = new Vector2(0, -rectTransform.sizeDelta.y - rectTransform.anchoredPosition.y - 10);
    }

    void Start() {
        rectTransform.anchoredPosition = closedPosition;
    }

    private bool IsTypewriterFinished() {
        return dialogueIndex >= currentDialogue.body.Length || typewriterIndex >= currentDialogue.body[dialogueIndex].Length;
    }

    /// <summary> Do the typewriter thing </summary>
    private IEnumerator StartTypewriter() {
        body.text = "";
        arrow.enabled = false;
        typewriterIndex = 0;
        shouldSkipTypewriter = false;
        string text = currentDialogue.body[dialogueIndex];
        while (typewriterIndex < text.Length) {
            if (shouldSkipTypewriter) {
                body.text += text[typewriterIndex..];
                typewriterIndex = text.Length;
                break;
            }
            body.text += text[typewriterIndex++];
            yield return new WaitForSeconds(TYPEWRITER_DELAY);
        }
        arrow.text = (dialogueIndex < currentDialogue.body.Length - 1) ? ">" : "x";
        arrow.enabled = true;
    }

    private void UpdateBody() {
        typewriter = StartCoroutine(StartTypewriter());
    }

    /** <summary> Begin a dialogue sequence </summary> */
    public void Show(Dialogue info) {
        if (active)
        {
            typewriterIndex = int.MaxValue;
            StopCoroutine(typewriter);
        }
        active = true;
        debounce = true;
        currentDialogue = info;
        header.text = info.header;
        body.text = "";
        arrow.text = "";
        dialogueIndex = 0;
        onDialogueBegin.Invoke();
        if (typewriterIndex < int.MaxValue)
        {
            LeanTween.move(rectTransform, openPosition, TWEEN_DURATION).setEaseOutQuad().setOnComplete(() =>
            {
                UpdateBody();
                debounce = false;
            });
        }
        else
        {
            UpdateBody();
            debounce = false;
        }
    }

    public void ShowNext() {
        if (++dialogueIndex >= currentDialogue.body.Length) {
            Hide();
            return;
        }
        UpdateBody();
    }

    public void Hide() {
        debounce = true;
        onDialogueEnd.Invoke();
        LeanTween.move(rectTransform, closedPosition, TWEEN_DURATION).setEaseOutQuad().setOnComplete(() => {
            active = false;
            debounce = false;
        });
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        SpeedUp();
    }

    public void OnKeyboardInput(InputAction.CallbackContext context) {
        if (context.performed)
            SpeedUp();
    }

    public void SpeedUp() {
        // Don't register click if not active or if on debounce cooldown
        if (!active || debounce)
            return;
        if (IsTypewriterFinished())
            ShowNext();
        else
            shouldSkipTypewriter = true;
    }
}
