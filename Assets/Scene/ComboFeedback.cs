
using UnityEngine;
using TMPro;
using System.Collections;

public class ComboFeedback : MonoBehaviour
{
    public static ComboFeedback Instance;

    public TMP_Text comboText;

    private Coroutine animationCoroutine;
    private Vector3 originalScale;

    private void Awake()
    {
        Instance = this;

        if (comboText != null)
        {
            originalScale = comboText.rectTransform.localScale;
            comboText.gameObject.SetActive(false);
        }
    }

    public void ShowMessage(string message)
    {
        if (comboText == null)
            return;

        // หยุด Animation เก่าก่อนแสดงข้อความใหม่
        if (animationCoroutine != null)
            StopCoroutine(animationCoroutine);

        // กำหนดสีตามข้อความ
        Color messageColor;

        switch (message)
        {
            case "NICE!":
                messageColor = new Color(0.2f, 1f, 0.4f);
                break;

            case "EXCELLENT!":
                messageColor = new Color(1f, 0.65f, 0.1f);
                break;

            default:
                messageColor = new Color(0.75f, 0.4f, 1f);
                break;
        }

        comboText.text = message;
        comboText.color = messageColor;
        comboText.gameObject.SetActive(true);

        animationCoroutine = StartCoroutine(PlayAnimation());
    }

    private IEnumerator PlayAnimation()
    {
        RectTransform rect = comboText.rectTransform;

        // เริ่มจากตัวเล็ก
        rect.localScale = originalScale * 0.5f;

        Color color = comboText.color;
        color.a = 1f;
        comboText.color = color;

        // ขยายให้ใหญ่ขึ้น
        float duration = 0.15f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;

            float t = Mathf.Clamp01(elapsed / duration);
            float scale = Mathf.Lerp(0.5f, 1.25f, t);

            rect.localScale = originalScale * scale;

            yield return null;
        }

        // หดกลับสู่ขนาดปกติ
        elapsed = 0f;

        while (elapsed < 0.12f)
        {
            elapsed += Time.unscaledDeltaTime;

            float t = Mathf.Clamp01(elapsed / 0.12f);
            float scale = Mathf.Lerp(1.25f, 1f, t);

            rect.localScale = originalScale * scale;

            yield return null;
        }

        rect.localScale = originalScale;

        // แสดงข้อความไว้ก่อนจางหาย
        yield return new WaitForSecondsRealtime(0.7f);

        // ค่อย ๆ จางหาย
        elapsed = 0f;
        duration = 0.3f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;

            float alpha = 1f - Mathf.Clamp01(elapsed / duration);

            color = comboText.color;
            color.a = alpha;
            comboText.color = color;

            yield return null;
        }

        comboText.gameObject.SetActive(false);
        rect.localScale = originalScale;
    }
}