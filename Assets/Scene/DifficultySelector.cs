using UnityEngine;
using UnityEngine.UI;

public class DifficultySelector : MonoBehaviour
{
    public Dropdown difficultyDropdown;
    public FruitSpawner fruitSpawner;

    private void Start()
    {
        if (difficultyDropdown == null)
            return;

        difficultyDropdown.onValueChanged.AddListener(SetDifficulty);
        SetDifficulty(difficultyDropdown.value);
    }

    public void SetDifficulty(int index)
    {
        if (fruitSpawner == null)
            return;

        switch (index)
        {
            case 0:
                fruitSpawner.speedLevel = FruitSpeedLevel.Easy;
                break;
            case 1:
                fruitSpawner.speedLevel = FruitSpeedLevel.Normal;
                break;
            case 2:
                fruitSpawner.speedLevel = FruitSpeedLevel.Hard;
                break;
        }
    }
}
