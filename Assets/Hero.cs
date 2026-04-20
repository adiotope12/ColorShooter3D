using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Cryptography;
using UnityEngine;

public class Hero : MonoBehaviour
{
    static public Hero S { get; private set; } // Singleton

    [Header("Inscribed")]
    public float speed = 30;
    public float baseRotationSpeed = 150f;
    public float rollMult = -45;
    public float pitchMult = 30;
    public GameObject projectilePrefab;
    public float projectileSpeed = 40;
    public Weapon[] weapons;

    [Header("Dynamic")] [Range(0, 20)]
    public float _shieldLevel = 1;
    public float maxShieldLevel = 4;
    [Tooltip("This field holds a  referrence to  the last triggering GameObject")]
    public GameObject lastTriggerGo = null;
    public delegate void WeaponFireDelegate();
    public WeaponFireDelegate fireEvent;

    private float baseMoveSpeed;
    private float baseMaxShieldLevel;
    private float appliedRotationSpeed;
    private float baseProjectileSpeed;

     void Awake()
    {
        if (S == null)
        {
            S = this; // Set the singleton
        }
        else
        {
            Debug.LogError("Hero.Awake() - Attempted to assign second Hero.S!");
        }

        baseMoveSpeed = speed;
        baseMaxShieldLevel = maxShieldLevel;
        baseProjectileSpeed = projectileSpeed;
        ApplyUpgradeStats();

        ClearWeapons();
        
        weapons[0].SetType(eWeaponType.blaster);
    }

    // Update is called once per frame
    void Update()
    {
        float hAxis = Input.GetAxis("Horizontal");
        float vAxis = Input.GetAxis("Vertical");

        Vector3 pos = transform.position;
        pos.x += hAxis * speed * Time.deltaTime;
        pos.y += vAxis * speed * Time.deltaTime;
        transform.position = pos;

        if(Input.GetAxis("Jump") == 1 && fireEvent != null)
        {
            fireEvent();
        }

// Rotate left (A)
        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(0, 0, appliedRotationSpeed * Time.deltaTime);
        }

        // Rotate right (F)
        if (Input.GetKey(KeyCode.F))
        {
            transform.Rotate(0, 0, -appliedRotationSpeed * Time.deltaTime);
        }
    }

    void ApplyUpgradeStats()
    {
        int hpLevel = UpgradeButton.GetStoredUpgradeLevel("HP", 0);
        int speedLevel = UpgradeButton.GetStoredUpgradeLevel("Speed", 0);
        int movementLevel = UpgradeButton.GetStoredUpgradeLevel("Movement", 0);
        int rotationLevel = UpgradeButton.GetStoredUpgradeLevel("Rotation", 0);
        int projectileSpeedLevel = UpgradeButton.GetStoredUpgradeLevel("Projectile Speed", 0);

        // +1 to base stat for each upgrade level.
        speed = baseMoveSpeed + speedLevel + movementLevel;
        maxShieldLevel = baseMaxShieldLevel + hpLevel;
        appliedRotationSpeed = baseRotationSpeed + rotationLevel;
        projectileSpeed = baseProjectileSpeed + projectileSpeedLevel;
        shieldLevel = maxShieldLevel;
    }
    
    void OnTriggerEnter(Collider other){
        Transform rootT = other.gameObject.transform.root;
        GameObject go = rootT.gameObject;
        if (go == lastTriggerGo) return;
        lastTriggerGo = go;

        Enemy enemy = go.GetComponent<Enemy>();
        PowerUp pUp = go.GetComponent<PowerUp>();
        if (enemy != null)
        {
            shieldLevel--;
            Destroy(go);
        } else if (pUp != null)
        {
            AbsorbPowerUp(pUp);
        }
         else
        {
           Debug.LogWarning("Triggered by non-Enemy: " + go.name);
        }
    }

    public void AbsorbPowerUp(PowerUp pUp)
    {
        pUp.AbsorbedBy(this.gameObject);
    }

    public float shieldLevel
    {
        get { return _shieldLevel; }
        set
        {
            _shieldLevel = Mathf.Min(value, maxShieldLevel);
            if (value < 0)
            {
                Destroy(this.gameObject);
                main.HERO_DIED();
            }
        }
    }  

    Weapon GetEmptyWeapon()
    {
        for (int i = 0; i < weapons.Length; i++)
        {
            if (weapons[i].type == eWeaponType.none)
            {
                return weapons[i];
            }
        }
        return null;
    }

    void ClearWeapons()
    {
        foreach (Weapon w in weapons)
        {
            w.SetType(eWeaponType.none);
        }
    }


}
