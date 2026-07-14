using UnityEngine;
using UnityEngine.UI;

/// Inventário (I ou botão da mochila): moldura dark-fantasy do LOD;
/// arma inicial da classe listada. Não pausa o jogo (como no LOD).
public class InventoryMenu : MonoBehaviour
{
    public static InventoryMenu Instance;
    public static bool IsOpen => Instance != null && Instance._open;

    GameObject _panel;
    bool _open;

    public static void Build()
    {
        var go = new GameObject("InventoryMenu");
        var canvas = go.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 20;
        var scaler = go.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1280, 720);
        go.AddComponent<GraphicRaycaster>();

        var inv = go.AddComponent<InventoryMenu>();
        Instance = inv;
        inv.BuildPanel();
    }

    void BuildPanel()
    {
        _panel = new GameObject("Panel");
        var prt = _panel.AddComponent<RectTransform>();
        prt.SetParent(transform, false);
        prt.anchorMin = Vector2.zero;
        prt.anchorMax = Vector2.one;
        prt.offsetMin = Vector2.zero;
        prt.offsetMax = Vector2.zero;

        // escurece o fundo; clicar fora fecha
        var dim = _panel.AddComponent<Image>();
        dim.color = new Color(0f, 0f, 0f, 0.82f);
        var dimBtn = _panel.AddComponent<Button>();
        dimBtn.transition = Selectable.Transition.None;
        dimBtn.onClick.AddListener(() => Close());

        // moldura do inventário (944x1680 → retrato)
        var frameGo = new GameObject("Frame");
        var frt = frameGo.AddComponent<RectTransform>();
        frt.SetParent(_panel.transform, false);
        frt.sizeDelta = new Vector2(360, 640);
        var frame = frameGo.AddComponent<Image>();
        frame.sprite = SpriteUtil.Ui("inventory_frame");
        frame.preserveAspect = true;
        frame.raycastTarget = false;

        // título na plaquinha da moldura
        var titleGo = new GameObject("Title");
        var trt = titleGo.AddComponent<RectTransform>();
        trt.SetParent(frameGo.transform, false);
        trt.anchoredPosition = new Vector2(0, 232);
        trt.sizeDelta = new Vector2(300, 40);
        var title = titleGo.AddComponent<Text>();
        title.font = HUD.UiFont;
        title.fontSize = 22;
        title.fontStyle = FontStyle.Bold;
        title.alignment = TextAnchor.MiddleCenter;
        title.color = new Color(0.85f, 0.72f, 0.42f);
        title.text = "INVENTÁRIO";
        title.raycastTarget = false;

        // arma inicial da classe (porte simplificado do sistema de itens)
        var itemGo = new GameObject("Item0");
        var irt = itemGo.AddComponent<RectTransform>();
        irt.SetParent(frameGo.transform, false);
        irt.anchoredPosition = new Vector2(0, 150);
        irt.sizeDelta = new Vector2(300, 26);
        var item = itemGo.AddComponent<Text>();
        item.font = HUD.UiFont;
        item.fontSize = 15;
        item.alignment = TextAnchor.MiddleCenter;
        item.color = new Color(0.9f, 0.85f, 0.7f);
        item.text = "Arma: Cajado Arcano";
        item.raycastTarget = false;

        _panel.SetActive(false);
    }

    public static void Toggle()
    {
        if (Instance != null)
            Instance.DoToggle();
    }

    public static void Close()
    {
        if (Instance != null && Instance._open)
            Instance.DoToggle();
    }

    void DoToggle()
    {
        _open = !_open;
        _panel.SetActive(_open);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
            DoToggle();
    }
}
