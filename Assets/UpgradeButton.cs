using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpgradeButton : MonoBehaviour
{
    private const string UpgradeKeyPrefix = "UpgradeLevel_";
    private const int BaseUpgradeCost = 20;
    private const int CostIncreasePerLevel = 5;

    [Header("Inscribed")]
    [SerializeField] private string upgradeType = "Damage";
    [SerializeField] private int upgradeLevel = 1;
    [SerializeField] private TMP_Text label;
    [SerializeField] private Button button;

    void Awake()
    {
        if (button == null)
        {
            button = GetComponent<Button>();
        }

        if (button != null)
        {
            button.onClick.AddListener(OnButtonPressed);
        }

        LoadLevel();
        RefreshLabel();
        RefreshButtonState();
    }

    void OnDestroy()
    {
        if (button != null)
        {
            button.onClick.RemoveListener(OnButtonPressed);
        }
    }

    void OnValidate()
    {
        if (upgradeLevel < 1)
        {
            upgradeLevel = 1;
        }

        RefreshLabel();
    }

    void Update()
    {
        RefreshButtonState();
    }

    public void OnButtonPressed()
    {
        int cost = GetCurrentUpgradeCost();
        if (!CoinCounter.TrySpendCoins(cost))
        {
            RefreshButtonState();
            return;
        }

        upgradeLevel++;
        SaveLevel();
        RefreshLabel();
        RefreshButtonState();
    }

    public void SetUpgradeType(string newType)
    {
        upgradeType = newType;
        LoadLevel();
        RefreshLabel();
    }

    public int GetUpgradeLevel()
    {
        return upgradeLevel;
    }

    public int GetCurrentUpgradeCost()
    {
        return BaseUpgradeCost + ((upgradeLevel - 1) * CostIncreasePerLevel);
    }

    public static int GetStoredUpgradeLevel(string type, int defaultLevel = 1)
    {
        string key = GetUpgradeKey(type);
        return Mathf.Max(1, PlayerPrefs.GetInt(key, defaultLevel));
    }

    public static void SetStoredUpgradeLevel(string type, int level)
    {
        string key = GetUpgradeKey(type);
        PlayerPrefs.SetInt(key, Mathf.Max(1, level));
        PlayerPrefs.Save();
    }

    public static void ResetStoredUpgradeLevel(string type)
    {
        string key = GetUpgradeKey(type);
        PlayerPrefs.DeleteKey(key);
        PlayerPrefs.Save();
    }

    void LoadLevel()
    {
        upgradeLevel = GetStoredUpgradeLevel(upgradeType, upgradeLevel);
    }

    void SaveLevel()
    {
        SetStoredUpgradeLevel(upgradeType, upgradeLevel);
    }

    static string GetUpgradeKey(string type)
    {
        string safeType = string.IsNullOrWhiteSpace(type) ? "Default" : type.Trim();
        return UpgradeKeyPrefix + safeType;
    }

    void RefreshLabel()
    {
        if (label == null)
        {
            return;
        }

        label.text = "Upgrade " + upgradeType + "\n(LV. " + upgradeLevel + ")";
    }

    void RefreshButtonState()
    {
        if (button == null)
        {
            return;
        }

        button.interactable = CoinCounter.GetCoins() >= GetCurrentUpgradeCost();
    }
}
