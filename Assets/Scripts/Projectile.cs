using UnityEngine;

/// Projétil simples (orbe do auto-attack): voa reto e acerta o 1º inimigo perto.
public class Projectile : MonoBehaviour
{
    public Vector3 dir = Vector3.forward;
    public float speed = 13f;
    public int damage = 4;
    public float life = 2f;
    public float hitRadius = 0.55f;

    void Update()
    {
        transform.position += dir * speed * Time.deltaTime;
        life -= Time.deltaTime;
        if (life <= 0f)
        {
            Destroy(gameObject);
            return;
        }

        foreach (var e in Object.FindObjectsByType<Enemy>(FindObjectsSortMode.None))
        {
            Vector3 d = e.transform.position - transform.position;
            d.y = 0f;
            if (d.magnitude <= hitRadius + 0.45f)
            {
                e.Hit(damage);
                Fx.Sphere(transform.position, 0.4f, new Color(0.75f, 0.45f, 1f, 0.7f), 0.25f, 2f);
                Destroy(gameObject);
                return;
            }
        }
    }
}
