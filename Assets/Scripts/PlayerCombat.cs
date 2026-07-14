using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// Combate do player (porte de player.gd): auto-attack no botão esquerdo +
/// 4 skills com cooldown e mana. Q Raio · W Bolha · E Pilar · R Superataque.
/// No esquema WASD as skills viram Q/E/C/R (como no LOD); 1-4 sempre funciona.
[RequireComponent(typeof(PlayerController))]
public class PlayerCombat : MonoBehaviour
{
    public const int MaxMana = 30;
    public const float ManaRegen = 4f;
    public float Mana { get; private set; } = MaxMana;
    public float ManaFraction => Mana / MaxMana;

    // slots: 0=raio · 1=bolha · 2=pilar · 3=superataque (valores do LOD)
    static readonly float[] CooldownTime = { 0.6f, 5f, 3.5f, 15f };
    static readonly int[] ManaCost = { 8, 14, 10, 28 };

    const float AttackCooldown = 0.5f;   // auto-attack do mago
    const int AttackDamage = 4;

    const float LightningRange = 9.4f;
    const int LightningDamage = 8;
    const float BubbleRadius = 2.5f;
    const float BubbleStun = 3f;
    const float SuperRadius = 6f;
    const int SuperDamage = 50;
    const float SuperStun = 1.5f;

    readonly float[] _cd = new float[4];
    float _attackTimer;
    CameraFollow _cam;
    PlayerController _pc;

    void Start()
    {
        _pc = GetComponent<PlayerController>();
        _cam = Camera.main != null ? Camera.main.GetComponent<CameraFollow>() : null;
    }

    public float CooldownFraction(int slot) => Mathf.Clamp01(_cd[slot] / CooldownTime[slot]);

    void Update()
    {
        for (int i = 0; i < 4; i++)
            if (_cd[i] > 0f) _cd[i] -= Time.deltaTime;
        if (_attackTimer > 0f) _attackTimer -= Time.deltaTime;
        Mana = Mathf.Min(Mana + ManaRegen * Time.deltaTime, MaxMana);

        if (Time.timeScale == 0f) return;   // pausado

        if (Input.GetKeyDown(KeyCode.F1))
            GameState.ControlScheme = GameState.ControlScheme == "mouse" ? "wasd" : "mouse";

        for (int slot = 0; slot < 4; slot++)
            if (SkillPressed(slot))
                TryCast(slot);

        // auto-attack: segurar botão esquerdo (ignora cliques na UI)
        bool overUI = EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
        if (Input.GetMouseButton(0) && !overUI && _attackTimer <= 0f && CursorPoint(out Vector3 aim))
        {
            _attackTimer = AttackCooldown;
            FireBolt(aim);
        }
    }

    bool SkillPressed(int slot)
    {
        if (Input.GetKeyDown((KeyCode)((int)KeyCode.Alpha1 + slot)))
            return true;
        bool wasd = GameState.ControlScheme == "wasd";
        switch (slot)
        {
            case 0: return Input.GetKeyDown(KeyCode.Q);
            case 1: return Input.GetKeyDown(wasd ? KeyCode.E : KeyCode.W);
            case 2: return Input.GetKeyDown(wasd ? KeyCode.C : KeyCode.E);
            case 3: return Input.GetKeyDown(KeyCode.R);
        }
        return false;
    }

    bool CursorPoint(out Vector3 p)
    {
        p = Vector3.zero;
        return _cam != null && _cam.MouseGroundPosition(out p);
    }

    void TryCast(int slot)
    {
        if (_cd[slot] > 0f || Mana < ManaCost[slot]) return;
        if (!CursorPoint(out Vector3 aim)) return;
        _cd[slot] = CooldownTime[slot];
        Mana -= ManaCost[slot];
        switch (slot)
        {
            case 0: CastLightning(aim); break;
            case 1: CastBubble(aim); break;
            case 2: CastPillar(aim); break;
            case 3: CastSuper(aim); break;
        }
    }

