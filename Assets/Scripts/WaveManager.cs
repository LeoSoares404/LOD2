using System.Collections.Generic;
using UnityEngine;

/// Ondas da cripta (porte do wave_manager.gd): 2 comuns → boss → boss veloz.
/// Começa quando o player entra na cripta; só avança ao limpar a onda.
public class WaveManager : MonoBehaviour
{
    public static WaveManager Instance;
    public const int TotalWaves = 4;

    public Rect bounds;   // área XZ da cripta

    Sprite[,] _ghoulFrames;
    readonly List<Enemy> _alive = new List<Enemy>();
    int _wave;
    bool _running;
    bool _victory;

    void Awake()
    {
        Instance = this;
        _ghoulFrames = SpriteUtil.Slice("ghoul_walk", 5, 1, 16f);
    }

    /// Chamado quando o player entra na cripta (a 1ª vez inicia as ondas).
    public void EnterCrypt()
    {
        if (_running || _victory) return;
        _running = true;
        Invoke(nameof(NextWave), 1.0f);
    }

    void NextWave()
    {
        _wave++;
        GameState.CurrentWave = _wave;
        if (HUD.Instance != null)
        {
            HUD.Instance.SetWaveCounter($"{_wave}/{TotalWaves} ondas");
            bool boss = _wave >= 3;
            HUD.Instance.Banner(boss ? "!! CHEFE !!" : $"RODADA {_wave}",
                boss ? new Color(1f, 0.4f, 0.3f) : new Color(0.6f, 1f, 0.95f));
        }

        switch (_wave)
        {
            case 1:
                SpawnMany(4, 12, 2.2f, 3, Color.white, 1f);
                break;
            case 2:
                SpawnMany(5, 12, 2.2f, 3, Color.white, 1f);
                SpawnMany(2, 6, 4.4f, 2, new Color(1f, 0.55f, 0.5f), 0.9f);   // sprinters
                break;
            case 3:
                SpawnOne(60, 1.6f, 8, new Color(0.75f, 1f, 0.75f), 2.1f);     // boss
                break;
            case 4:
                SpawnOne(45, 3.4f, 6, new Color(1f, 0.5f, 0.9f), 1.7f);       // boss veloz
                break;
        }
    }

    void SpawnMany(int count, int hp, float speed, int dmg, Color tint, float scale)
    {
        for (int i = 0; i < count; i++)
            SpawnOne(hp, speed, dmg, tint, scale);
    }

    void SpawnOne(int hp, float speed, int dmg, Color tint, float scale)
    {
        var go = new GameObject("Enemy");
        go.transform.position = RandomPoint();
        var health = go.AddComponent<Health>();
        health.SetMax(hp);
        var e = go.AddComponent<Enemy>();
        e.speed = speed;
        e.touchDamage = dmg;
        e.leash = bounds;
        e.Setup(_ghoulFrames, tint, scale);
        e.OnDeath = OnEnemyDeath;
        _alive.Add(e);
    }

    Vector3 RandomPoint()
    {
        var pc = UnityEngine.Object.FindFirstObjectByType<PlayerController>();
        for (int i = 0; i < 24; i++)
        {
            var p = new Vector3(
                Random.Range(bounds.xMin + 1.5f, bounds.xMax - 1.5f), 0f,
                Random.Range(bounds.yMin + 1.5f, bounds.yMax - 1.5f));
            if (pc == null) return p;
            Vector3 d = p - pc.transform.position;
            d.y = 0f;
            if (d.magnitude >= 6f) return p;   // nasce longe do player
        }
        return new Vector3(bounds.center.x, 0f, bounds.yMax - 2f);
    }

    void OnEnemyDeath(Enemy e)
    {
        _alive.Remove(e);
        if (!_running || _alive.Count > 0) return;

        if (_wave >= TotalWaves)
        {
            _victory = true;
            _running = false;
            if (HUD.Instance != null)
            {
                HUD.Instance.SetWaveCounter("");
                HUD.Instance.Banner("VITÓRIA!", new Color(1f, 0.9f, 0.4f), 4f);
            }
        }
        else
        {
            Invoke(nameof(NextWave), 1.6f);
        }
    }
}
