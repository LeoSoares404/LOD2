using UnityEngine;

/// Player 2.5D no plano XZ (porte de player.gd). Esquemas do LOD:
/// "mouse" (Clássico) = andar segurando o botão direito, estilo Diablo;
/// "wasd" (Moderno) = WASD move (e as skills remapeiam pra Q/E/C/R).
/// F1 troca o esquema (ver PlayerCombat).
public class PlayerController : MonoBehaviour
{
    public float speed = 5.6f;            // m/s (LOD: SPEED)
    public float arriveDistance = 0.25f;  // LOD: ARRIVE_DISTANCE

    public Vector3 Velocity { get; private set; }
    public bool IsMoving { get; private set; }

    Vector3 _target;
    bool _clickMoving;
    CameraFollow _cam;

    void Start()
    {
        _cam = Camera.main != null ? Camera.main.GetComponent<CameraFollow>() : null;
        _target = transform.position;
    }

    /// Cancela o click-to-move (usado em teleportes: portas, superataque, morte).
    public void CancelMove()
    {
        _clickMoving = false;
        _target = transform.position;
    }

    void Update()
    {
        Vector3 vel = WasdVelocity();
        if (vel == Vector3.zero)
            vel = ClickVelocity();

        transform.position += vel * Time.deltaTime;
        Velocity = vel;
        IsMoving = vel.sqrMagnitude > 0.01f;
    }

    Vector3 WasdVelocity()
    {
        if (GameState.ControlScheme != "wasd")
            return Vector3.zero;   // no Clássico, WASD são teclas de skill/nada

        Vector3 d = Vector3.zero;
        if (Input.GetKey(KeyCode.W)) d.z += 1;
        if (Input.GetKey(KeyCode.S)) d.z -= 1;
        if (Input.GetKey(KeyCode.D)) d.x += 1;
        if (Input.GetKey(KeyCode.A)) d.x -= 1;
        if (d != Vector3.zero)
            _clickMoving = false;
        return d.normalized * speed;
    }

    Vector3 ClickVelocity()
    {
        // segurar o botão direito segue o cursor (LOD: move_click)
        if (Input.GetMouseButton(1) && _cam != null && _cam.MouseGroundPosition(out Vector3 p))
        {
            _target = p;
            _clickMoving = true;
        }
        if (!_clickMoving)
            return Vector3.zero;

        Vector3 to = _target - transform.position;
        to.y = 0f;
        if (to.magnitude <= arriveDistance)
        {
            _clickMoving = false;
            return Vector3.zero;
        }
        return to.normalized * speed;
    }
}
