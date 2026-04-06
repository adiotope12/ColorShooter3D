using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundsCheck : MonoBehaviour
{
    [System.Flags]
    public enum eScreenLocs
    {
        onScreen = 0,
        offRight = 1,
        offLeft = 2,
        offUp = 4,
        offDown = 8
    }
    public enum eType {center, inset, outset};
    [Header("Inscribed")]
    public eType boundsType = eType.inset;
    public float radius = 1f;
    public bool keepOnScreen = true;
    [Header("Dynamic")]
    public eScreenLocs screenLoc = eScreenLocs.onScreen;
    public float camWidth;
    public float camHeight;

    void Awake()
    {
        camHeight = Camera.main.orthographicSize;
        camWidth = camHeight * Camera.main.aspect;
    }

    void LateUpdate()
    {
        float checkRadius = 0;
        if(boundsType == eType.inset)
        {
            checkRadius = -radius;
        }
        else if(boundsType == eType.outset)
        {
            checkRadius = radius;
        }
        Vector3 pos = transform.position;
        screenLoc = eScreenLocs.onScreen;

        
        if (pos.x > camWidth + checkRadius)     {
            pos.x = camWidth + checkRadius;
            screenLoc |= eScreenLocs.offRight;
        }
        if (pos.x < -camWidth - checkRadius)    {
            pos.x = -camWidth - checkRadius;
            screenLoc |= eScreenLocs.offLeft;
        }

        if (pos.y > camHeight + checkRadius)    {
            pos.y = camHeight + checkRadius;
            screenLoc |= eScreenLocs.offUp;
        }
        if (pos.y < -camHeight - checkRadius)   {
            pos.y = -camHeight - checkRadius;
            screenLoc |= eScreenLocs.offDown;
        }

        if (keepOnScreen && !isOnScreen)
        {
            transform.position = pos;
            screenLoc = eScreenLocs.onScreen;
        }
    }

    public bool isOnScreen
    {
        get {return screenLoc == eScreenLocs.onScreen;}
    }

    public bool LocIs(eScreenLocs checkLoc)
    {
        if (checkLoc == eScreenLocs.onScreen)
        {
            return screenLoc == eScreenLocs.onScreen;
        }
        else
        {
            return (screenLoc & checkLoc) == checkLoc;
        }

    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
