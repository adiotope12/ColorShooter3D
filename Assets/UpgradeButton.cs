using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpgradeButton : MonoBehaviour
{
    private const string UpgradeKeyPrefix = "UpgradeLevel_";

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

    public void OnButtonPressed()
    {
        upgradeLevel++;
        SaveLevel();
        RefreshLabel();
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
}