    void FireBolt(Vector3 target)
    {
        Vector3 dir = target - transform.position;
        dir.y = 0f;
        if (dir.sqrMagnitude < 0.0001f) dir = Vector3.forward;
        dir.Normalize();

        var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        Object.Destroy(go.GetComponent<Collider>());
        go.name = "MagicBolt";
        go.transform.position = transform.position + Vector3.up * 0.75f + dir * 0.4f;
        go.transform.localScale = Vector3.one * 0.35f;
        go.GetComponent<Renderer>().material = Fx.Mat(new Color(0.75f, 0.45f, 1f, 0.95f));
        var p = go.AddComponent<Projectile>();
        p.dir = dir;
        p.damage = AttackDamage;
    }

    /// Q — Raio: linha instantânea na direção do cursor, dano em quem cruza.
    void CastLightning(Vector3 aim)
    {
        Vector3 dir = aim - transform.position;
        dir.y = 0f;
        if (dir.sqrMagnitude < 0.0001f) dir = Vector3.forward;
        dir.Normalize();
        Vector3 end = transform.position + dir * LightningRange;

        Vector3 up = Vector3.up * 0.75f;
        Fx.Line(transform.position + up, end + up, new Color(0.55f, 0.8f, 1f), 0.16f, 0.18f);
        foreach (var e in Object.FindObjectsByType<Enemy>(FindObjectsSortMode.None))
            if (DistToSegmentXZ(e.transform.position, transform.position, end) <= 0.9f)
                e.Hit(LightningDamage);
    }

    /// W — Bolha: atordoa inimigos em área no cursor.
    void CastBubble(Vector3 aim)
    {
        Fx.Sphere(aim + Vector3.up * 1.0f, BubbleRadius, new Color(0.4f, 0.7f, 1f, 0.35f), 0.7f, 1.05f);
        foreach (var e in EnemiesWithin(aim, BubbleRadius))
            e.Stun(BubbleStun);
    }

    /// E — Pilar de Fogo: zona de dano contínuo no cursor.
    void CastPillar(Vector3 aim)
    {
        var zone = new GameObject("FirePillar").AddComponent<PillarZone>();
        zone.transform.position = aim;
    }

    /// R — Superataque: teleporta pro cursor + explosão com stun.
    void CastSuper(Vector3 aim)
    {
        transform.position = aim;
        _pc.CancelMove();
        if (_cam != null) _cam.Snap();
        Fx.Sphere(aim + Vector3.up * 0.8f, 1.2f, new Color(0.9f, 0.6f, 1f, 0.5f), 0.45f, SuperRadius / 1.2f);
        foreach (var e in EnemiesWithin(aim, SuperRadius))
        {
            e.Hit(SuperDamage);
            e.Stun(SuperStun);
        }
    }

    static IEnumerable<Enemy> EnemiesWithin(Vector3 center, float radius)
    {
        foreach (var e in Object.FindObjectsByType<Enemy>(FindObjectsSortMode.None))
        {
            Vector3 d = e.transform.position - center;
            d.y = 0f;
            if (d.magnitude <= radius)
                yield return e;
        }
    }

    static float DistToSegmentXZ(Vector3 p, Vector3 a, Vector3 b)
    {
        Vector2 P = new Vector2(p.x, p.z);
        Vector2 A = new Vector2(a.x, a.z);
        Vector2 B = new Vector2(b.x, b.z);
        Vector2 AB = B - A;
        float t = Mathf.Clamp01(Vector2.Dot(P - A, AB) / AB.sqrMagnitude);
        return Vector2.Distance(P, A + AB * t);
    }
}

/// Zona do Pilar de Fogo: tiques de dano durante a duração (valores do LOD).
public class PillarZone : MonoBehaviour
{
    const float Radius = 2.5f;
    const float Duration = 2.5f;
    const float TickRate = 0.2f;
    const int TickDamage = 2;

    float _life = Duration;
    float _tick;

    void Start()
    {
        Fx.Cylinder(transform.position, Radius, 3f, new Color(1f, 0.45f, 0.15f, 0.3f), Duration);
    }

    void Update()
    {
        _life -= Time.deltaTime;
        _tick -= Time.deltaTime;
        if (_tick <= 0f)
        {
            _tick = TickRate;
            foreach (var e in Object.FindObjectsByType<Enemy>(FindObjectsSortMode.None))
            {
                Vector3 d = e.transform.position - transform.position;
                d.y = 0f;
                if (d.magnitude <= Radius)
                    e.Hit(TickDamage);
            }
        }
        if (_life <= 0f)
            Destroy(gameObject);
    }
}
