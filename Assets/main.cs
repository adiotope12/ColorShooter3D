using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;  
using UnityEngine;
using System.Dynamic;

public class main : MonoBehaviour
{
    static private main S;
    static private Dictionary<eWeaponType, WeaponDefinition> WEAP_DICT;
    static private HashSet<int> bossThresholdsTriggered = new HashSet<int>();
    [Header("Inscribed")]
    public bool spawnEnemies = true;
    public GameObject[] prefabEnemy;
    public GameObject prefabBoss;
    public float enemySpawnPerSecond = 0.5f;
    public float enemyInsetDefault = 1.5f;
    public float gameRestartDelay = 2f;
    public GameObject prefabPowerUp;
    public int[] bossScoreThresholds = new int[] { 50, 100, 200 };
    public Score distanceScore;
    public WeaponDefinition[] weaponDefinitions;
    public eWeaponType[] powerUpFrequency = new eWeaponType[]
    {
        eWeaponType.blaster, eWeaponType.blaster,
        eWeaponType.spread, eWeaponType.shield
    };

    static public WeaponDefinition GET_WEAPON_DEFINITION(eWeaponType wt)
    {
        if(WEAP_DICT.ContainsKey(wt))
        {
            return WEAP_DICT[wt];
        }
        return new WeaponDefinition();
    }

    static public float GET_PROJECTILE_DAMAGE(eWeaponType wt)
    {
        float baseDamage = GET_WEAPON_DEFINITION(wt).damageOnHit;
        int damageLevel = UpgradeButton.GetStoredUpgradeLevel("Damage", 0);
        float damageMultiplier = 1f + (0.5f * damageLevel);
        return baseDamage * damageMultiplier;
    }



    private BoundsCheck bndCheck;
    private int previousDistanceScore = 0;

    private enum EnemySpawnSide
    {
        Top,
        Left,
        Right,
        Bottom
    }

    void Awake()
    {
        S = this;
        bndCheck = GetComponent<BoundsCheck>();
        bossThresholdsTriggered.Clear();
        Invoke("SpawnEnemy", GetEnemySpawnDelay());

        WEAP_DICT = new Dictionary<eWeaponType, WeaponDefinition>();
        foreach (WeaponDefinition def in weaponDefinitions)        {
            WEAP_DICT[def.type] = def;
        }

        if (distanceScore == null)
        {
            distanceScore = FindObjectOfType<Score>();
        }

        previousDistanceScore = GetCurrentDistanceScore();
    }

    void Update()
    {
        CheckBossDistanceThresholds();
    }

    int GetCurrentDistanceScore()
    {
        if (distanceScore == null)
        {
            distanceScore = FindObjectOfType<Score>();
        }

        if (distanceScore == null)
        {
            return 0;
        }

        return Mathf.FloorToInt(distanceScore.metersFlown);
    }

    void CheckBossDistanceThresholds()
    {
        if (prefabBoss == null) return;

        int currentDistanceScore = GetCurrentDistanceScore();
        foreach (int threshold in bossScoreThresholds)
        {
            if (previousDistanceScore < threshold && currentDistanceScore >= threshold && !bossThresholdsTriggered.Contains(threshold))
            {
                bossThresholdsTriggered.Add(threshold);
                SpawnBoss();
            }
        }

        previousDistanceScore = currentDistanceScore;
    }

    float GetEnemySpeedMultiplier()
    {
        // Linearly scale from 1x to 3x as distance score goes from 0 to 200.
        float t = Mathf.Clamp01(GetCurrentDistanceScore() / 200f);
        return Mathf.Lerp(1f, 3f, t);
    }

    float GetEnemyHealthMultiplier()
    {
        // Linearly scale from 1x to 5x as distance score goes from 0 to 200.
        float t = Mathf.Clamp01(GetCurrentDistanceScore() / 200f);
        return Mathf.Lerp(1f, 5f, t);
    }

    float GetEnemySpawnDelay()
    {
        float effectiveSpawnPerSecond = enemySpawnPerSecond * GetEnemySpeedMultiplier();
        return 1f / Mathf.Max(0.01f, effectiveSpawnPerSecond);
    }

