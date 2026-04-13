using UnityEngine;
using UnityEngine.UI;

public class HpBar : MonoBehaviour
{
    public Image fillImage;
    public Color healthyColor = Color.green;
    public Color lowHealthColor = Color.red;
    private float heroMaxHealth;
    private bool initialized;
    private float initialWidth;
    private float initialAnchoredPosX;
    private float initialPivotX;

    void Start()
    {
        TryInitialize();
        UpdateHealthBar();
    }

    void Update()
    {
        if (!initialized)
        {
            TryInitialize();
        }

        UpdateHealthBar();
    }

    void TryInitialize()
    {
        if (fillImage == null || Hero.S == null) return;

        // Capture starting hero health as the max so it isn't hardcoded.
        heroMaxHealth = Mathf.Max(0.01f, Hero.S.shieldLevel);

        // Force one-sided depletion via the image fill system.
        fillImage.type = Image.Type.Filled;
        fillImage.fillMethod = Image.FillMethod.Horizontal;
        fillImage.fillOrigin = (int)Image.OriginHorizontal.Right;

        RectTransform rt = fillImage.rectTransform;
        initialWidth = rt.rect.width;
        initialAnchoredPosX = rt.anchoredPosition.x;
        initialPivotX = rt.pivot.x;

        initialized = true;
    }

    void UpdateHealthBar()
    {
        if (!initialized || fillImage == null) return;

        if (Hero.S == null)
        {
            SetBarValue(0f);
            return;
        }

        // Hero dies below 0, so offset health by +1 for UI representation.
        float currentHealthForBar = Hero.S.shieldLevel + 1f;
        float maxHealthForBar = heroMaxHealth + 1f;
        float normalizedHealth = Mathf.Clamp01(currentHealthForBar / maxHealthForBar);
        SetBarValue(normalizedHealth);
    }

    void SetBarValue(float normalizedHealth)
    {
        fillImage.fillAmount = normalizedHealth;
        fillImage.color = Color.Lerp(healthyColor, lowHealthColor, 1f - normalizedHealth);

        // Fallback: resize width and keep right edge fixed so depletion appears from the left.
        ApplyWidthFallback(normalizedHealth);
    }

    void ApplyWidthFallback(float normalizedHealth)
    {
        RectTransform rt = fillImage.rectTransform;

        float newWidth = initialWidth * normalizedHealth;
        rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, newWidth);

        float initialRightEdgeX = initialAnchoredPosX + (1f - initialPivotX) * initialWidth;
        Vector2 anchoredPos = rt.anchoredPosition;
        anchoredPos.x = initialRightEdgeX - (1f - initialPivotX) * newWidth;
        rt.anchoredPosition = anchoredPos;
    }
}