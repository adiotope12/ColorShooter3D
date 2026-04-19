using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_2 : Enemy
{
    [Header("Enemy_2 Inscribed Fields")]
    public float lifeTime =  10;
    [Tooltip("Determines how much the sine wave will ease the interpolation")]
    public float sinEccecntricity = 0.6f;
    [Tooltip("2D heading correction in degrees. Use 180 for backwards-facing sprites/models.")]
    public float headingOffsetZ = 0f;
    public AnimationCurve rotCurve;
    [Header("Enemy_2 Private Fields")]
    [SerializeField] private float birthTime;
    [SerializeField] private Quaternion baseRotation;
    [SerializeField] private Vector3 p0;
    [SerializeField] private Vector3 p1;

    void Start()
    {
        EnsureBoundsCheck();
        if (bndCheck == null) return;

        p0 = Vector3.zero;
        p0.x = -bndCheck.camWidth - bndCheck.radius;
        p0.y = Random.Range(-bndCheck.camHeight, bndCheck.camHeight);

        p1 = Vector3.zero;
        p1.x = bndCheck.camWidth + bndCheck.radius;
        p1.y = Random.Range(-bndCheck.camHeight, bndCheck.camHeight);

        if (Random.value > 0.5f)
        {
            p0.x *= -1;
            p1.x *= -1;
        }
        birthTime = Time.time;
        transform.position = p0;
        Vector2 heading = (p1 - p0);
        float angle = Mathf.Atan2(heading.y, heading.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle + headingOffsetZ);
        baseRotation = transform.rotation;
    }

    public override void Move()
    {
        float u = (Time.time - birthTime) / lifeTime;
        if (u > 1)
        {
            Destroy(this.gameObject);
            return;
        }

        float shipRot = rotCurve.Evaluate(u) * 360f;
        transform.rotation = baseRotation * Quaternion.Euler(0f, 0f, -shipRot);

        u = u + sinEccecntricity * Mathf.Sin(u * Mathf.PI * 2);
        pos = (1 - u) * p0 + u * p1;
    }

}
