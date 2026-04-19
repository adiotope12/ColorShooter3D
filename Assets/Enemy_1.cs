using System.Collections;
using System.Collections.Generic;
using System.Runtime.Versioning;
using UnityEngine;

public class Enemy_1 : Enemy
{
    [Header("Enemy_1 Inscribed Fields")]
    [Tooltip("# of secondsd for a full sine  wave")]
    public float waveFrequency = 2f;
    [Tooltip("Sine wave width in meters")]
    public float waveWidth = 4f;
    [Tooltip("Amount the ship will roll left and right with the sine wave")]
    public float waveRotY = 45f;
    [Tooltip("Model heading correction in degrees. 180 flips a backwards model.")]
    public float headingOffsetY = 180f;

    private float x0;
    private float birthTime;
    private Quaternion spawnFacingRotation;

    void Start()
    {
        x0 = pos.x;
        birthTime = Time.time;
        spawnFacingRotation = transform.rotation * Quaternion.Euler(0f, headingOffsetY, 0f);
    }

    public override void Move()
    {
        Vector3 tempPos = pos;
        float age = Time.time - birthTime;
        float theta = Mathf.PI * 2f * age / waveFrequency;
        float sin = Mathf.Sin(theta);
        tempPos.x = x0 + waveWidth * sin;
        pos = tempPos;

        Vector3 rot = new Vector3(0, sin * waveRotY, 0);
        this.transform.rotation = spawnFacingRotation * Quaternion.Euler(rot);
        base.Move();
    }

    public override void SetMoveDirection(Vector3 newDirection)
    {
        base.SetMoveDirection(newDirection);
        spawnFacingRotation = transform.rotation * Quaternion.Euler(0f, headingOffsetY, 0f);
    }
    

}
