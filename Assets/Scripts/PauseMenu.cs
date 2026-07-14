using UnityEngine;
using UnityEngine.UI;

/// Menu de pausa (ESC): moldura pixel-art do LOD + Continuar / Sair do jogo.
public class PauseMenu : MonoBehaviour
{
    public static PauseMenu Instance;

    GameObject _panel;
    bool _open;

    public static void Build()
    {
        var go = new GameObject("PauseMenu");
        var canvas = go.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 30;
        var scaler = go.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1280, 720);
        go.AddComponent<GraphicRaycaster>();

        var pm = go.AddComponent<PauseMenu>();
        Instance = pm;
        pm.BuildPanel();
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
        _panel.AddComponent<Image>().color = new Color(0f, 0f, 0f, 0.8f);

        // moldura do LOD
        var frameGo = new GameObject("Frame");
        var frt = frameGo.AddComponent<RectTransform>();
        frt.SetParent(_panel.transform, false);
        frt.sizeDelta = new Vector2(380, 380);
        var frame = frameGo.AddComponent<Image>();
        frame.sprite = SpriteUtil.Ui("pause_frame");
        frame.preserveAspect = true;
        frame.raycastTarget = false;

        MakeButton("Continuar", new Vector2(0, 26), () => Toggle());
        MakeButton("Sair do jogo", new Vector2(0, -34), () =>
        {
            Application.Quit();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        });

        _panel.SetActive(false);
    }

    void MakeButton(string label, Vector2 pos, System.Action onClick)
    {
        var go = new GameObject("Btn" + label);
        var rt = go.AddComponent<RectTransform>();
        rt.SetParent(_panel.transform, false);
        rt.anchoredPosition = pos;
        rt.sizeDelta = new Vector2(170, 38);

        var img = go.AddComponent<Image>();
        img.color = new Color(0.16f, 0.12f, 0.08f, 0.95f);
        var btn = go.AddComponent<Button>();
        btn.onClick.AddListener(() => onClick());

        var txtGo = new GameObject("Text");
        var trt = txtGo.AddComponent<RectTransform>();
        trt.SetParent(go.transform, false);
        trt.anchorMin = Vector2.zero;
        trt.anchorMax = Vector2.one;
        trt.offsetMin = Vector2.zero;
        trt.offsetMax = Vector2.zero;
        var t = txtGo.AddComponent<Text>();
        t.font = HUD.UiFont;
        t.fontSize = 16;
        t.alignment = TextAnchor.MiddleCenter;
        t.color = new Color(0.85f, 0.72f, 0.42f);   // dourado da UI do LOD
        t.text = label;
        t.raycastTarget = false;
    }

    void Update()
    {
        if (!Input.GetKeyDown(KeyCode.Escape)) return;
        if (InventoryMenu.IsOpen)
        {
            InventoryMenu.Close();   // ESC fecha o inventário primeiro
            return;
        }
        Toggle();
    }

    public void Toggle()
    {
        _open = !_open;
        _panel.SetActive(_open);
        Time.timeScale = _open ? 0f : 1f;
    }
}
