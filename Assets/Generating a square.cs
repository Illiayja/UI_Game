using UnityEngine;
using UnityEngine.UI;

public class NeobrutalUIBuilder : MonoBehaviour
{
    [Header("Hierarchy")]
    public Transform parent;

    [Header("Block Dimensions")]
    public float width = 200f;
    public float height = 100f;

    [Header("Styling")]
    public float borderThickness = 8f;
    public Vector2 shadowOffset = new Vector2(8, -8);

    [Header("Corner Rounding Per Layer")]
    [Tooltip("Окреме заокруглення для кожного шару (Sliced спрайт з border)")]
    public float cornerMain = 1f;
    public float cornerBorder = 1f;
    public float cornerShadow = 1f;

    [Header("Image Settings (Reference Sprite)")]
    public Image referenceImage;

    void Start()
    {
        if (referenceImage == null)
        {
            Debug.LogError("❌ Reference Image is not assigned!");
            return;
        }

        Vector2 baseSize = new Vector2(width, height);

        GameObject root = new GameObject("NeobrutalBlock", typeof(RectTransform));
        root.transform.SetParent(parent, false);

        RectTransform rootRect = root.GetComponent<RectTransform>();
        rootRect.anchorMin = rootRect.anchorMax = rootRect.pivot = new Vector2(0.5f, 0.5f);
        rootRect.sizeDelta = baseSize;

        // 1. Shadow
        CreateLayer("Shadow", root.transform, baseSize, shadowOffset, Color.black, 0, cornerShadow);

        // 2. Border
        Vector2 borderSize = baseSize + Vector2.one * borderThickness;
        CreateLayer("Border", root.transform, borderSize, Vector2.zero, Color.black, 1, cornerBorder);

        // 3. Main
        CreateLayer("Main", root.transform, baseSize, Vector2.zero, Color.white, 2, cornerMain);
    }

    void CreateLayer(string name, Transform parent, Vector2 size, Vector2 offset, Color color, int order, float cornerMultiplier)
    {
        GameObject obj = new GameObject(name, typeof(Image));
        obj.transform.SetParent(parent, false);

        RectTransform rect = obj.GetComponent<RectTransform>();
        rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = size;
        rect.anchoredPosition = offset;

        Image img = obj.GetComponent<Image>();

        // Apply reference settings
        img.sprite = referenceImage.sprite;
        img.type = Image.Type.Sliced;
        img.material = referenceImage.material;
        img.preserveAspect = referenceImage.preserveAspect;
        img.fillCenter = referenceImage.fillCenter;
        img.raycastTarget = referenceImage.raycastTarget;
        img.maskable = referenceImage.maskable;
        img.useSpriteMesh = referenceImage.useSpriteMesh;


        img.color = color;
        img.pixelsPerUnitMultiplier = cornerMultiplier;

        obj.transform.SetSiblingIndex(order);
    }
}
