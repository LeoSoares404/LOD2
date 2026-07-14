using UnityEngine;

/// Animação por spritesheet do mago (5 colunas × 3 linhas), porte do
/// _update_animation de player.gd. Col 0 = parado; col 1-4 = ciclo de andar.
/// Linhas: 0 = frente (baixo), 1 = costas (cima), 2 = lado (direita).
[RequireComponent(typeof(SpriteRenderer), typeof(PlayerController))]
public class PlayerSpriteAnimator : MonoBehaviour
{
    public string sheetName = "mago_walk";
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
        _frames = SpriteUtil.Slice(sheetName, 5, 3, ppu);
        if (_frames != null)
            _sr.sprite = _frames[ROW_DOWN, 0];
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
                _sr.flipX = v.x < 0f;  // encara o lado do movimento
            }
            else
            {
                _facingRow = v.z > 0f ? ROW_UP : ROW_DOWN;  // +z = costas (norte)
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
