using UnityEngine;

/// Faz o objeto encarar a câmera (sprite 2.5D), como o billboard do Godot.
/// Com a câmera isométrica fixa, o sprite aparece "de frente" no ângulo certo.
public class Billboard : MonoBehaviour
{
    void LateUpdate()
    {
        var cam = Camera.main;
        if (cam != null)
            transform.rotation = cam.transform.rotation;
    }
}
