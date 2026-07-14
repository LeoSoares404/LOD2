using UnityEngine;

/// Câmera 2.5D isométrica fixa (estilo Diablo): segue o alvo com suavização,
/// ângulo e distância fixos. Porte de camera_rig.gd do LOD (Godot).
[RequireComponent(typeof(Camera))]
public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float pitch = 55f;      // graus olhando pra baixo (LOD: Iso.CAM_PITCH = -55)
    public float distance = 25f;   // recuo da câmera
    public float smooth = 8f;      // LOD: SMOOTH = 8

    Camera _cam;

    void Awake() => _cam = GetComponent<Camera>();

    void Start()
    {
        transform.rotation = Quaternion.Euler(pitch, 0f, 0f);
        if (target != null) transform.position = DesiredPosition();
    }

    void LateUpdate()
    {
        if (target == null) return;
        float k = 1f - Mathf.Exp(-smooth * Time.deltaTime);  // mesma suavização do LOD
        transform.position = Vector3.Lerp(transform.position, DesiredPosition(), k);
        transform.rotation = Quaternion.Euler(pitch, 0f, 0f);
    }

    /// Corta a suavização e cola no alvo (teleportes: portas, superataque).
    public void Snap()
    {
        if (target != null)
            transform.position = DesiredPosition();
    }

    Vector3 DesiredPosition()
    {
        float r = pitch * Mathf.Deg2Rad;
        // acima (+y) e atrás (-z) do alvo, olhando pra baixo no ângulo `pitch`
        Vector3 offset = new Vector3(0f, distance * Mathf.Sin(r), -distance * Mathf.Cos(r));
        return target.position + offset;
    }

    /// Ponto do chão (plano y=0) sob o cursor — usado pelo click-to-move.
    public bool MouseGroundPosition(out Vector3 point)
    {
        point = Vector3.zero;
        Ray ray = _cam.ScreenPointToRay(Input.mousePosition);
        Plane ground = new Plane(Vector3.up, Vector3.zero);
        if (ground.Raycast(ray, out float enter))
        {
            point = ray.GetPoint(enter);
            return true;
        }
        return false;
    }
}
