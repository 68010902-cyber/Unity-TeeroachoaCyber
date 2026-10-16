using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BackgroundSelector : MonoBehaviour
{
    private const string BackgroundObjectName = "4fc23384-9456-4464-be09-29c2b3edff2d_0";

    [SerializeField] private TMP_Dropdown levelDropdownTemplate;
    [SerializeField] private TMP_Dropdown backgroundDropdown;
    [SerializeField] private Sprite[] additionalBackgrounds;

    private readonly List<Sprite> availableBackgrounds = new List<Sprite>();
    private SpriteRenderer previewRenderer;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void RegisterSceneLoaded()
    {
        SceneManager.sceneLoaded -= ApplySelectedBackgroundToScene;
        SceneManager.sceneLoaded += ApplySelectedBackgroundToScene;
    }

    private static void ApplySelectedBackgroundToScene(Scene scene, LoadSceneMode mode)
    {
        Sprite selectedBackground = GameData.selectedBackgroundSprite;
        if (selectedBackground == null)
            return;

        foreach (GameObject rootObject in scene.GetRootGameObjects())
        {
            SpriteRenderer[] renderers = rootObject.GetComponentsInChildren<SpriteRenderer>(true);
            foreach (SpriteRenderer renderer in renderers)
            {
                if (renderer.gameObject.name == BackgroundObjectName)
                    renderer.sprite = selectedBackground;
            }
        }
    }

    private void Start()
    {
        previewRenderer = GetComponent<SpriteRenderer>();
        if (previewRenderer == null)
        {
            Debug.LogError("BackgroundSelector needs a SpriteRenderer on the same GameObject.", this);
            return;
        }

        if (backgroundDropdown == null)
        {
            GameObject existingDropdown = GameObject.Find("BackgroundDropdown");
            if (existingDropdown != null)
                backgroundDropdown = existingDropdown.GetComponent<TMP_Dropdown>();
        }

        List<Sprite> optionSprites = new List<Sprite>();
        if (backgroundDropdown != null)
        {
            foreach (TMP_Dropdown.OptionData option in backgroundDropdown.options)
            {
                if (option.image != null && !optionSprites.Contains(option.image))
                    optionSprites.Add(option.image);
            }
        }

        if (backgroundDropdown == null && levelDropdownTemplate != null)
        {
            backgroundDropdown = Instantiate(
                levelDropdownTemplate.gameObject,
                levelDropdownTemplate.transform.parent).GetComponent<TMP_Dropdown>();
            backgroundDropdown.gameObject.name = "BackgroundDropdown";
            backgroundDropdown.GetComponent<RectTransform>().anchoredPosition += Vector2.down * 42f;
        }

        if (backgroundDropdown == null)
        {
            Debug.LogError("Create a TMP Dropdown named BackgroundDropdown.", this);
            return;
        }

        Sprite defaultBackground = previewRenderer.sprite;
        if (defaultBackground != null)
            availableBackgrounds.Add(defaultBackground);

        if (additionalBackgrounds != null)
        {
            foreach (Sprite background in additionalBackgrounds)
            {
                if (background != null && !availableBackgrounds.Contains(background))
                    availableBackgrounds.Add(background);
            }
        }

        foreach (Sprite background in optionSprites)
        {
            if (!availableBackgrounds.Contains(background))
                availableBackgrounds.Add(background);
        }

        if (availableBackgrounds.Count == 0)
            return;

        int selectedIndex = availableBackgrounds.IndexOf(GameData.selectedBackgroundSprite);
        if (selectedIndex < 0)
            selectedIndex = 0;

        backgroundDropdown.onValueChanged.RemoveListener(SelectBackground);
        backgroundDropdown.ClearOptions();

        List<string> labels = new List<string> { "Default background" };
        for (int index = 1; index < availableBackgrounds.Count; index++)
            labels.Add(availableBackgrounds[index].name);

        backgroundDropdown.AddOptions(labels);
        backgroundDropdown.SetValueWithoutNotify(selectedIndex);
        backgroundDropdown.RefreshShownValue();
        backgroundDropdown.onValueChanged.AddListener(SelectBackground);
        SelectBackground(selectedIndex);
    }

    private void SelectBackground(int index)
    {
        if (index < 0 || index >= availableBackgrounds.Count)
            return;

        GameData.selectedBackgroundSprite = availableBackgrounds[index];
        previewRenderer.sprite = GameData.selectedBackgroundSprite;
    }
}