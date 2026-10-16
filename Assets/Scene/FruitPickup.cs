using UnityEngine;

public class FruitPickup : MonoBehaviour
{
    public int scoreValue = 10;
    private bool collected;

    private void OnTriggerEnter2D(Collider2D other)
    {
        TryCollect(other.gameObject);
    }

    
private void OnCollisionEnter2D(Collision2D collision)
{
    if (collision.gameObject.CompareTag("Ground"))
    {
        GameData.combo = 0;
        Debug.Log("Fruit hit ground! Combo reset to 0.");
        Destroy(gameObject);
        return;
    }

    TryCollect(collision.gameObject);
}

    private void TryCollect(GameObject other)
    {
        if (collected || other == null)
            return;

        if (!other.CompareTag("Player"))
            return;

        collected = true;

        // เพิ่มคะแนน
        GameData.score += scoreValue;

        // เพิ่ม Combo
        GameData.combo++;

        GameData.UpdateBestScore();

        // แสดงข้อความใน Console
        ShowComboMessage();

        Debug.Log(
            "Picked fruit: +" + scoreValue +
            " score | Combo: " + GameData.combo
        );

        Destroy(gameObject);
    }

    private void ShowComboMessage()
{
    if (ComboFeedback.Instance == null)
        return;

    if (GameData.combo == 1)
    {
        ComboFeedback.Instance.ShowMessage("NICE!");
    }
    else if (GameData.combo == 2)
    {
        ComboFeedback.Instance.ShowMessage("EXCELLENT!");
    }
    else if (GameData.combo >= 3)
    {
        ComboFeedback.Instance.ShowMessage("PERFECT!");
    }
}

}