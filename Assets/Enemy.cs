using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoundsCheck))]
public class Enemy : MonoBehaviour
{
    [Header("Inscribed")]
    public float speed = 10f;
    public float fireRate = 0.3f;
    public float health = 10;
    public int score = 100;
    public float powerUpDropChance = 1f;
    protected bool calledShipDestroyed = false;
    [SerializeField] protected Vector3 moveDirection = Vector3.down;
    [SerializeField] protected BoundsCheck.eScreenLocs despawnEdge = BoundsCheck.eScreenLocs.offDown;
    protected Quaternion initialRotation;
    public Vector3 pos {
        get { return this.transform.position; }
        set { this.transform.position = value; }
    }
    // Update is called once per frame
    void Update()
    {
        EnsureBoundsCheck();
        if (bndCheck == null) return;
        Move();
        if(bndCheck.LocIs(despawnEdge))
        {
            Destroy(this.gameObject);
        }
    }

    protected BoundsCheck bndCheck;
    void Awake()
    {
        EnsureBoundsCheck();
        initialRotation = transform.rotation;
    }

    protected void EnsureBoundsCheck()
    {
        if (bndCheck != null) return;
        bndCheck = GetComponent<BoundsCheck>();
        if (bndCheck == null)
        {
            bndCheck = gameObject.AddComponent<BoundsCheck>();
            Debug.LogWarning(name + " was missing BoundsCheck. Added one automatically.");
        }
    }

    public virtual void Move()
    {
        Vector3 tempPos = pos;
        Vector3 dir = moveDirection.sqrMagnitude > 0f ? moveDirection.normalized : Vector3.down;
        tempPos += dir * speed * Time.deltaTime;
        pos = tempPos;
    }

    public virtual void SetMoveDirection(Vector3 newDirection)
    {
        if (newDirection.sqrMagnitude > 0f)
        {
            moveDirection = newDirection.normalized;
            Quaternion rotationOffset = Quaternion.FromToRotation(Vector3.down, moveDirection);
            transform.rotation = rotationOffset * initialRotation;
        }
    }

    public virtual void SetDespawnEdge(BoundsCheck.eScreenLocs newDespawnEdge)
    {
        despawnEdge = newDespawnEdge;
    }

    void OnCollisionEnter(Collision coll)
    {
        GameObject otherGO = coll.gameObject;
        ProjectileHero p = otherGO.GetComponent<ProjectileHero>();
        if(p != null)
        {
            if(bndCheck.isOnScreen)
            {
                health -= main.GET_PROJECTILE_DAMAGE(p.type);
                if (health <= 0)
                {
                    if(!calledShipDestroyed) 
                    {
                        Debug.Log(this);
                        calledShipDestroyed = true;
                        main.SHIP_DESTROYED(this);
                    }
                    Destroy(this.gameObject);
                }
            }
            Debug.Log(health);
            Destroy(otherGO);
         }
        else
        {
            Debug.Log("Enemy hitt by non-ProjectileHero: " + otherGO.name);
        }
        
    }
}
