using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RetrySceneController : MonoBehaviour
{
    [SerializeField] private TMP_Text playerText;
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text bestScoreText;

    private void Start()
    {
        CreateBackgroundImage();

        if (playerText != null)
            playerText.text = "Player: " + GameData.username;
        if (levelText != null)
            levelText.text = "Level: " + GameData.selectedLevel;
        if (scoreText != null)
            scoreText.text = "Score: " + GameData.score;
        if (bestScoreText != null)
            bestScoreText.text = "Best Score: " + GameData.bestScore;
    }

    private void CreateBackgroundImage()
    {
        if (GameData.selectedBackgroundSprite == null)
            return;

        GameObject backgroundObject = new GameObject("RetryBackground", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        backgroundObject.transform.SetParent(transform, false);
        backgroundObject.transform.SetAsFirstSibling();

        RectTransform backgroundRect = backgroundObject.GetComponent<RectTransform>();
        backgroundRect.anchorMin = Vector2.zero;
        backgroundRect.anchorMax = Vector2.one;
        backgroundRect.offsetMin = Vector2.zero;
        backgroundRect.offsetMax = Vector2.zero;

        Image backgroundImage = backgroundObject.GetComponent<Image>();
        backgroundImage.sprite = GameData.selectedBackgroundSprite;
        backgroundImage.raycastTarget = false;
    }
}