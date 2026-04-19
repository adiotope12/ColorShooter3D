using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;  
using UnityEngine;
using System.Dynamic;

public class main : MonoBehaviour
{
    static private main S;
    static private Dictionary<eWeaponType, WeaponDefinition> WEAP_DICT;
    [Header("Inscribed")]
    public bool spawnEnemies = true;
    public GameObject[] prefabEnemy;
    public float enemySpawnPerSecond = 0.5f;
    public float enemyInsetDefault = 1.5f;
    public float gameRestartDelay = 2f;
    public GameObject prefabPowerUp;
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



    private BoundsCheck bndCheck;

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
        Invoke("SpawnEnemy", 1f / enemySpawnPerSecond);

        WEAP_DICT = new Dictionary<eWeaponType, WeaponDefinition>();
        foreach (WeaponDefinition def in weaponDefinitions)        {
            WEAP_DICT[def.type] = def;
        }
    }

    public void SpawnEnemy()
    {
        if (!spawnEnemies)
        {
            Invoke (nameof(SpawnEnemy), 1f / enemySpawnPerSecond);
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
            enemy.SetMoveDirection(moveDirection);
            enemy.SetDespawnEdge(despawnEdge);
        }

        Invoke("SpawnEnemy", 1f / enemySpawnPerSecond);
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
    // Start is called before the first frame update
}
