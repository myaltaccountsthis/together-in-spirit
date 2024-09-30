using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BossLevel : MonoBehaviour
{
    private const float ACTIVATE_DELAY = .5f;
    private const float SPIRIT_WAVE_DELAY = .25f;
    private const float ENEMY_ORB_DELAY = .4f;
    private const float ENEMY_ORB_TRAVEL_DURATION = 2f;
    private const float BOSS_CHARGE_DURATION = 2f;
    private const float BOSS_CHARGE_ANIMATION_DELAY = .2f;
    private const float BOSS_HOVER_PERIOD = 4f, BOSS_HOVER_AMPLITUDE = .03f;

    public BoxCollider2D boundsCollider;
    [HideInInspector] public Bounds bounds;
    public GameObject warning, keyWarning;
    public Transform bossMinionTransform, bezierPointTransform;
    public Sprite bossSprite;
    public Sprite[] bossChargeSprites;
    public Sprite bossAttackSprite;
    public SpriteRenderer bossSpriteRenderer;
    public SpriteRenderer machineSpriteRenderer;
    public Sprite machineSpriteActive, machineSpriteInactive;
    public float TIME_TO_SURVIVE = 90f;
    private CameraSystem cameraSystem;
    private BossKeySystem bossKeySystem;
    private SpiritWave spiritWavePrefab;
    private Enemy enemyPrefab, enemyBigPrefab;
    private Transform enemyOrbPrefab;
    private SpriteRenderer enemyPortalPrefab;
    private HealthBar healthBar;

    private bool playedCutscene;
    private bool active;
    private bool isAttacking, isCharging;
    private float nextAttackCooldown;
    private int numAttacks;
    private int chargeAnimIndex;
    private float aliveTime;
    private float sineWaveTick, animationTick;

    void Awake() {
        cameraSystem = Camera.main.GetComponent<CameraSystem>();
        // Starts out inactive
        bounds = boundsCollider.bounds;
        spiritWavePrefab = Resources.Load<SpiritWave>("Prefabs/SpiritWave");
        enemyPrefab = Resources.Load<Enemy>("Prefabs/Enemy");
        enemyBigPrefab = Resources.Load<Enemy>("Prefabs/EnemyBig");
        enemyOrbPrefab = Resources.Load<Transform>("Prefabs/EnemyOrb");
        enemyPortalPrefab = Resources.Load<SpriteRenderer>("Prefabs/EnemyPortal");
        BossKeySystem prefab = Resources.Load<BossKeySystem>("Prefabs/BossKeySystem");
        bossKeySystem = Instantiate(prefab, GameObject.Find("WorldCanvas").transform);
        bossKeySystem.warning = keyWarning;
        bossKeySystem.spirit = cameraSystem.spirit;
        warning.SetActive(false);
        keyWarning.SetActive(false);
        playedCutscene = false;
        active = false;
        numAttacks = 0;
        isAttacking = false;
        animationTick = 0;
        chargeAnimIndex = 0;
        aliveTime = 0;
        nextAttackCooldown = ACTIVATE_DELAY;
        machineSpriteRenderer.sprite = machineSpriteInactive;
    }

    void Update() {
        if (!playedCutscene) {
            if (cameraSystem.IsCutsceneMode)
                return;
            nextAttackCooldown -= Time.deltaTime;
            if (nextAttackCooldown <= 0) {
                playedCutscene = true;
                Activate();
            }
        }
        if (!active)
            return;
        
        // Do up and down sine wave
        sineWaveTick += Time.deltaTime;
        if (sineWaveTick >= BOSS_HOVER_PERIOD)
            sineWaveTick -= BOSS_HOVER_PERIOD;
        bossSpriteRenderer.transform.localPosition = new(0, BOSS_HOVER_AMPLITUDE * Mathf.Sin(2 * Mathf.PI * sineWaveTick / BOSS_HOVER_PERIOD), 0);

        aliveTime += Time.deltaTime;
        float healthPercent = healthBar.Health = 1 - aliveTime / TIME_TO_SURVIVE;
        if (healthPercent <= 0) {
            active = false;
            bossSpriteRenderer.sprite = bossSprite;
            cameraSystem.OnWin();
            Destroy(gameObject);
            return;
        }

        if (isAttacking) {
            // Animation stuff
            if (isCharging) {
                animationTick += Time.deltaTime;
                if (animationTick >= BOSS_CHARGE_ANIMATION_DELAY) {
                    animationTick -= BOSS_CHARGE_ANIMATION_DELAY;
                    chargeAnimIndex = (chargeAnimIndex + 1) % bossChargeSprites.Length;
                    bossSpriteRenderer.sprite = bossChargeSprites[chargeAnimIndex];
                }
            }
        }
        else {
            nextAttackCooldown -= Time.deltaTime;
            if (nextAttackCooldown <= 0) {
                warning.SetActive(false);
                DoRandomAttack();
            }
            else if (nextAttackCooldown <= 1.5f) {
                warning.SetActive(true);
            }
        }
    }

    void OnDestroy() {
        if (bossKeySystem != null)
            Destroy(bossKeySystem.gameObject);
        if (healthBar != null)
            Destroy(healthBar.gameObject);
        cameraSystem.InBossFight = false;
    }

    public void Activate() {
        StartCoroutine(ActivateBossSequence());
    }

    private IEnumerator ActivateBossSequence() {
        // START BOSS
        yield return new WaitForSeconds(ACTIVATE_DELAY);
        cameraSystem.spirit.TrapSpirit();
        StartCoroutine(cameraSystem.StartTrapSpiritAnimation(() => {
            nextAttackCooldown = GetNextAttackCooldown();
            bossKeySystem.gameObject.SetActive(true);
            cameraSystem.InBossFight = true;
            active = true;
            machineSpriteRenderer.sprite = machineSpriteActive;
            healthBar = Instantiate(Resources.Load<HealthBar>("Prefabs/HealthBar"), GameObject.Find("WorldCanvas").transform);
            healthBar.attachment = bossSpriteRenderer.transform.Find("HealthBarAttachment");
            healthBar.scale = 1.5f;
        }));
    }

    // General attack functions
    private IEnumerator ChargeAttackCoroutine(UnityAction callback) {
        chargeAnimIndex = 0;
        isCharging = true;
        yield return new WaitForSeconds(BOSS_CHARGE_DURATION);
        callback();
    }
    
    private void DoneWithAttack() {
        isAttacking = false;
        numAttacks++;
        bossSpriteRenderer.sprite = bossSprite;
        nextAttackCooldown = GetNextAttackCooldown();
    }

    private void DoRandomAttack() {
        isAttacking = true;
        StartCoroutine(ChargeAttackCoroutine(() => {
            isCharging = false;
            bossSpriteRenderer.sprite = bossAttackSprite;
            float val = Random.value;
            if (val < .6f)
                StartCoroutine(SummonSpiritWaves());
            else
                StartCoroutine(SpawnEnemies());
        }));
    }

    private float GetNextAttackCooldown() {
        return 8f - 5 / (1 + Mathf.Exp(-.5f * (numAttacks - 5)));
    }

    // Spirit wave stuff
    /// <summary> Get the number of spirit waves that the boss should summon, per spirit wave tick </summary>
    private int GetSpiritWavesPerSecond() {
        return Mathf.FloorToInt(Mathf.Sqrt(numAttacks + 2) * 3 + 2);
    }

    private int GetSpiritWaveDamage() {
        return Mathf.FloorToInt(Mathf.Sqrt(numAttacks + 1) * 10 + 5f);
    }
    
    private (Vector3, Vector3) GetRandomSpiritWavePositionAndDirection() {
        float val = Random.value;
        if (val < .25f)
            return (new Vector3(bounds.min.x, Random.Range(bounds.min.y, bounds.max.y), 0), Vector3.right);
        if (val < .5f)
            return (new Vector3(bounds.max.x, Random.Range(bounds.min.y, bounds.max.y), 0), Vector3.left);
        if (val < .75f)
            return (new Vector3(Random.Range(bounds.min.x, bounds.max.x), bounds.min.y, 0), Vector3.up);
        return (new Vector3(Random.Range(bounds.min.x, bounds.max.x), bounds.max.y, 0), Vector3.down);
    }

    private float GetSpiritWaveSpeed() {
        return Random.Range(3f, 5f * Mathf.Log10(numAttacks + 10));
    }

    private IEnumerator SummonSpiritWaves() {
        int duration = 5;
        int spiritWavesPerSecond = GetSpiritWavesPerSecond();
        int totalSpiritWaves = duration * spiritWavesPerSecond;
        float t = 0;
        for (int i = 0; i < totalSpiritWaves; i++) {
            while (spiritWavesPerSecond * t < i) {
                yield return new WaitForSeconds(SPIRIT_WAVE_DELAY);
                t += SPIRIT_WAVE_DELAY;
            }
            (Vector3 pos, Vector3 direction) = GetRandomSpiritWavePositionAndDirection();
            SpiritWave spiritWave = Instantiate(spiritWavePrefab, pos, Quaternion.identity, transform);
            spiritWave.damage = GetSpiritWaveDamage();
            spiritWave.SetDirection(direction * GetSpiritWaveSpeed());
        }
        DoneWithAttack();
    }

    // Enemy spawning stuff
    private int GetEnemyCount() {
        return Mathf.FloorToInt(Mathf.Sqrt(numAttacks + 2));
    }

    private Enemy GetRandomEnemyPrefab() {
        if (numAttacks < 3)
            return enemyPrefab;
        float val = Random.value;
        val = Mathf.Pow(val, 1 + numAttacks / 10f);
        if (val < .8f)
            return enemyPrefab;
        return enemyBigPrefab;
    }

    private IEnumerator SpawnEnemies() {
        int numEnemies = GetEnemyCount();
        for (int i = 0; i < numEnemies; i++) {
            Vector3 pos = new(Random.Range(bounds.min.x, bounds.max.x), Random.Range(bounds.min.y, bounds.max.y), 0);
            Transform orb = Instantiate(enemyOrbPrefab, bossMinionTransform.position, Quaternion.identity, transform);
            LTBezierPath path = new(new Vector3[] {
                bossMinionTransform.position,
                bezierPointTransform.position,
                bezierPointTransform.position,
                pos
            });
            LeanTween.move(orb.gameObject, path, ENEMY_ORB_TRAVEL_DURATION).setOnComplete(() => {
                Destroy(orb.gameObject);
                SpriteRenderer portal = Instantiate(enemyPortalPrefab, pos, Quaternion.identity, transform);
                portal.color = new(1, 1, 1, 0);
                LeanTween.alpha(portal.gameObject, 1, .1f).setOnComplete(() => {
                    Instantiate(GetRandomEnemyPrefab(), pos, Quaternion.identity, transform);
                    LeanTween.alpha(portal.gameObject, 0, .6f).setEaseInQuad().setOnComplete(() => Destroy(portal.gameObject));
                });
            });
            yield return new WaitForSeconds(ENEMY_ORB_DELAY);
        }
        yield return new WaitForSeconds(ENEMY_ORB_TRAVEL_DURATION);
        DoneWithAttack();
    }
}
