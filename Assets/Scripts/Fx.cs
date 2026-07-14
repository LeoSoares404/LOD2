using UnityEngine;

/// Efeitos visuais rápidos por código (malhas/linhas com shader de sprite,
/// que aceita cor com transparência e funciona no URP).
public static class Fx
{
    public static Material Mat(Color c)
    {
        var m = new Material(Shader.Find("Sprites/Default"));
        m.color = c;
        return m;
    }

    /// Esfera que cresce até `growTo`× e desvanece em `life` segundos.
    public static GameObject Sphere(Vector3 pos, float radius, Color c, float life = 0.5f, float growTo = 1f)
    {
        var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        Object.Destroy(go.GetComponent<Collider>());
        go.name = "FxSphere";
        go.transform.position = pos;
        go.transform.localScale = Vector3.one * radius * 2f;
        go.GetComponent<Renderer>().material = Mat(c);
        var t = go.AddComponent<FxTimed>();
        t.life = life;
        t.growTo = growTo;
        t.baseColor = c;
        return go;
    }

    /// Cilindro (pilar) que desvanece em `life` segundos.
    public static GameObject Cylinder(Vector3 pos, float radius, float height, Color c, float life)
    {
        var go = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        Object.Destroy(go.GetComponent<Collider>());
        go.name = "FxCylinder";
        go.transform.position = pos + Vector3.up * (height * 0.5f);
        go.transform.localScale = new Vector3(radius * 2f, height * 0.5f, radius * 2f);
        go.GetComponent<Renderer>().material = Mat(c);
        var t = go.AddComponent<FxTimed>();
        t.life = life;
        t.baseColor = c;
        return go;
    }

    /// Linha (raio) que desvanece em `life` segundos.
    public static void Line(Vector3 a, Vector3 b, Color c, float width = 0.14f, float life = 0.16f)
    {
        var go = new GameObject("FxLine");
        var lr = go.AddComponent<LineRenderer>();
        lr.positionCount = 2;
        lr.SetPosition(0, a);
        lr.SetPosition(1, b);
        lr.startWidth = width;
        lr.endWidth = width * 0.5f;
        lr.material = Mat(Color.white);
        lr.startColor = c;
        lr.endColor = c;
        var t = go.AddComponent<FxTimed>();
        t.life = life;
        t.baseColor = c;
    }
}

/// Cresce até growTo× e desvanece durante `life`, depois se destrói.
public class FxTimed : MonoBehaviour
{
    public float life = 0.4f;
    public float growTo = 1f;
    public Color baseColor = Color.white;

    float _t;
    Vector3 _scale0;
    LineRenderer _lr;
    Renderer _rend;

    void Start()
    {
        _scale0 = transform.localScale;
        _lr = GetComponent<LineRenderer>();
        _rend = GetComponent<Renderer>();
    }

    void Update()
    {
        _t += Time.deltaTime;
        float k = Mathf.Clamp01(_t / life);
        if (growTo != 1f)
            transform.localScale = _scale0 * Mathf.Lerp(1f, growTo, k);

        var c = baseColor;
        c.a = baseColor.a * (1f - k);
        if (_lr != null)
        {
            _lr.startColor = c;
            _lr.endColor = c;
        }
        else if (_rend != null)
        {
            _rend.material.color = c;
        }

        if (_t >= life)
            Destroy(gameObject);
    }
}
