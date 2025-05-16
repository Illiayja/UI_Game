using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class NeobrutalUIButtonBuilder : MonoBehaviour
{
    [Header("Hierarchy")]
    public Transform parent;

    [Header("Button Dimensions")]
    public float width = 200f;
    public float height = 100f;

    [Header("Styling")]
    public float borderThickness = 8f;
    public Vector2 shadowOffset = new Vector2(8, -8);

    [Header("Corner Rounding Per Layer")]
    public float cornerMain = 1f;
    public float cornerBorder = 1f;
    public float cornerShadow = 1f;

    [Header("Image Settings (Reference Sprite)")]
    public Image referenceImage;

    [Header("Text Settings")]
    public string buttonText = "Button";
    public Font textFont;
    public int fontSize = 28;
    public Color textColor = Color.black;
    public FontStyle fontStyle = FontStyle.Bold;
    public TextAnchor alignment = TextAnchor.MiddleCenter;

    [Header("Button Animation (Instant Change)")]
    public Color pressedColor = new Color(0.9f, 0.9f, 0.9f, 1f);
    public Vector2 pressedMainOffset = new Vector2(0, -4);
    public Vector2 pressedBorderOffset = new Vector2(0, -2);

    [Header("Scene Settings")]
    public string sceneToLoad;

    // Runtime refs
    private RectTransform mainRect;
    private RectTransform borderRect;
    private Image mainImage;
    private Vector2 originalMainPos;
    private Vector2 originalBorderPos;
    private Color originalMainColor;

    void Start()
    {
        if (referenceImage == null)
        {
            Debug.LogError("❌ Reference Image is not assigned!");
            return;
        }

        Vector2 baseSize = new Vector2(width, height);

        GameObject root = new GameObject("NeobrutalButton", typeof(RectTransform), typeof(CanvasRenderer), typeof(Button));
        root.transform.SetParent(parent, false);

        RectTransform rootRect = root.GetComponent<RectTransform>();
        rootRect.anchorMin = rootRect.anchorMax = rootRect.pivot = new Vector2(0.5f, 0.5f);
        rootRect.sizeDelta = baseSize;

        Button btn = root.GetComponent<Button>();
        btn.transition = Selectable.Transition.None;

        // Додаємо прозорий Image для обробки кліків
        Image bg = root.AddComponent<Image>();
        bg.color = new Color(0, 0, 0, 0); // прозорий фон
        bg.raycastTarget = true;

        // Scene switch on click
        if (!string.IsNullOrEmpty(sceneToLoad))
        {
            btn.onClick.AddListener(() =>
            {
                SceneManager.LoadScene(sceneToLoad);
            });
        }
        Vector2 borderSize = baseSize + Vector2.one * borderThickness;

        // Shadow
        CreateLayer("Shadow", root.transform, borderSize, shadowOffset, Color.black, cornerShadow, 0);

        // Border
        GameObject border = CreateLayer("Border", root.transform, borderSize, Vector2.zero, Color.black, cornerBorder, 1);
        borderRect = border.GetComponent<RectTransform>();
        originalBorderPos = borderRect.anchoredPosition;

        // Main
        GameObject main = CreateLayer("Main", root.transform, baseSize, Vector2.zero, Color.white, cornerMain, 2);
        mainRect = main.GetComponent<RectTransform>();
        originalMainPos = mainRect.anchoredPosition;
        mainImage = main.GetComponent<Image>();
        originalMainColor = mainImage.color;

        // Text
        CreateTextElement(root.transform, baseSize);

        // Add instant change animation logic
        EventTrigger trigger = root.AddComponent<EventTrigger>();
        AddInstantButtonAnimations(trigger);
    }

    GameObject CreateLayer(string name, Transform parent, Vector2 size, Vector2 offset, Color color, float cornerMultiplier, int order)
    {
        GameObject obj = new GameObject(name, typeof(Image));
        obj.transform.SetParent(parent, false);

        RectTransform rect = obj.GetComponent<RectTransform>();
        rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = size;
        rect.anchoredPosition = offset;

        Image img = obj.GetComponent<Image>();
        img.sprite = referenceImage.sprite;
        img.type = Image.Type.Sliced;
        img.material = referenceImage.material;
        img.preserveAspect = referenceImage.preserveAspect;
        img.fillCenter = referenceImage.fillCenter;
        img.raycastTarget = false;
        img.maskable = referenceImage.maskable;
        img.useSpriteMesh = referenceImage.useSpriteMesh;

        img.color = color;
        img.pixelsPerUnitMultiplier = cornerMultiplier;

        obj.transform.SetSiblingIndex(order);
        return obj;
    }

    void CreateTextElement(Transform parent, Vector2 size)
    {
        GameObject textObj = new GameObject("Text", typeof(Text));
        textObj.transform.SetParent(parent, false);

        RectTransform rect = textObj.GetComponent<RectTransform>();
        rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = size;
        rect.anchoredPosition = Vector2.zero;

        Text text = textObj.GetComponent<Text>();
        text.text = buttonText;
        text.font = textFont != null ? textFont : Resources.GetBuiltinResource<Font>("Arial.ttf");
        text.fontSize = fontSize;
        text.color = textColor;
        text.alignment = alignment;
        text.fontStyle = fontStyle;
        text.raycastTarget = false;
    }

    void AddInstantButtonAnimations(EventTrigger trigger)
    {
        AddTrigger(trigger, EventTriggerType.PointerDown, () =>
        {
            mainImage.color = pressedColor;
            mainRect.anchoredPosition = originalMainPos + pressedMainOffset;
            borderRect.anchoredPosition = originalBorderPos + pressedBorderOffset;
        });

        AddTrigger(trigger, EventTriggerType.PointerUp, () =>
        {
            mainImage.color = originalMainColor;
            mainRect.anchoredPosition = originalMainPos;
            borderRect.anchoredPosition = originalBorderPos;
        });

        AddTrigger(trigger, EventTriggerType.PointerExit, () =>
        {
            mainImage.color = originalMainColor;
            mainRect.anchoredPosition = originalMainPos;
            borderRect.anchoredPosition = originalBorderPos;
        });
    }

    void AddTrigger(EventTrigger trigger, EventTriggerType type, UnityEngine.Events.UnityAction action)
    {
        EventTrigger.Entry entry = new EventTrigger.Entry { eventID = type };
        entry.callback.AddListener((eventData) => action());
        trigger.triggers.Add(entry);
    }
}
