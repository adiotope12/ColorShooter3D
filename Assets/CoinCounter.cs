using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;



public class CoinCounter : MonoBehaviour
{
    private const string CoinsKey = "Coins";
    private static int sharedCoins = 0;
    private static bool hasLoadedCoins = false;

    static void EnsureLoaded()
    {
        if (hasLoadedCoins) return;
        sharedCoins = PlayerPrefs.GetInt(CoinsKey, 0);
        hasLoadedCoins = true;
    }

    public static int GetCoins()
    {
        EnsureLoaded();
        return sharedCoins;
    }

    public static void SetCoins(int value)
    {
        EnsureLoaded();
        sharedCoins = Mathf.Max(0, value);
        PlayerPrefs.SetInt(CoinsKey, sharedCoins);
    }

    public static bool TrySpendCoins(int cost)
    {
        EnsureLoaded();
        int clampedCost = Mathf.Max(0, cost);
        if (sharedCoins < clampedCost)
        {
            return false;
        }

        sharedCoins -= clampedCost;
        PlayerPrefs.SetInt(CoinsKey, sharedCoins);
        return true;
    }

    public int coins
    {
        get { return GetCoins(); }
        set { SetCoins(value); }
    }

    private TextMeshProUGUI uiText;

    void Awake()
    {
        EnsureLoaded();
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
