using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;

public class CameraSystem : MonoBehaviour
{
    // Note that camera.orthographicSize is the half-size of vertical axis
    // so these are in units of half-size
    private const float MIN_SIZE = 2.5f, MAX_SIZE = 8f;
    private const float PADDING = 3f;
    private const float BOSS_FIGHT_SIZE = 19f;
    private const float BOSS_FIGHT_PADDING = 5f;
    private const float TRAP_ANIMATION_SPIRIT_RADIUS = 3f;

    public Player player;
    public Spirit spirit;
    public Transform bossMachine;
    public SpiritSeparator spiritSeparator;
    public Image cover;
    public Room currentRoom;
    public SpriteRenderer magicCircle;
    public EndScreen endScreen;

    public bool IsAlive => player.Health > 0 && spirit.Health > 0;
    public bool IsCutsceneMode => cutsceneMode;

    private new Camera camera;
    private PostProcessVolume volume;
    private bool cutsceneMode;
    public bool InDeathAnimation { get; private set; }
    public bool InBossFight = false;

    void Awake() {
        camera = Camera.main;
        volume = GetComponent<PostProcessVolume>();
    }

    void Start() {
        cutsceneMode = false;
        ForceReposition();
    }

    void Update() {
        if (cutsceneMode)
            return;
        UpdateCamera();
        if (currentRoom.index == 0 && spirit.gameObject.activeSelf)
            spirit.gameObject.SetActive(false);
    }

    public void FlashVignette() {
        LeanTween.cancel(volume.gameObject);
        LeanTween.value(volume.gameObject, value => volume.weight = value, 1, 0, .6f).setEaseOutSine();
    }

    private void ForceReposition() {
        (Vector2 position, float targetSize) = GetCameraTargetPositionScale();
        camera.transform.position = new Vector3(position.x, position.y, camera.transform.position.z);
        camera.orthographicSize = targetSize;
    }

    private (Vector2, float) GetCameraTargetPositionScale() {
        Vector2 playerPos = player.transform.position, spiritPos = spirit.transform.position;
        if (!spirit.gameObject.activeSelf) {
            spiritPos = playerPos;
        }
        Vector2 size = new(Mathf.Abs(playerPos.x - spiritPos.x) + PADDING, Mathf.Abs(playerPos.y - spiritPos.y) + (InBossFight ? BOSS_FIGHT_PADDING : PADDING));
        Vector2 position = (playerPos + spiritPos) / 2f;
        if (InBossFight) {
            size.x = MathF.Max(size.x, BOSS_FIGHT_SIZE);
            // hardcoded ahh magic numbers
            position.x = -.5f;
        }
        float scale = Mathf.Max(size.x / camera.aspect, size.y) / 2f;
        float targetSize = Mathf.Clamp(scale, MIN_SIZE, InBossFight ? BOSS_FIGHT_SIZE : MAX_SIZE);
        if (scale > MAX_SIZE && !InBossFight) {
            Vector2 offset = new(
                -Mathf.Max(0, position.x - MAX_SIZE * camera.aspect - (playerPos.x - PADDING / 2f)) + Mathf.Max(0, playerPos.x + PADDING / 2f - (position.x + MAX_SIZE * camera.aspect)),
                -Mathf.Max(0, position.y - MAX_SIZE - (playerPos.y - PADDING / 2f)) + Mathf.Max(0, playerPos.y + PADDING / 2f - (position.y + MAX_SIZE))
            );
            position += offset;
        }
        return (position, targetSize);
    }

    private void UpdateCamera() {
        (Vector2 position, float targetSize) = GetCameraTargetPositionScale();
        float mySize = camera.orthographicSize;
        float distance = targetSize - mySize;
        float delta = Mathf.Sign(distance) * (Mathf.Abs(distance) * 3f + .25f) * Time.deltaTime;
        if (Mathf.Abs(delta) > Mathf.Abs(distance))
            delta = distance;
        Vector2 posDistance = position - (Vector2)transform.position;
        Vector2 posDelta = (posDistance.magnitude * 10f + .25f) * Time.deltaTime * posDistance.normalized;
        if (posDelta.magnitude > posDistance.magnitude)
            posDelta = posDistance;
        camera.orthographicSize += delta;
        camera.transform.position += (Vector3)posDelta;
    }

    public void BeginCutscene() {
        player.CanMove = false;
        spirit.CanMove = false;
        cutsceneMode = true;
    }

