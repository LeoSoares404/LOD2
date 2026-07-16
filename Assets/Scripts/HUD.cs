using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// HUD construída por código (porte do hud.tscn): orbes de vida/mana com a
/// arte do LOD, hotbar Q/W/E/R com cooldown radial, contador de ondas,
/// banner central e botão da mochila.
public class HUD : MonoBehaviour
{
    public static HUD Instance;

    static Font s_font;
    public static Font UiFont
    {
        get
        {
            if (s_font == null)
                s_font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            return s_font;
        }
    }

    Health _health;
    PlayerCombat _combat;
    Image _healthFill, _manaFill, _xpFill;
    readonly Image[] _cd = new Image[4];
    Text _counter, _banner, _scheme, _gold, _level;
    float _bannerT, _bannerHold;
    Color _bannerColor = Color.white;

    public static HUD Build(GameObject player)
    {
        // EventSystem (necessário pros botões da UI)
        if (Object.FindFirstObjectByType<EventSystem>() == null)
        {
            var es = new GameObject("EventSystem");
            es.AddComponent<EventSystem>();
            es.AddComponent<StandaloneInputModule>();
        }

        var go = new GameObject("HUD");
        var canvas = go.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 10;
        var scaler = go.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1280, 720);
        go.AddComponent<GraphicRaycaster>();

