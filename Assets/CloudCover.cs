using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudCover : MonoBehaviour
{
    [Header("Inscribed")]
    public Sprite[] cloudSprites;
    public int  numClouds = 40;
    public Vector3 minPos = new Vector3(-20, -5, -5);
    public Vector3 maxPos = new Vector3(300, 40, 5);
    public float speed = 10f;

    [Tooltip("For scaleRange, x is the min value and y is the max value")]
    public Vector2 scaleRange = new Vector2(1, 4);

    [Header("Dynamic")]
    public List<Transform> clouds = new List<Transform>();
    public List<SpriteRenderer> renderers = new List<SpriteRenderer>();

    private float camTop;
    private float camBottom;

    // Start is called before the first frame update
    void Start()
    {
        float camHeight = Camera.main.orthographicSize;
        camTop = camHeight;
        camBottom = -camHeight;

        Transform parentTrans = this.transform;
        GameObject cloudGO;
        Transform cloudTrans;
        SpriteRenderer sRend;
        float scaleMult;
        for (int i = 0; i < numClouds; i++)
        {
            cloudGO = new GameObject();
            cloudTrans = cloudGO.transform;
            sRend = cloudGO.AddComponent<SpriteRenderer>();
            int spriteNum = Random.Range(0, cloudSprites.Length);
            sRend.sprite = cloudSprites[spriteNum];
            cloudTrans.position = RandomPos();
            cloudTrans.SetParent(parentTrans,true);
            scaleMult = Random.Range(scaleRange.x, scaleRange.y);
            cloudTrans.localScale = Vector3.one * scaleMult;
            clouds.Add(cloudTrans);
            renderers.Add(sRend);
            // cloudTrans.localPosition = new Vector3(Random.Range(minPos.x, maxPos.x), Random.Range(minPos.y, maxPos.y), Random.Range(minPos.z, maxPos.z));
            // cloudTrans.localPosition = Vector3.zero;
        }
        
    }

    Vector3 RandomPos()
    {
        return new Vector3(Random.Range(minPos.x, maxPos.x), Random.Range(minPos.y, maxPos.y), Random.Range(minPos.z, maxPos.z));
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < clouds.Count; i++)
        {
            Transform cloudTrans = clouds[i];
            SpriteRenderer cloudRenderer = renderers[i];
            Vector3 pos = cloudTrans.position;
            pos.y -= speed * Time.deltaTime;

            float cloudHalfHeight = cloudRenderer.bounds.extents.y;

            if (pos.y < camBottom - cloudHalfHeight)
            {
                pos = new Vector3(
                    Random.Range(minPos.x, maxPos.x),
                    camTop + cloudHalfHeight,
                    Random.Range(minPos.z, maxPos.z)
                );

                int spriteNum = Random.Range(0, cloudSprites.Length);
                cloudRenderer.sprite = cloudSprites[spriteNum];
                float scaleMult = Random.Range(scaleRange.x, scaleRange.y);
                cloudTrans.localScale = Vector3.one * scaleMult;
            }

            cloudTrans.position = pos;
        }
    }
}
