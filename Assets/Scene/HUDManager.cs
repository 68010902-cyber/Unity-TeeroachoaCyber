using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
    [Header("HUD Text")]
    public Text usernameText;
    public Text scoreText;
    public Text levelText;

    private void Update()
    {
        if (usernameText != null)
            usernameText.text = "USERNAME: " + GameData.username;

        if (scoreText != null)
            scoreText.text = "SCORE: " + GameData.score;

        if (levelText != null)
            levelText.text = "LEVEL: " + GameData.selectedLevel.ToString();
    }
}
