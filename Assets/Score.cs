using UnityEngine;
using TMPro;

public class Score : MonoBehaviour
{
    [Header("Inscribed")]
    public float metersPerSecond = 1f;
    public string highScoreKey = "HighScoreMeters";

    [Header("Dynamic")]
    public float metersFlown = 0f;
    public int highScoreMeters = 0;

    private TextMeshProUGUI uiText;

    void Start()
    {
        uiText = GetComponent<TextMeshProUGUI>();
        highScoreMeters = PlayerPrefs.GetInt(highScoreKey, 0);
        UpdateScoreText();
    }

    void Update()
    {
        if (Hero.S != null)
        {
            metersFlown += metersPerSecond * Time.deltaTime;
        }

        int currentMeters = Mathf.FloorToInt(metersFlown);
        if (currentMeters > highScoreMeters)
        {
            highScoreMeters = currentMeters;
            PlayerPrefs.SetInt(highScoreKey, highScoreMeters);
        }

        UpdateScoreText();
    }

    void OnDisable()
    {
        PlayerPrefs.Save();
    }

    void OnApplicationQuit()
    {
        PlayerPrefs.Save();
    }

    void UpdateScoreText()
    {
        if (uiText == null) return;

        int currentMeters = Mathf.FloorToInt(metersFlown);
        uiText.text = "Score: " + currentMeters.ToString("#,0") + "\n"
            + "High Score: " + highScoreMeters.ToString("#,0");
    }
}
