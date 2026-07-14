using UnityEngine;

/// Zona de passagem (porte do door.gd): encostar teleporta o player.
/// Trava global evita a porta-destino disparar na chegada.
public class DoorTrigger : MonoBehaviour
{
    public Vector3 destination;
    public float radius = 1.3f;
    public System.Action onUse;

    static float s_lock;

    Transform _player;
    PlayerController _pc;

    void Start()
    {
        _pc = Object.FindFirstObjectByType<PlayerController>();
        if (_pc != null)
            _player = _pc.transform;
    }

    void Update()
    {
        if (_player == null || Time.time < s_lock) return;

        Vector3 d = _player.position - transform.position;
        d.y = 0f;
        if (d.magnitude > radius) return;

        s_lock = Time.time + 1.2f;
        _player.position = destination;
        _pc.CancelMove();
        var cam = Camera.main != null ? Camera.main.GetComponent<CameraFollow>() : null;
        if (cam != null)
            cam.Snap();
        onUse?.Invoke();
    }
}
