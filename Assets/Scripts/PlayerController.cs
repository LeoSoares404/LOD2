using UnityEngine;

/// Player 2.5D no plano XZ. Movimento WASD + click-to-move (segurar botão
/// direito, estilo Diablo). Porte simplificado de player.gd (LOD/Godot).
/// 16 px = 1 m no LOD -> aqui usamos metros/unidades Unity direto.
public class PlayerController : MonoBehaviour
{
    public float speed = 5.6f;            // m/s (LOD: SPEED)
    public float arriveDistance = 0.25f;  // LOD: ARRIVE_DISTANCE

    Vector3 _target;
    bool _moving;
    CameraFollow _cam;

    void Start()
    {
        _cam = Camera.main != null ? Camera.main.GetComponent<CameraFollow>() : null;
        _target = transform.position;
    }

    void Update()
    {
        Vector3 wasd = WasdDir();
        if (wasd.sqrMagnitude > 0.001f)
        {
            _moving = false;  // WASD tem prioridade e cancela o click-to-move
            transform.position += wasd.normalized * speed * Time.deltaTime;
            return;
        }
        MoveClick();
    }

    Vector3 WasdDir()
    {
        Vector3 d = Vector3.zero;
        if (Input.GetKey(KeyCode.W)) d.z += 1;  // pra "cima" na tela (longe da câmera)
        if (Input.GetKey(KeyCode.S)) d.z -= 1;
        if (Input.GetKey(KeyCode.D)) d.x += 1;
        if (Input.GetKey(KeyCode.A)) d.x -= 1;
        return d;
    }

    void MoveClick()
    {
        // segurar o botão direito segue o cursor (LOD: move_click = botão direito)
        if (Input.GetMouseButton(1) && _cam != null && _cam.MouseGroundPosition(out Vector3 p))
        {
            _target = p;
            _moving = true;
        }
        if (!_moving) return;

        Vector3 to = _target - transform.position;
        to.y = 0f;
        if (to.magnitude <= arriveDistance)
        {
            _moving = false;
            return;
        }
        transform.position += to.normalized * speed * Time.deltaTime;
    }
}
