using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Unit : MonoBehaviour, Interactable
{
    [SerializeField]
    float maxHealth;
    [SerializeField]
    float health;

    public delegate void TakeDamageCallback(float amount);
    public TakeDamageCallback OnTakeDamage;

    /// <summary>
    /// Callback to delegate the amount healed from a unit.
    /// </summary>
    /// <param name="totalAmount">The total amount of the heal, independent of the current health.</param>
    /// <param name="amount">The amount of the heal, dependent on the current health.</param>
    public delegate void HealCallback(float totalAmount, float amount);
    public HealCallback OnHeal;

    public delegate void DieCallback(GameObject actor);
    public DieCallback OnDie;

    public virtual void OnAction(GameObject gameObject)
    {
        
    }

    public virtual void OnSelect(GameObject gameObject)
    {
        
    }

    public virtual void TakeDamage(GameObject actor, float amount)
    {
        health -= amount;
        OnTakeDamage(amount);

        if(health <= 0)
        {
            OnDie(actor);
        }
    }

    public virtual void Heal(GameObject actor, float amount)
    {
        float previousHealth = health;

        if (health + amount >= maxHealth)
        {
            health = maxHealth;
            OnHeal(maxHealth - previousHealth, amount);
        } else
        {
            health += amount;
            OnHeal(amount, amount);
        }
    }
}