    public void SpawnEnemy()
    {
        if (!spawnEnemies)
        {
            Invoke (nameof(SpawnEnemy), GetEnemySpawnDelay());
            return;
        }
        int ndx = Random.Range(0, prefabEnemy.Length);
        GameObject go = Instantiate<GameObject>(prefabEnemy[ndx]);

        float enemyInset = enemyInsetDefault;
        if (go.GetComponent<BoundsCheck>() != null)
        {
            enemyInset = Mathf.Abs(go.GetComponent<BoundsCheck>().radius);
        }
        Vector3 pos = Vector3.zero;
        float xMin = -bndCheck.camWidth + enemyInset;
        float xMax = bndCheck.camWidth - enemyInset;
        float yMin = -bndCheck.camHeight + enemyInset;
        float yMax = bndCheck.camHeight - enemyInset;

        EnemySpawnSide spawnSide = (EnemySpawnSide)Random.Range(0, 4);
        Vector3 moveDirection = Vector3.down;
        BoundsCheck.eScreenLocs despawnEdge = BoundsCheck.eScreenLocs.offDown;

        switch (spawnSide)
        {
            case EnemySpawnSide.Top:
                pos.x = Random.Range(xMin, xMax);
                pos.y = bndCheck.camHeight + enemyInset;
                moveDirection = Vector3.down;
                despawnEdge = BoundsCheck.eScreenLocs.offDown;
                break;
            case EnemySpawnSide.Left:
                pos.x = -bndCheck.camWidth - enemyInset;
                pos.y = Random.Range(yMin, yMax);
                moveDirection = Vector3.right;
                despawnEdge = BoundsCheck.eScreenLocs.offRight;
                break;
            case EnemySpawnSide.Right:
                pos.x = bndCheck.camWidth + enemyInset;
                pos.y = Random.Range(yMin, yMax);
                moveDirection = Vector3.left;
                despawnEdge = BoundsCheck.eScreenLocs.offLeft;
                break;
            default:
                pos.x = Random.Range(xMin, xMax);
                pos.y = -bndCheck.camHeight - enemyInset;
                moveDirection = Vector3.up;
                despawnEdge = BoundsCheck.eScreenLocs.offUp;
                break;
        }

        go.transform.position = pos;

        Enemy enemy = go.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.speed *= GetEnemySpeedMultiplier();
            enemy.SetMoveDirection(moveDirection);
            enemy.SetDespawnEdge(despawnEdge);
        }

        Invoke("SpawnEnemy", GetEnemySpawnDelay());
    }

    void DelayedRestart()
    {
        Invoke(nameof(Restart), gameRestartDelay);
    }

    void Restart()
    {
        // Add linking to upgrade scene and saving coins here.
        SceneManager.LoadScene("__Scene_0");
    }

    static public void HERO_DIED()
    {
        S.DelayedRestart();
    }
    
    static public void SHIP_DESTROYED(Enemy e)
    {
        if (S == null)
    {
        Debug.LogWarning("main.S is null in SHIP_DESTROYED! Check initialization order.");
        return;
    }

        if (Random.value <= e.powerUpDropChance)
        {
            Debug.LogWarning("main.S is initialization order.");
            int ndx = Random.Range(0, S.powerUpFrequency.Length);
            eWeaponType pUpType = S.powerUpFrequency[ndx];

            GameObject go = Instantiate<GameObject>(S.prefabPowerUp);
            PowerUp pUp = go.GetComponent<PowerUp>();
            pUp.transform.position = e.transform.position;
        }
    }

    void SpawnBoss()
    {
        GameObject go = Instantiate<GameObject>(prefabBoss);

        float bossInset = enemyInsetDefault;
        if (go.GetComponent<BoundsCheck>() != null)
            bossInset = Mathf.Abs(go.GetComponent<BoundsCheck>().radius);

        // Bosses always spawn from the top, centred
        Vector3 pos = Vector3.zero;
        pos.x = 0f;
        pos.y = bndCheck.camHeight + bossInset;
        go.transform.position = pos;

        Enemy enemy = go.GetComponent<Enemy>();
        if (enemy != null)
        {
            float speedMultiplier = GetEnemySpeedMultiplier();
            enemy.speed *= speedMultiplier;
            enemy.health *= GetEnemyHealthMultiplier();
            enemy.SetMoveDirection(Vector3.down);
            enemy.SetDespawnEdge(BoundsCheck.eScreenLocs.offDown);
        }
    }
    // Start is called before the first frame update
}
