using UnityEngine;

/// Monta o jogo inteiro em runtime: mundo aberto (hub) + cripta com ondas +
/// player (mago) + HUD + menus. Uso: GameObject vazio → GameBootstrap → Play.
///
/// Fluxo: nasce no campo → entra na construção da cripta → teleporta pra
/// cripta interna e as ondas começam → porta do sul volta pro mundo.
public class GameBootstrap : MonoBehaviour
{
    // cripta interna deslocada pra longe do mundo aberto
    static readonly Vector3 CryptCenter = new Vector3(60f, 0f, 60f);
    const float CryptW = 34f;
    const float CryptH = 20f;

    void Awake()
    {
        BuildOverworld();
        BuildCrypt();
        var player = BuildPlayer();
        BuildCamera(player);
        BuildDoors(player);
        var hud = HUD.Build(player);
        PauseMenu.Build();
        InventoryMenu.Build();
        WirePlayerDeath(player, hud);
    }

    void BuildOverworld()
    {
        var ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
        ground.name = "Ground";
        ground.transform.localScale = new Vector3(10, 1, 10);   // 100x100 m
        SetColor(ground, new Color(0.34f, 0.5f, 0.22f));

        BuildBillboardSprite("crypt_entrance", new Vector3(0, 0, 8f), 120f);
    }

    void BuildCrypt()
    {
        // chão com a textura da cripta do LOD
        var floor = GameObject.CreatePrimitive(PrimitiveType.Plane);
        floor.name = "CryptFloor";
        floor.transform.position = CryptCenter;
        floor.transform.localScale = new Vector3(CryptW / 10f, 1f, CryptH / 10f);
        var rend = floor.GetComponent<Renderer>();
        var tex = Resources.Load<Texture2D>("crypt_floor");
        if (tex != null)
        {
            rend.material.mainTexture = tex;
            rend.material.mainTextureScale = new Vector2(CryptW / 7.5f, CryptH / 7.5f);  // LOD: tile de 7,5 m
            rend.material.color = new Color(0.8f, 0.9f, 0.85f);
        }
        else
        {
            SetColor(floor, new Color(0.1f, 0.16f, 0.15f));
        }

        // paredes (visuais)
        BuildWall(new Vector3(CryptCenter.x, 1f, CryptCenter.z + CryptH / 2f), new Vector3(CryptW + 2f, 2f, 1f));
        BuildWall(new Vector3(CryptCenter.x, 1f, CryptCenter.z - CryptH / 2f), new Vector3(CryptW + 2f, 2f, 1f));
        BuildWall(new Vector3(CryptCenter.x - CryptW / 2f, 1f, CryptCenter.z), new Vector3(1f, 2f, CryptH));
        BuildWall(new Vector3(CryptCenter.x + CryptW / 2f, 1f, CryptCenter.z), new Vector3(1f, 2f, CryptH));

        // porta de volta (arco de dia do LOD) na parede sul
        BuildBillboardSprite("door", new Vector3(CryptCenter.x, 0f, CryptCenter.z - CryptH / 2f + 1.2f), 16f);

        // gerente de ondas da cripta
        var wm = new GameObject("WaveManager").AddComponent<WaveManager>();
        wm.bounds = new Rect(CryptCenter.x - CryptW / 2f, CryptCenter.z - CryptH / 2f, CryptW, CryptH);
    }

    void BuildWall(Vector3 pos, Vector3 size)
    {
        var wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
        wall.name = "Wall";
        wall.transform.position = pos;
        wall.transform.localScale = size;
        SetColor(wall, new Color(0.16f, 0.2f, 0.22f));
    }

    GameObject BuildPlayer()
    {
        var player = new GameObject("Player");
        player.transform.position = Vector3.zero;
        player.AddComponent<SpriteRenderer>();
        player.AddComponent<Billboard>();
        player.AddComponent<PlayerController>();
        player.AddComponent<PlayerSpriteAnimator>();
        var health = player.AddComponent<Health>();
        health.SetMax(20);   // vida do mago (LOD)
        player.AddComponent<PlayerCombat>();
        return player;
    }

    void BuildCamera(GameObject player)
    {
        var cam = Camera.main;
        if (cam == null)
        {
            Debug.LogWarning("GameBootstrap: nenhuma Main Camera na cena.");
            return;
        }
        cam.fieldOfView = 25f;   // perspectiva quase-orto do LOD
        var follow = cam.GetComponent<CameraFollow>();
        if (follow == null)
            follow = cam.gameObject.AddComponent<CameraFollow>();
        follow.target = player.transform;
        follow.Snap();
    }

    void BuildDoors(GameObject player)
    {
        // mundo → cripta (boca da construção)
        var enter = new GameObject("DoorToCrypt").AddComponent<DoorTrigger>();
        enter.transform.position = new Vector3(0f, 0f, 7.7f);
        enter.destination = CryptCenter + new Vector3(0f, 0f, -CryptH / 2f + 3.5f);
        enter.onUse = () =>
        {
            if (WaveManager.Instance != null)
                WaveManager.Instance.EnterCrypt();
        };

        // cripta → mundo (porta da parede sul)
        var exit = new GameObject("DoorToOverworld").AddComponent<DoorTrigger>();
        exit.transform.position = new Vector3(CryptCenter.x, 0f, CryptCenter.z - CryptH / 2f + 1.2f);
        exit.destination = new Vector3(0f, 0f, 5.2f);
    }

    void WirePlayerDeath(GameObject player, HUD hud)
    {
        var health = player.GetComponent<Health>();
        var pc = player.GetComponent<PlayerController>();
        health.Died += () =>
        {
            // respawn no mundo aberto com vida cheia (LOD recarregava a cena)
            player.transform.position = Vector3.zero;
            pc.CancelMove();
            var cam = Camera.main != null ? Camera.main.GetComponent<CameraFollow>() : null;
            if (cam != null)
                cam.Snap();
            health.HealFull();
            if (hud != null)
                hud.Banner("VOCÊ MORREU", new Color(1f, 0.35f, 0.3f), 2.5f);
        };
    }

    static void SetColor(GameObject go, Color c)
    {
        go.GetComponent<Renderer>().material.color = c;
    }

    /// Sprite billboard a partir de um PNG em Assets/Resources (pivô na base).
    static void BuildBillboardSprite(string resourceName, Vector3 pos, float ppu)
    {
        var sprite = SpriteUtil.Whole(resourceName, ppu);
        if (sprite == null) return;
        var go = new GameObject(resourceName);
        go.transform.position = pos;
        var sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = sprite;
        go.AddComponent<Billboard>();
    }
}
