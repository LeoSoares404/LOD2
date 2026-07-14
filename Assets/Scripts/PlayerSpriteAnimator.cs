using UnityEngine;

/// Animação por spritesheet do mago (5 colunas × 3 linhas), porte do
/// _update_animation de player.gd. Col 0 = parado; col 1-4 = ciclo de andar.
/// Linhas: 0 = frente (baixo), 1 = costas (cima), 2 = lado (direita).
[RequireComponent(typeof(SpriteRenderer), typeof(PlayerController))]
public class PlayerSpriteAnimator : MonoBehaviour
{
    public string sheetName = "mago_walk";
    public int cols = 5;
    public int rows = 3;
    public float ppu = 16f;   // 16 px = 1 m (como no LOD)
    public float fps = 8f;    // LOD: ANIM_FPS

    const int ROW_DOWN = 0, ROW_UP = 1, ROW_SIDE = 2;

    SpriteRenderer _sr;
    PlayerController _pc;
    Sprite[,] _frames;
    float _animTime;
    int _facingRow = ROW_DOWN;

    void Start()
    {
        _sr = GetComponent<SpriteRenderer>();
        _pc = GetComponent<PlayerController>();
        SliceSheet();
        if (_frames != null)
            _sr.sprite = _frames[ROW_DOWN, 0];
    }

    void SliceSheet()
    {
        var tex = Resources.Load<Texture2D>(sheetName);
        if (tex == null)
        {
            Debug.LogWarning($"PlayerSpriteAnimator: '{sheetName}' não encontrado em Assets/Resources.");
            return;
        }
        int cw = tex.width / cols;
        int ch = tex.height / rows;
        _frames = new Sprite[rows, cols];
        for (int r = 0; r < rows; r++)
            for (int c = 0; c < cols; c++)
            {
                // imagem: linha 0 = topo; textura Unity tem y=0 embaixo -> inverte
                var rect = new Rect(c * cw, tex.height - (r + 1) * ch, cw, ch);
                _frames[r, c] = Sprite.Create(tex, rect, new Vector2(0.5f, 0f), ppu);
            }
    }

    void Update()
    {
        if (_frames == null) return;

        Vector3 v = _pc.Velocity;
        bool walking = _pc.IsMoving && v.magnitude > 0.1f;

        if (walking)
        {
            if (Mathf.Abs(v.x) > Mathf.Abs(v.z))
            {
                _facingRow = ROW_SIDE;
                _sr.flipX = v.x < 0;  // encara o lado do movimento
            }
            else
            {
                _facingRow = v.z > 0 ? ROW_UP : ROW_DOWN;  // +z = costas (norte/cima)
            }
            _animTime += Time.deltaTime;
        }
        else
        {
            _animTime = 0f;
        }

        int col = walking ? 1 + (int)(_animTime * fps) % 4 : 0;
        _sr.sprite = _frames[_facingRow, col];
    }
}
