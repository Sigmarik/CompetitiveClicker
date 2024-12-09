using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthTracker : MonoBehaviour
{
    void Start()
    {
        health = maxHealth;
    }
    public bool IsDead() { return health <= 0.0f; }
    public float Damage(float amount)
    {
        float oldHealth = health;
        health -= amount;
        health = Mathf.Clamp(health, 0, maxHealth);

        float delta = oldHealth - health;
        if (delta > 0.0f)
        {
            onDamaged?.Invoke(delta);
        }
        else if (delta < 0.0f)
        {
            onHealed?.Invoke(delta);
        }

        if (health == 0.0f && oldHealth > 0.0f)
        {
            onDeath?.Invoke();
        }

        return delta;
    }

    public float Heal(float amount, bool ignoreMortality = false)
    {
        if (IsDead() && !ignoreMortality)
        {
            return 0.0f;
        }

        return -Damage(-amount);
    }

    public float Kill()
    {
        return Damage(health);
    }

    public float Restore(bool ignoreMortality = false)
    {
        return Heal(maxHealth - health, ignoreMortality);
    }

    public delegate void OnDeath();
    public delegate void OnDamaged(float amount);
    public delegate void OnHealed(float amount);

    [HideInInspector]
    public float health = 0.0f;
    public float maxHealth = 100.0f;

    public OnDeath onDeath;
    public OnDamaged onDamaged;
    public OnHealed onHealed;
}