        var hud = go.AddComponent<HUD>();
        Instance = hud;
        hud._health = player.GetComponent<Health>();
        hud._combat = player.GetComponent<PlayerCombat>();
        hud.BuildElements();
        return hud;
    }

    void BuildElements()
    {
        // --- orbe de vida (esq.) e mana (dir.): arte por trás escura + fill vertical ---
        var orbHp = SpriteUtil.Ui("orb_health");
        var orbMp = SpriteUtil.Ui("orb_mana");
        NewImage("HpBg", new Vector2(0.5f, 0f), new Vector2(-250, 70), new Vector2(110, 110), orbHp, new Color(0.2f, 0.1f, 0.1f, 0.95f));
        _healthFill = NewImage("HpFill", new Vector2(0.5f, 0f), new Vector2(-250, 70), new Vector2(110, 110), orbHp, Color.white);
        MakeVerticalFill(_healthFill);
        NewImage("MpBg", new Vector2(0.5f, 0f), new Vector2(250, 70), new Vector2(110, 110), orbMp, new Color(0.1f, 0.1f, 0.2f, 0.95f));
        _manaFill = NewImage("MpFill", new Vector2(0.5f, 0f), new Vector2(250, 70), new Vector2(110, 110), orbMp, Color.white);
        MakeVerticalFill(_manaFill);

        // --- hotbar Q/W/E/R com ícone + overlay radial de cooldown ---
        string[] icons = { "skill_q", "skill_w", "skill_e", "skill_r" };
        string[] letters = { "Q", "W", "E", "R" };
        for (int i = 0; i < 4; i++)
        {
            float x = -87f + i * 58f;
            NewImage("SlotBg" + i, new Vector2(0.5f, 0f), new Vector2(x, 64), new Vector2(54, 54), null, new Color(0f, 0f, 0f, 0.6f));
            NewImage("SlotIcon" + i, new Vector2(0.5f, 0f), new Vector2(x, 64), new Vector2(50, 50), SpriteUtil.Ui(icons[i]), Color.white);
            _cd[i] = NewImage("SlotCd" + i, new Vector2(0.5f, 0f), new Vector2(x, 64), new Vector2(50, 50), null, new Color(0f, 0f, 0f, 0.65f));
            _cd[i].type = Image.Type.Filled;
            _cd[i].fillMethod = Image.FillMethod.Radial360;
            _cd[i].fillOrigin = (int)Image.Origin360.Top;
            _cd[i].fillClockwise = false;
            _cd[i].fillAmount = 0f;
            var letter = NewText("SlotKey" + i, new Vector2(0.5f, 0f), new Vector2(x + 18f, 48f), new Vector2(20, 16), 12, TextAnchor.LowerRight);
            letter.text = letters[i];
            letter.color = new Color(1f, 1f, 1f, 0.9f);
        }

        // --- contador de ondas (topo) ---
        _counter = NewText("WaveCounter", new Vector2(0.5f, 1f), new Vector2(0, -34), new Vector2(400, 30), 18, TextAnchor.MiddleCenter);
        _counter.text = "";

        // --- banner central (RODADA X / CHEFE / VITÓRIA) ---
        _banner = NewText("Banner", new Vector2(0.5f, 0.5f), new Vector2(0, 120), new Vector2(800, 60), 40, TextAnchor.MiddleCenter);
        _banner.fontStyle = FontStyle.Bold;
        _banner.color = Color.clear;

        // --- esquema de controle (topo-esq.) ---
        _scheme = NewText("Scheme", new Vector2(0f, 1f), new Vector2(210, -20), new Vector2(400, 24), 12, TextAnchor.MiddleLeft);

        // --- ouro (topo-dir.) ---
        _gold = NewText("Gold", new Vector2(1f, 1f), new Vector2(-110, -20), new Vector2(200, 24), 16, TextAnchor.MiddleRight);
        _gold.color = new Color(1f, 0.85f, 0.25f);
        _gold.text = "Ouro: 0";

        // --- barra de XP + nível (base da tela) ---
        NewImage("XpBg", new Vector2(0.5f, 0f), new Vector2(0, 16), new Vector2(420, 7), null, new Color(0f, 0f, 0f, 0.55f));
        _xpFill = NewImage("XpFill", new Vector2(0.5f, 0f), new Vector2(0, 16), new Vector2(420, 7), null, new Color(0.45f, 0.75f, 1f, 0.9f));
        _xpFill.type = Image.Type.Filled;
        _xpFill.fillMethod = Image.FillMethod.Horizontal;
        _xpFill.fillOrigin = (int)Image.OriginHorizontal.Left;
        _xpFill.fillAmount = 0f;
        _level = NewText("Level", new Vector2(0.5f, 0f), new Vector2(-238, 16), new Vector2(70, 20), 13, TextAnchor.MiddleRight);
        _level.text = "Nv 1";

        // --- botão da mochila (inferior-dir.) ---
        var bagImg = NewImage("Backpack", new Vector2(1f, 0f), new Vector2(-52, 60), new Vector2(64, 64), SpriteUtil.Ui("inventory_backpack"), Color.white);
        bagImg.raycastTarget = true;   // NewImage desliga por padrão; botão precisa
        var btn = bagImg.gameObject.AddComponent<Button>();
        btn.onClick.AddListener(() => InventoryMenu.Toggle());
    }

    static void MakeVerticalFill(Image img)
    {
        img.type = Image.Type.Filled;
        img.fillMethod = Image.FillMethod.Vertical;
        img.fillOrigin = (int)Image.OriginVertical.Bottom;
        img.fillAmount = 1f;
    }

    Image NewImage(string name, Vector2 anchor, Vector2 pos, Vector2 size, Sprite sprite, Color color)
    {
        var rt = NewRect(name, anchor, pos, size);
        var img = rt.gameObject.AddComponent<Image>();
        img.sprite = sprite;
        img.color = color;
        if (sprite != null)
            img.preserveAspect = true;
        img.raycastTarget = false;
        return img;
    }

    Text NewText(string name, Vector2 anchor, Vector2 pos, Vector2 size, int fontSize, TextAnchor align)
    {
        var rt = NewRect(name, anchor, pos, size);
        var t = rt.gameObject.AddComponent<Text>();
        t.font = UiFont;
        t.fontSize = fontSize;
        t.alignment = align;
        t.color = Color.white;
        t.horizontalOverflow = HorizontalWrapMode.Overflow;
        t.verticalOverflow = VerticalWrapMode.Overflow;
        t.raycastTarget = false;
        return t;
    }

    RectTransform NewRect(string name, Vector2 anchor, Vector2 pos, Vector2 size)
    {
        var go = new GameObject(name);
        var rt = go.AddComponent<RectTransform>();
        rt.SetParent(transform, false);
        rt.anchorMin = anchor;
        rt.anchorMax = anchor;
        rt.anchoredPosition = pos;
        rt.sizeDelta = size;
        return rt;
    }

    void Update()
    {
        if (_health != null && _health.maxHealth > 0)
            _healthFill.fillAmount = (float)_health.Current / _health.maxHealth;
        if (_combat != null)
        {
            _manaFill.fillAmount = _combat.ManaFraction;
            for (int i = 0; i < 4; i++)
                _cd[i].fillAmount = _combat.CooldownFraction(i);
        }
        if (_gold != null)
            _gold.text = $"Ouro: {GameState.Gold}";
        if (_level != null)
            _level.text = $"Nv {GameState.Level}";
        if (_xpFill != null)
            _xpFill.fillAmount = Mathf.Clamp01((float)GameState.Xp / GameState.XpToNext());
        if (_scheme != null)
            _scheme.text = GameState.ControlScheme == "mouse"
                ? "Clássico: andar = botão direito · F1 troca"
                : "Moderno: WASD anda (skills Q/E/C/R) · F1 troca";

        if (_bannerT > 0f)
        {
            _bannerT -= Time.deltaTime;
            float fadeOut = Mathf.Clamp01(_bannerT / 0.6f);
            float fadeIn = Mathf.Clamp01((_bannerHold - _bannerT) / 0.3f);
            var c = _bannerColor;
            c.a = Mathf.Min(fadeOut, fadeIn);
            _banner.color = c;
        }
    }

    public void Banner(string text, Color color, float hold = 2.2f)
    {
        _banner.text = text;
        _bannerColor = color;
        _bannerHold = hold;
        _bannerT = hold;
    }

    public void SetWaveCounter(string text)
    {
        if (_counter != null)
            _counter.text = text;
    }
}
