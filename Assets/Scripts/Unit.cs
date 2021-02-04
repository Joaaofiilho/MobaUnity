using System;
using UnityEngine;
using Utils;

public abstract class Unit : MonoBehaviour
{
    [SerializeField] private float maxHealth;
    [SerializeField] private float currentHealth;

    //Stats section
    protected readonly Statistics Statistics = new Statistics();

    //Attack section
    [HideInInspector] public Unit attackTarget;

    [HideInInspector] public bool isOnAttackRange;

    private float _attackSpeedCounter;

    [SerializeField] protected float attackChannelingTime = 0.2f;

    private float _attackChannelingCounter;

    [SerializeField] private float basicAttackAnimationSpeed = 15f;

    [SerializeField] private GameObject basicAttackPrefab;

    //Debug section
    [SerializeField] private bool showAttackRange;
    
    //Callbacks
    public event Action<float> OnTakeDamage = delegate(float amount) {  };
    
    public event Action<float, float> OnHealthChanged = delegate(float currentHealth, float maxHealth) {  };
    
    public event Action<Unit, Unit> OnHit = delegate(Unit actor, Unit target) {  };

    /// <summary>
    /// Callback to delegate the amount healed from a unit.
    /// </summary>
    /// /// <param name="amount">The amount of the heal, dependent on the current health.</param>
    /// <param name="totalAmount">The total amount of the heal, independent of the current health.</param>
    public event Action<float, float> OnHeal = delegate(float amount, float totalAmount) {  };
    
    public event Action<Unit, Unit> OnDied = delegate(Unit actor, Unit target) { };

    protected virtual void Awake()
    {
        _attackSpeedCounter = 1f / Statistics.AttackSpeed;
    }

    protected virtual void Update()
    {
        if (_attackSpeedCounter < 1f / Statistics.AttackSpeed)
        {
            _attackSpeedCounter += Time.deltaTime;
        }

        if (attackTarget)
        {
            isOnAttackRange = Vector3.Distance(PhysicsUtils.Vector3Y0(transform.position),
                PhysicsUtils.Vector3Y0(attackTarget.transform.position)) <= Statistics.BasicAttackRange;

            if (isOnAttackRange)
            {
                if (_attackSpeedCounter >= 1f / Statistics.AttackSpeed)
                {
                    _attackChannelingCounter += Time.deltaTime;

                    if (_attackChannelingCounter >= attackChannelingTime)
                    {
                        _attackSpeedCounter = 0;

                        var prefab = Instantiate(basicAttackPrefab, transform.position, Quaternion.identity);
                        var basicAttack = prefab.GetComponent<BasicAttack>();
                        basicAttack.Follow(this, attackTarget, Statistics.AttackDamage, basicAttackAnimationSpeed, OnHit);
                    }
                }
            }
            else
            {
                attackTarget = null;
            }
        }
        else
        {
            isOnAttackRange = false;
            _attackChannelingCounter = 0;
        }
    }

    private void OnDestroy()
    {
        OnTakeDamage = null;
        OnDied = null;
        OnHeal = null;
        OnHit = null;
        OnHealthChanged = null;
    }

    protected virtual void OnDrawGizmos()
    {
        if (showAttackRange)
        {
            Gizmos.DrawWireSphere(transform.position, Statistics.BasicAttackRange);
        }
    }

    /// <summary>
    /// Tries to perform an attack on the desired target.
    /// </summary>
    /// <param name="target">The target who is going to be attacked.</param>
    public void SetAttackTarget(Unit target)
    {
        attackTarget = target;
        target.OnDied += OnAttackUnitDie;
    }

    //Class methods

    /// <summary>
    /// Method called when an actions occurs on this target (Default = [Mouse] Right click).
    /// </summary>
    /// <param name="actor">Who did the action.</param>
    public virtual void OnReceiveAction(GameObject actor)
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
    
    protected bool IsAttackTarget(Unit unit)
    {
        return attackTarget && attackTarget.gameObject.GetInstanceID() == unit.gameObject.GetInstanceID();
    }
    
    private void HighLight()
    {
        //TODO: Needs implementation
    }

    private void OnAttackUnitDie(Unit actor, Unit target)
    {
        if (IsAttackTarget(target))
        {
            attackTarget = null;
        }
    }
}