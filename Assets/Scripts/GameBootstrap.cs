using UnityEngine;

/// Monta uma cena mínima jogável em runtime: chão + player + câmera isométrica.
/// Serve pra provar o core do LOD2 sem montar tudo na mão no editor.
///
/// Uso: GameObject vazio na cena -> Add Component -> GameBootstrap -> Play.
/// Esperado: uma cápsula roxa (placeholder do mago) andando num campo verde,
/// com WASD e com o botão direito do mouse (click-to-move), câmera iso seguindo.
public class GameBootstrap : MonoBehaviour
{
    void Awake()
    {
        // --- chão (campo verde) ---
        var ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
        ground.name = "Ground";
        ground.transform.localScale = new Vector3(10, 1, 10);  // Plane 10x10 -> 100x100 m
        SetColor(ground, new Color(0.34f, 0.5f, 0.22f));

        // --- player (cápsula roxa, placeholder do mago) ---
        var player = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        player.name = "Player";
        player.transform.position = new Vector3(0, 1, 0);  // metade da altura da cápsula
        SetColor(player, new Color(0.5f, 0.25f, 0.7f));
        player.AddComponent<PlayerController>();

        // --- construção da cripta (set-piece billboard, ao norte) ---
        BuildBillboardSprite("crypt_entrance", new Vector3(0, 0, 8f), 120f);

        // --- câmera iso na Main Camera existente ---
        var cam = Camera.main;
        if (cam != null)
        {
            cam.fieldOfView = 25f;  // LOD usava fov 25 (perspectiva quase-orto)
            var follow = cam.GetComponent<CameraFollow>();
            if (follow == null) follow = cam.gameObject.AddComponent<CameraFollow>();
            follow.target = player.transform;
        }
        else
        {
            Debug.LogWarning("GameBootstrap: nenhuma Main Camera encontrada.");
        }
    }

    static void SetColor(GameObject go, Color c)
    {
        // instancia o material pra não alterar o material compartilhado
        go.GetComponent<Renderer>().material.color = c;
    }

    /// Cria um sprite billboard a partir de um PNG em Assets/Resources.
    /// Pivô na base (fica em pé no chão); `ppu` = pixels por unidade (tamanho).
    static void BuildBillboardSprite(string resourceName, Vector3 pos, float ppu)
    {
        var tex = Resources.Load<Texture2D>(resourceName);
        if (tex == null)
        {
            Debug.LogWarning($"GameBootstrap: '{resourceName}' não encontrado em Assets/Resources.");
            return;
        }
        var go = new GameObject(resourceName);
        go.transform.position = pos;
        var sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = Sprite.Create(
            tex, new Rect(0, 0, tex.width, tex.height),
            new Vector2(0.5f, 0f),  // pivô base-centro: assenta no chão
            ppu);
        go.AddComponent<Billboard>();
    }
}