    public void EndCutscene() {
        player.CanMove = true;
        spirit.CanMove = true;
        cutsceneMode = false;
    }

    private Vector3 GetSpiritOffsetTrapAnimation(float alpha) {
        float radius = LeanTween.linear(TRAP_ANIMATION_SPIRIT_RADIUS, 0, Mathf.Abs(alpha - .7f) / .3f);
        if (alpha < .7f)
            radius = LeanTween.linear(0, TRAP_ANIMATION_SPIRIT_RADIUS, alpha / .7f);
        float angle = Mathf.PI * 2f / (1.5f - alpha) * alpha * 3f;
        return new Vector3(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
    }

    public IEnumerator StartTrapSpiritAnimation(Action callback) {
        BeginCutscene();
        // Do cutscene
        // Start with dip to black
        Color startColor = Color.clear, targetColor = Color.black;
        cover.color = startColor;
        cover.enabled = true;
        float t, duration = 1.25f;
        LeanTween.color(cover.rectTransform, targetColor, duration).setEaseOutQuad();
        yield return new WaitForSeconds(duration + .5f);
        // Focus on the machine for a bit
        camera.orthographicSize = 4f;
        camera.transform.position = new Vector3(bossMachine.position.x, bossMachine.position.y, camera.transform.position.z);
        LeanTween.color(cover.rectTransform, startColor, duration).setEaseInQuad();
        yield return new WaitForSeconds(duration + 2f);
        // Tween camera to the spirit
        Vector3 cameraTargetPos = new(spirit.transform.position.x, spirit.transform.position.y, camera.transform.position.z);
        LeanTween.move(camera.gameObject, cameraTargetPos, duration).setEaseOutQuad();
        yield return new WaitForSeconds(duration + .5f);
        // Tween in the magic circle
        Vector3 originalSize = magicCircle.transform.localScale;
        magicCircle.transform.position = spirit.transform.position;
        magicCircle.transform.localScale = Vector3.zero;
        magicCircle.enabled = true;
        LeanTween.scale(magicCircle.gameObject, originalSize, duration).setEaseOutSine();
        yield return new WaitForSeconds(duration + .5f);
        // Tween the spirit into the machine
        Vector3 spiritStartPos = spirit.transform.position, spiritTargetPos = bossMachine.position;
        t = 0f;
        duration = Mathf.Sqrt((spiritTargetPos - spiritStartPos).magnitude) + 2f;
        targetColor = Color.white;
        cover.color = startColor;
        while (t < duration) {
            t += Time.deltaTime;
            float trueAlpha = t / duration;
            float alpha = LeanTween.easeInQuad(0, 1, trueAlpha);
            // Spirit position should finish tweening early
            spirit.transform.position = Vector3.Lerp(spiritStartPos, spiritTargetPos, Mathf.Min(alpha * 1.8f, 1f)) + GetSpiritOffsetTrapAnimation(trueAlpha);
            magicCircle.transform.position = spirit.transform.position;
            camera.transform.position = new(spirit.transform.position.x, spirit.transform.position.y, camera.transform.position.z);
            cover.color = Color.Lerp(startColor, targetColor, Mathf.Max(0f, (alpha - .85f) / .15f));
            yield return null;
        }
        yield return new WaitForSeconds(1f);
        // End with dip to black
        duration = 1.25f;
        targetColor = Color.black;
        LeanTween.color(cover.rectTransform, targetColor, duration).setEaseInQuad();
        yield return new WaitForSeconds(duration + 1f);
        magicCircle.transform.localScale = originalSize;
        magicCircle.enabled = false;
        UpdateCamera();
        LeanTween.color(cover.rectTransform, startColor, duration).setEaseOutQuad();
        yield return new WaitForSeconds(duration);
        cover.enabled = false;

        EndCutscene();
        callback();
    }

    public void PlayDipToBlack(Action callback) {
        StartCoroutine(DipToBlackCoroutine(callback));
    }

    private IEnumerator DipToBlackCoroutine(Action callback) {
        BeginCutscene();
        Color startColor = Color.clear, targetColor = Color.black;
        cover.color = startColor;
        cover.enabled = true;
        float duration = 1.25f;
        LeanTween.color(cover.rectTransform, targetColor, duration).setEaseOutQuad();
        yield return new WaitForSeconds(duration + .5f);
        // Do callback and reposition camera
        callback();
        ForceReposition();
        LeanTween.color(cover.rectTransform, startColor, duration).setEaseInQuad();
        yield return new WaitForSeconds(duration);
        cover.enabled = false;
        EndCutscene();
    }

    public void PlayDeathAnimation() {
        if (player.Health > 0)
            player.TakeDamage(int.MaxValue / 2);
        if (spirit.Health > 0)
            spirit.TakeDamage(int.MaxValue / 2);

        InDeathAnimation = true;
        player.dataManager.currentData.score -= 100;
        StartCoroutine(RespawnCoroutine());
    }

    private IEnumerator RespawnCoroutine() {
        BeginCutscene();
        // Funny effects
        float t = 0, duration = 1f;
        Color startColor = new(1, 0, 0, 0), middleColor = Color.red, endColor = Color.black;
        cover.color = startColor;
        cover.enabled = true;
        // Tween to red
        while (t < duration) {
            t += Time.deltaTime;
            cover.color = Color.Lerp(startColor, middleColor, t / duration);
            yield return null;
        }
        // Tween to black
        t = 0;
        duration = 1.5f;
        while (t < duration) {
            t += Time.deltaTime;
            cover.color = Color.Lerp(middleColor, endColor, LeanTween.easeInOutSine(0, 1, t / duration));
            yield return null;
        }
        cover.color = endColor;
        // Do respawn functionality
        currentRoom.Activate();
        player.transform.position = currentRoom.spawnLocation.transform.position;
        spirit.transform.position = currentRoom.spiritSpawnLocation.transform.position;
        yield return null;
        ForceReposition();
        // Wait and tween to transparent
        yield return new WaitForSeconds(1);
        Color transparent = new(0, 0, 0, 0);
        t = 0;
        duration = 1f;
        while (t < duration) {
            t += Time.deltaTime;
            cover.color = Color.Lerp(endColor, transparent, t / duration);
            yield return null;
        }
        // Done with sequence
        cover.enabled = false;
        EndCutscene();
    }

    public void PlaySplitAnimation() {
        StartCoroutine(SplitAnimationCoroutine());
    }

    private IEnumerator SplitAnimationCoroutine() {
        BeginCutscene();
        float duration = 2f;
        SpriteRenderer spriteRenderer = spiritSeparator.GetComponent<SpriteRenderer>();

        LeanTween.move(player.gameObject, spiritSeparator.transform.position, duration).setEaseOutQuad();
        yield return new WaitForSeconds(duration);
        spirit.transform.position = player.transform.position;
        spirit.gameObject.SetActive(true);
        spriteRenderer.sprite = spiritSeparator.activateSprite;

        duration = 1f;
        LeanTween.alpha(player.gameObject, .3f, duration);
        yield return new WaitForSeconds(duration);

        LeanTween.alpha(spirit.gameObject, 0, duration);
        yield return new WaitForSeconds(duration + 1);

        spirit.transform.position = spiritSeparator.outputDuct.transform.position;
        LeanTween.alpha(spirit.gameObject, 1, duration);
        LeanTween.alpha(player.gameObject, 1, duration);
        yield return new WaitForSeconds(duration + 1);

        spriteRenderer.sprite = spiritSeparator.normalSprite;
        spirit.CanMove = true;
        player.dialogueManager.Show(new Dialogue("Yikes", new []{"The old machine you were investigating just separated your spirit from your body.", "Maybe your spirit can help you reach the rest of the mansion.", "Control your spirit with the arrow keys."}));
        EndCutscene();
    }


    public void OnWin() {
        StartCoroutine(OnWinCoroutine());
    }

    private IEnumerator OnWinCoroutine() {
        BeginCutscene();
        Color startColor = Color.clear, targetColor = Color.white;
        cover.color = startColor;
        cover.enabled = true;
        float duration = 1.25f;
        LeanTween.color(cover.rectTransform, targetColor, duration).setEaseOutQuad();
        yield return new WaitForSeconds(duration + .5f);
        // Do callback and reposition camera
        int time = Mathf.FloorToInt(Time.time - player.dataManager.currentData.startTime);
        int score = Mathf.Max(100, player.dataManager.currentData.score - time * 8);
        Debug.Log(time);
        endScreen.Show(score, time);
        ForceReposition();
        LeanTween.color(cover.rectTransform, startColor, duration).setEaseInQuad();
        yield return new WaitForSeconds(duration);
        cover.enabled = false;
        EndCutscene();
    }
}
