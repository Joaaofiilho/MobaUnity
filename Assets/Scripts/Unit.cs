﻿using System;
using UnityEngine;
using Utils;

public abstract class Unit : MonoBehaviour
{
    [SerializeField] private float maxHealth;
    [SerializeField] private float currentHealth;

    //Stats section
    protected Statistics Statistics;

    //Attack section
    [HideInInspector] public Unit attackTarget;

    [SerializeField] private float basicAttackRange = 6f;

    [HideInInspector] public bool isOnAttackRange;

    [SerializeField] private float attackSpeed = 1f;

    private float _attackSpeedCounter;

    [SerializeField] private float attackChannelingTime = 0.2f;

    private float _attackChannelingCounter;

    [SerializeField] private float basicAttackAnimationSpeed = 15f;

    [SerializeField] private GameObject basicAttackPrefab;

    //Debug section
    [SerializeField] private bool showAttackRange;
    
    //Callbacks
    
    //amount of damage
    public event Action<float> OnTakeDamage = delegate {  };
    
    //currentHealth and maxHealth
    public event Action<float, float> OnHealthChanged = delegate {  }; 

    /// <summary>
    /// Callback to delegate the amount healed from a unit.
    /// </summary>
    /// /// <param name="amount">The amount of the heal, dependent on the current health.</param>
    /// <param name="totalAmount">The total amount of the heal, independent of the current health.</param>
    public event Action<float, float> OnHeal = delegate(float amount, float totalAmount) {  };
    
    public event Action<Unit, Unit> OnDied = delegate(Unit actor, Unit target) { };

    protected virtual void Awake()
    {
        _attackSpeedCounter = 1f / attackSpeed;
        Statistics = new Statistics();
    }

    protected virtual void Update()
    {
        if (_attackSpeedCounter < 1f / attackSpeed)
        {
            _attackSpeedCounter += Time.deltaTime;
        }

        if (attackTarget)
        {
            isOnAttackRange = Vector3.Distance(PhysicsUtils.Vector3Y0(transform.position),
                PhysicsUtils.Vector3Y0(attackTarget.transform.position)) <= basicAttackRange;

            if (isOnAttackRange)
            {
                if (_attackSpeedCounter >= 1f / attackSpeed)
                {
                    _attackChannelingCounter += Time.deltaTime;

                    if (_attackChannelingCounter >= attackChannelingTime)
                    {
                        _attackSpeedCounter = 0;

                        var prefab = Instantiate(basicAttackPrefab, transform.position, Quaternion.identity);
                        var basicAttack = prefab.GetComponent<BasicAttack>();
                        basicAttack.Follow(this, attackTarget, Statistics.AttackDamage, basicAttackAnimationSpeed);
                    }
                }
            }
        }
        else
        {
            isOnAttackRange = false;
            _attackChannelingCounter = 0;
        }
    }

    protected virtual void OnDrawGizmos()
    {
        if (showAttackRange)
        {
            Gizmos.DrawWireSphere(transform.position, basicAttackRange);
        }
    }

    /// <summary>
    /// Tries to perform an attack on the desired target.
    /// </summary>
    /// <param name="target">The target who is going to be attacked.</param>
    public void SetAttackTarget(Unit target)
    {
        attackTarget = target;
    }

    //Class methods

    /// <summary>
    /// Method called when an actions occurs on this target (Default = [Mouse] Right click).
    /// </summary>
    /// <param name="actor">Who did the action.</param>
    public virtual void Action(GameObject actor)
    {
        HighLight();
    }


    /// <summary>
    /// Method called when a selection occurs on this target (Default = [Mouse] Left click).
    /// </summary>
    /// <param name="actor">Who did the selection.</param>
    public virtual void Select(GameObject actor)
    {
    }

    protected virtual void OnKillUnit(Unit target)
    {
        
    }

    protected abstract void OnDie(Unit actor);

    public void TakeDamage(Unit actor, float amount, DamageType damageType = DamageType.TrueDamage)
    {
        currentHealth -= amount;
        OnTakeDamage(amount);
        
        OnHealthChanged(currentHealth, maxHealth);
        
        if (currentHealth <= 0)
        {
            OnDie(actor);
            actor.OnKillUnit(this);
            OnDied(actor, this);
        }
    }

    public void Heal(Unit actor, float amount)
    {
        var previousHealth = currentHealth;
        
        if (currentHealth + amount >= maxHealth)
        {
            currentHealth = maxHealth;
            OnHealthChanged(currentHealth, maxHealth);
            OnHeal(maxHealth - previousHealth, amount);
        }
        else
        {
            currentHealth += amount;
            OnHealthChanged(currentHealth, maxHealth);
            OnHeal(amount, amount);
        }
    }

    private void HighLight()
    {
        //TODO: Needs implementation
    }
}