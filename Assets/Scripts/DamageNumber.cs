using UnityEngine;

/// Número de dano flutuante (porte de damage_number.tscn): sobe e desvanece.
public class DamageNumber : MonoBehaviour
{
    const float Life = 0.9f;
    const float RiseSpeed = 1.6f;

    float _t = Life;
    TextMesh _tm;
    Color _color;

    public static void Spawn(Vector3 pos, string text, Color color)
    {
        var go = new GameObject("DamageNumber");
        go.transform.position = pos + new Vector3(Random.Range(-0.4f, 0.4f), 0f, 0f);

        var tm = go.AddComponent<TextMesh>();
        tm.text = text;
        tm.characterSize = 0.1f;
        tm.fontSize = 42;
        tm.anchor = TextAnchor.MiddleCenter;
        tm.color = color;
        tm.font = HUD.UiFont;
        go.GetComponent<MeshRenderer>().sharedMaterial = tm.font.material;

        var dn = go.AddComponent<DamageNumber>();
        dn._tm = tm;
        dn._color = color;
        go.AddComponent<Billboard>();
    }

    void Update()
    {
        _t -= Time.deltaTime;
        transform.position += Vector3.up * RiseSpeed * Time.deltaTime;
        if (_tm != null)
        {
            var c = _color;
            c.a = Mathf.Clamp01(_t / 0.4f);   // desvanece no fim
            _tm.color = c;
        }
        if (_t <= 0f)
            Destroy(gameObject);
    }
}
