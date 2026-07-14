using System;
using UnityEngine;

/// Inimigo perseguidor (ghoul e variantes: sprinter/boss por cor/escala).
/// Persegue o player enquanto ele estiver dentro do leash (área da cripta);
/// dano por toque com cooldown. Porte do ghoul.gd do LOD.
[RequireComponent(typeof(Health))]
public class Enemy : MonoBehaviour
{
    public float speed = 2.2f;
    public int touchDamage = 3;
    public float attackCooldown = 0.8f;
    public Rect leash;                 // limites XZ onde pode caçar (vazio = sempre)
    public Action<Enemy> OnDeath;

    Health _health;
    Transform _player;
    Health _playerHealth;
    SpriteRenderer _sr;
    Sprite[,] _frames;                 // 1 linha × 5 colunas (0 parado, 1-4 andando)
    float _animT;
    float _atkTimer;
    float _stun;
    float _flash;
    Color _tint = Color.white;
    Vector3 _jitter;                   // offset por inimigo pra não empilharem

    void Awake()
    {
        _health = GetComponent<Health>();
        _health.Died += Die;
    }

    void Start()
    {
        var pc = UnityEngine.Object.FindFirstObjectByType<PlayerController>();
        if (pc != null)
        {
            _player = pc.transform;
            _playerHealth = pc.GetComponent<Health>();
        }
    }

    /// Monta o visual (sprite billboard filho) — chamar logo após AddComponent.
    public void Setup(Sprite[,] frames, Color tint, float scale)
    {
        _frames = frames;
        _tint = tint;
        var spriteGo = new GameObject("Sprite");
        spriteGo.transform.SetParent(transform, false);
        spriteGo.transform.localScale = Vector3.one * scale;
        _sr = spriteGo.AddComponent<SpriteRenderer>();
        _sr.color = tint;
        if (_frames != null)
            _sr.sprite = _frames[0, 0];
        spriteGo.AddComponent<Billboard>();
        _jitter = new Vector3(UnityEngine.Random.Range(-0.7f, 0.7f), 0f, UnityEngine.Random.Range(-0.7f, 0.7f));
    }

    public void Stun(float duration) => _stun = Mathf.Max(_stun, duration);

    /// Dano vindo do player (skills/projéteis) — com número e flash.
    public void Hit(int damage)
    {
        if (_health.IsDead) return;
        _health.TakeDamage(damage);
        _flash = 0.15f;
        DamageNumber.Spawn(transform.position + Vector3.up * 1.5f, damage.ToString(), new Color(1f, 0.9f, 0.4f));
    }

    void Update()
    {
        // tinta: flash de dano > stun > normal
        if (_flash > 0f)
        {
            _flash -= Time.deltaTime;
            if (_sr != null) _sr.color = _flash > 0f ? new Color(1f, 0.35f, 0.35f) : _tint;
        }
        if (_stun > 0f)
        {
            _stun -= Time.deltaTime;
            if (_sr != null && _flash <= 0f)
                _sr.color = _stun > 0f ? new Color(0.6f, 0.75f, 1f) : _tint;
            return;   // atordoado: não anda nem ataca
        }

        if (_atkTimer > 0f) _atkTimer -= Time.deltaTime;
        if (_player == null) return;

        // só caça se o player estiver dentro do leash (não sai da cripta)
        Vector3 pp = _player.position;
        bool playerInside = leash.width <= 0f || leash.Contains(new Vector2(pp.x, pp.z));
        bool walking = false;

        if (playerInside && _playerHealth != null && !_playerHealth.IsDead)
        {
            Vector3 to = (pp + _jitter) - transform.position;
            to.y = 0f;
            if (to.magnitude > 0.85f)
            {
                transform.position += to.normalized * speed * Time.deltaTime;
                walking = true;
                if (_sr != null && Mathf.Abs(to.x) > 0.05f)
                    _sr.flipX = to.x < 0f;
            }

            // dano por toque (distância real, sem jitter)
            Vector3 real = pp - transform.position;
            real.y = 0f;
            if (real.magnitude <= 1.0f && _atkTimer <= 0f)
            {
                _atkTimer = attackCooldown;
                _playerHealth.TakeDamage(touchDamage);
                DamageNumber.Spawn(pp + Vector3.up * 1.9f, "-" + touchDamage, new Color(1f, 0.35f, 0.32f));
            }
        }

        // animação (col 0 parado, 1-4 ciclo de andar)
        if (_frames != null && _sr != null)
        {
            if (walking) _animT += Time.deltaTime;
            else _animT = 0f;
            int col = walking ? 1 + (int)(_animT * 8f) % 4 : 0;
            _sr.sprite = _frames[0, col];
        }
    }

    void Die()
    {
        Fx.Sphere(transform.position + Vector3.up * 0.6f, 0.5f, new Color(0.5f, 0.1f, 0.15f, 0.6f), 0.35f, 2.2f);
        OnDeath?.Invoke(this);
        Destroy(gameObject);
    }
}
