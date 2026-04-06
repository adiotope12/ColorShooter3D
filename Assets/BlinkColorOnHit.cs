using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class BlinkColorOnHit : MonoBehaviour
{
    private static float blinkDuration = 0.1f;
    private static Color blinkColor = Color.red;

    [Header("Dynamic")]
    public bool showingColor = false;
    public float blinkCompleteTime;
    public bool ignoreCollisionEnter = false;

    private Material[] materials;
    private Color[] originalColors;
    private BoundsCheck bndCheck;

    void Awake()
    {
        bndCheck = GetComponent<BoundsCheck>();
        materials = Utils.GetAllMaterials(gameObject);
        originalColors = new Color[materials.Length];
        for (int i = 0; i < materials.Length; i++)
        {
            originalColors[i] = materials[i].color;
        }
    }
    // Start is called before the first frame update


    // Update is called once per frame
    void Update()
    {
        if (showingColor && Time.time > blinkCompleteTime) RevertColors();
    }

    void OnCollisionEnter(Collision coll)
    {
        if (ignoreCollisionEnter) return;
        ProjectileHero p = coll.gameObject.GetComponent<ProjectileHero>();
        if (p != null)
        {
            if (bndCheck != null && !bndCheck.isOnScreen) return;
            SetColors();
        }
    }

    public void SetColors()
    {
        showingColor = true;
        blinkCompleteTime = Time.time + blinkDuration;
        for (int i = 0; i < materials.Length; i++)
        {
            materials[i].color = blinkColor;
        }
    }

    public void RevertColors()
    {
        showingColor = false;
        for (int i = 0; i < materials.Length; i++)
        {
            materials[i].color = originalColors[i];
        }
    }
}
