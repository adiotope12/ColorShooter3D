using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoundsCheck))]
public class PowerUp : MonoBehaviour
{
    public CoinCounter coinCounter;
    [Header("Inscribed")]
    public Vector2 rotMinMax = new Vector2(15, 90);
    public Vector2 driftMinMax = new Vector2(.25f, 2);
    public float lifeTime = 10f;
    public float fadeTime = 4f;

    [Header("Dynamic")]
    public eWeaponType type;
    public Vector3 rotPerSecond;
    public float birthTime;
    private Rigidbody rigid;
    private BoundsCheck bndCheck;
    private  Material cubeMat;

    void Start()
    {
        GameObject coinCounterGO = GameObject.Find("CoinCounter");
        coinCounter = coinCounterGO.GetComponent<CoinCounter>();
    }
    void Awake()
    {
        
        rigid = GetComponent<Rigidbody>();
        bndCheck = GetComponent<BoundsCheck>();

        Vector3 vel = Random.onUnitSphere;
        vel.z = 0;
        vel.Normalize();

        vel *= Random.Range(driftMinMax.x, driftMinMax.y);
        rigid.velocity = vel;

        transform.rotation = Quaternion.identity;
        
        rotPerSecond = new Vector3(Random.Range(rotMinMax[0], rotMinMax[1]), 
                                    Random.Range(rotMinMax[0], rotMinMax[1]), 
                                    Random.Range(rotMinMax[0], rotMinMax[1]));

        birthTime = Time.time;
    }

    void Update()
    {
        transform.rotation = Quaternion.Euler(rotPerSecond * Time.time);
        float u = (Time.time - (birthTime + lifeTime)) / fadeTime;
        if (u >= 1)
        {
            Destroy(this.gameObject);
            return;
        }

        if(!bndCheck.isOnScreen)
        {
            Destroy(this.gameObject);
        }
    }

    // public eWeaponType type
    // {
    //     get { return type; }
    //     set
    //     {
    //         SetType(value);
    //     }
    // }

    // public void SetType(eWeaponType wt)
    // {
    //     type = wt;
    //     WeaponDefinition def = main.GET_WEAPON_DEFINITION(type);
    //     cubeMat.color = def.powerUpColor;
    //     letter.text = def.letter;
    // }

    public void AbsorbedBy(GameObject target)
    {
        Debug.LogWarning("scheway");
        coinCounter.coins += main.GET_COIN_VALUE();
        Destroy(this.gameObject);
    }
}
