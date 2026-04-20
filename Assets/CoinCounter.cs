using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;



public class CoinCounter : MonoBehaviour
{
    private const string CoinsKey = "Coins";
    private static int sharedCoins = 0;
    private static bool hasLoadedCoins = false;

    public int coins
    {
        get { return sharedCoins; }
        set
        {
            sharedCoins = Mathf.Max(0, value);
            PlayerPrefs.SetInt(CoinsKey, sharedCoins);
        }
    }

    private TextMeshProUGUI uiText;

    void Awake()
    {
        if (!hasLoadedCoins)
        {
            sharedCoins = PlayerPrefs.GetInt(CoinsKey, 0);
            hasLoadedCoins = true;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
       uiText = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        uiText.text = "Coins: " + coins.ToString("#,0");
    }

    void OnDisable()
    {
        PlayerPrefs.Save();
    }

    void OnApplicationQuit()
    {
        PlayerPrefs.Save();
    }
}
