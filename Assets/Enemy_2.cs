using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_2 : Enemy
{
    [Header("Enemy_2 Inscribed Fields")]
    [Tooltip("Re-aim toward Hero this often (seconds). 0 = every frame.")]
    public float reAimInterval = 0.5f;
    [Header("Enemy_2 Private Fields")]
    private float nextReAimTime = 0f;

    void Start()
    {
        EnsureBoundsCheck();
        if (bndCheck == null) return;
        nextReAimTime = Time.time + reAimInterval;
        AimAtHero();
    }

    public override void Move()
    {
        base.Move();
        
        if (reAimInterval > 0f && Time.time >= nextReAimTime)
        {
            AimAtHero();
            nextReAimTime = Time.time + reAimInterval;
        }
    }

    void AimAtHero()
    {
        if (Hero.S == null) return;
        
        Vector2 toHero = Hero.S.transform.position - transform.position;
        if (toHero.sqrMagnitude < 0.001f) return;
        
        float angle = Mathf.Atan2(toHero.y, toHero.x) * Mathf.Rad2Deg;
        SetMoveDirection(new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad), 0f));
    }

}
