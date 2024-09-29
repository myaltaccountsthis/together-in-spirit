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
    private const float TRAP_ANIMATION_SPIRIT_RADIUS = 3f;

    public Player player;
    public Spirit spirit;
    public Transform bossMachine;
    public Image cover;

    public bool IsAlive => player.Health > 0 && spirit.Health > 0;

    private new Camera camera;
    private PostProcessVolume volume;
    private bool cutsceneMode;
    public bool InDeathAnimation { get; private set; }

    void Awake() {
        camera = Camera.main;
        camera.orthographicSize = MIN_SIZE;
        volume = GetComponent<PostProcessVolume>();
    }

    void Start() {
        cutsceneMode = false;
    }

    void Update() {
        if (cutsceneMode)
            return;
        UpdateCamera();
    }

    public void FlashVignette() {
        LeanTween.cancel(volume.gameObject);
        LeanTween.value(volume.gameObject, value => volume.weight = value, 1, 0, .6f).setEaseOutSine();
    }

    private void UpdateCamera() {
        Vector2 playerPos = player.transform.position, spiritPos = spirit.transform.position;
        if (!spirit.gameObject.activeSelf) {
            spiritPos = playerPos;
        }
        Vector2 size = new(Mathf.Abs(playerPos.x - spiritPos.x) + PADDING, Mathf.Abs(playerPos.y - spiritPos.y) + PADDING);
        Vector2 position = (playerPos + spiritPos) / 2f;
        float targetSize = Mathf.Clamp(Mathf.Max(size.x / camera.aspect, size.y) / 2f, MIN_SIZE, MAX_SIZE);
        float mySize = camera.orthographicSize;
        float distance = targetSize - mySize;
        float delta = Mathf.Sign(distance) * (Mathf.Abs(distance) * 3f + .25f) * Time.deltaTime;
        float scale = Mathf.Max(size.x / camera.aspect, size.y) / 2;
        if (Mathf.Abs(delta) > Mathf.Abs(distance))
            delta = distance;
        if (scale > MAX_SIZE) {
            Vector2 offset = new(
                -Mathf.Max(0, position.x - MAX_SIZE * camera.aspect - (playerPos.x - PADDING / 2f)) + Mathf.Max(0, playerPos.x + PADDING / 2f - (position.x + MAX_SIZE * camera.aspect)),
                -Mathf.Max(0, position.y - MAX_SIZE - (playerPos.y - PADDING / 2f)) + Mathf.Max(0, playerPos.y + PADDING / 2f - (position.y + MAX_SIZE))
            );
            position += offset;
        }
        Vector2 posDistance = position - (Vector2)transform.position;
        Vector2 posDelta = posDistance.normalized * (posDistance.magnitude * 10f + .25f) * Time.deltaTime;
        if (posDelta.magnitude > posDistance.magnitude)
            posDelta = posDistance;
        camera.orthographicSize += delta;
        camera.transform.position += new Vector3(posDelta.x, posDelta.y, 0);
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
        float radius = LeanTween.linear(TRAP_ANIMATION_SPIRIT_RADIUS, 0, Mathf.Abs(alpha - .5f) * 2f);
        if (alpha < .5f)
            radius = LeanTween.linear(0, TRAP_ANIMATION_SPIRIT_RADIUS, alpha * 2f);
        float angle = Mathf.PI * 2f / (1.5f - alpha) * alpha * 1.5f;
        return new Vector3(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
    }

    public IEnumerator StartTrapSpiritAnimation() {
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
        // Tween the spirit into the machine
        Vector3 spiritStartPos = spirit.transform.position, spiritTargetPos = bossMachine.position;
        t = 0f;
        duration = (spiritTargetPos - spiritStartPos).magnitude / 2f + 2f;
        targetColor = Color.white;
        cover.color = startColor;
        while (t < duration) {
            t += Time.deltaTime;
            float trueAlpha = t / duration;
            float alpha = LeanTween.easeInQuad(0, 1, trueAlpha);
            // Spirit position should finish tweening early
            spirit.transform.position = Vector3.Lerp(spiritStartPos, spiritTargetPos, Mathf.Min(alpha * 1.8f, 1f)) + GetSpiritOffsetTrapAnimation(trueAlpha);
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
        UpdateCamera();
        LeanTween.color(cover.rectTransform, startColor, duration).setEaseOutQuad();
        yield return new WaitForSeconds(duration);
        cover.enabled = false;

        EndCutscene();
    }

    public void PlayDeathAnimation() {
        if (player.Health > 0)
            player.Die();
        if (spirit.Health > 0)
            spirit.Die();

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
        Debug.LogWarning("IMPLEMENT ROOM RESPAWN");
        // currentRoom.Activate();
        // transform.position = currentRoom.spawnLocation.position;
        player.Respawn();
        spirit.Respawn();
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
}
