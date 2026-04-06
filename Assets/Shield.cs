using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour
{
    [Header("Inscribed")]
    public float rotationsPerSecond = 0.1f;

    [Header("Dynamic")]
    public int levelShown = 0;

    Material mat;
    // Start is called before the first frame update
    void Start()
    {
        mat = GetComponent<Renderer>().material;
        
    }

    // Update is called once per frame
    void Update()
    {int currLevel = Mathf.FloorToInt(Hero.S.shieldLevel);
        if (levelShown != currLevel)
        {
            mat.mainTextureOffset = new Vector2(0.2f * currLevel, 0);
            levelShown = currLevel;
        }
        float rZ = rotationsPerSecond * Time.time * 360;
        transform.rotation = Quaternion.Euler(0, 0, rZ);
        
    }
}
