using System;
using UnityEngine;

/// Vida de player e inimigos (porte do health_component.gd).
public class Health : MonoBehaviour
{
    public int maxHealth = 20;
    public int Current { get; private set; }
    public bool IsDead => Current <= 0;

    public event Action<int, int> Changed;   // (atual, max)
    public event Action Died;

    void Awake()
    {
        if (Current <= 0)
            Current = maxHealth;
    }

    /// Define máximo e enche — usar logo após AddComponent (Awake roda antes).
    public void SetMax(int max)
    {
        maxHealth = max;
        Current = max;
        Changed?.Invoke(Current, maxHealth);
    }

    public void TakeDamage(int amount)
    {
        if (IsDead) return;
        Current = Mathf.Max(Current - amount, 0);
        Changed?.Invoke(Current, maxHealth);
        if (Current == 0)
            Died?.Invoke();
    }

    public void Heal(int amount)
    {
        if (IsDead) return;
        Current = Mathf.Min(Current + amount, maxHealth);
        Changed?.Invoke(Current, maxHealth);
    }

    public void HealFull()
    {
        Current = maxHealth;
        Changed?.Invoke(Current, maxHealth);
    }
}
