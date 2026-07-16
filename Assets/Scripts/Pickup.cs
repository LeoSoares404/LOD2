using UnityEngine;

/// Drop no chão (ouro/poção): flutua, é puxado pro player quando ele chega
/// perto (estilo Diablo) e coleta ao encostar.
public class Pickup : MonoBehaviour
{
    public enum Kind { Gold, Potion }

    public Kind kind = Kind.Gold;
    public int amount = 3;

    const float MagnetRange = 2.4f;
    const float MagnetSpeed = 8f;
    const float CollectRange = 0.55f;

    Transform _player;
    Health _playerHealth;
    float _bobT;
    float _baseY;

    public static void SpawnGold(Vector3 pos, int amount)
    {
        Spawn(pos, Kind.Gold, amount, new Color(1f, 0.85f, 0.25f), 9);
    }

    public static void SpawnPotion(Vector3 pos, int heal)
    {
        Spawn(pos, Kind.Potion, heal, new Color(0.95f, 0.25f, 0.3f), 12);
    }

    static void Spawn(Vector3 pos, Kind kind, int amount, Color color, int size)
    {
        var go = new GameObject("Pickup" + kind);
        // espalha um pouco ao redor do ponto da morte
        pos += new Vector3(Random.Range(-0.6f, 0.6f), 0f, Random.Range(-0.6f, 0.6f));
        go.transform.position = pos + Vector3.up * 0.3f;
        var sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = SpriteUtil.Circle(color, size);
        go.AddComponent<Billboard>();
        var p = go.AddComponent<Pickup>();
        p.kind = kind;
        p.amount = amount;
    }

    void Start()
    {
        var pc = Object.FindFirstObjectByType<PlayerController>();
        if (pc != null)
        {
            _player = pc.transform;
            _playerHealth = pc.GetComponent<Health>();
        }
        _baseY = transform.position.y;
        _bobT = Random.value * 10f;
    }

    void Update()
    {
        _bobT += Time.deltaTime;
        var p = transform.position;
        p.y = _baseY + Mathf.Abs(Mathf.Sin(_bobT * 3f)) * 0.12f;
        transform.position = p;

        if (_player == null) return;

        Vector3 to = _player.position - transform.position;
        to.y = 0f;
        float dist = to.magnitude;

        if (dist <= MagnetRange && dist > 0.01f)
            transform.position += to.normalized * MagnetSpeed * Time.deltaTime;

        if (dist <= CollectRange)
            Collect();
    }

    void Collect()
    {
        switch (kind)
        {
            case Kind.Gold:
                GameState.AddGold(amount);
                DamageNumber.Spawn(transform.position + Vector3.up * 0.8f,
                    $"+{amount} ouro", new Color(1f, 0.85f, 0.25f));
                break;
            case Kind.Potion:
                if (_playerHealth != null)
                    _playerHealth.Heal(amount);
                DamageNumber.Spawn(transform.position + Vector3.up * 0.8f,
                    $"+{amount} vida", new Color(0.4f, 1f, 0.45f));
                break;
        }
        Destroy(gameObject);
    }
}
