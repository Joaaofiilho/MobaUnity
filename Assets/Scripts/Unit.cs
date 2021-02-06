using System;
using UnityEngine;
using Utils;

[RequireComponent(typeof(Unit))]
public abstract class Unit : MonoBehaviour
{
    [SerializeField] private float maxHealth;
    [SerializeField] private float currentHealth;
    protected bool ShouldLoseTargetWhenOutsideRange;

    //Stats section
    protected readonly Statistics Statistics = new Statistics();

    //Attack section
    protected Unit attackTarget { get; private set; }

    [HideInInspector] public bool isOnAttackRange;

    private float _attackSpeedCounter;

    [SerializeField] protected float attackChannelingTime = 0.2f;

    private float _attackChannelingCounter;

    [SerializeField] private float basicAttackAnimationSpeed = 15f;

    [SerializeField] private GameObject basicAttackPrefab;

    //Debug section
    [SerializeField] private bool showAttackRange;

    //Unity methods
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

                        OnAttack();
                        basicAttack.Fire(this, attackTarget, basicAttackAnimationSpeed, GetBasicAttackDamage());
                    }
                }
            }
            else
            {
                if (ShouldLoseTargetWhenOutsideRange)
                {
                    SetAttackTarget(null);
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
            Gizmos.DrawWireSphere(transform.position, Statistics.BasicAttackRange);
        }
    }

    //Actions, abstract and overridable methods

    //Callbacks

    public event Action<Unit, Unit> OnDieCallback = delegate(Unit actor, Unit target) { };

    protected virtual void OnDie(Unit actor, Unit target)
    {
        if (actor && actor.IsAttackTarget(target))
        {
            actor.SetAttackTarget(null);
        }

        OnDieCallback(actor, target);
    }

    public event Action<Unit, Unit, AttackInformation[]> OnTakeDamageCallback =
        delegate(Unit actor, Unit target, AttackInformation[] attackInformation) { };

    /// <summary>
    /// Called when this unit takes any damage that affects its health.
    /// This is only called if the hit or spell removed health of this unit.
    /// </summary>
    /// <param name="actor"></param>
    /// <param name="target"></param>
    /// <param name="attackInformations">The attack information. The numbers are after the armor and magic resistance calculations.</param>
    protected virtual void OnTakeDamage(Unit actor, Unit target, AttackInformation[] attackInformations)
    {
        DamagePopup.Create(transform, attackInformations);
            
        OnTakeDamageCallback(actor, target, attackInformations);
    }

    public event Action<float, float> OnHealthChangedCallback = delegate(float currentHealth, float maxHealth) { };

    protected virtual void OnHealthChanged(float currentHealth, float maxHealth)
    {
        OnHealthChangedCallback(currentHealth, maxHealth);
    }

    public event Action<Unit, Unit, AttackInformation[]> OnHitCallback =
        delegate(Unit actor, Unit target, AttackInformation[] attackInformations) { };

    /// <summary>
    /// Called everytime a basic attack hits this unit.
    /// </summary>
    /// <param name="actor">Who performed the basic attack.</param>
    /// <param name="target">The target who got attacked by the basic attack.</param>
    /// <param name="attackInformations">The attack information. The numbers are after the armor and magic resistance calculations.
    /// If the size is 0, then no damage has been dealt.</param>
    protected virtual void OnHit(Unit actor, Unit target, AttackInformation[] attackInformations)
    {
        OnHitCallback(actor, target, attackInformations);
    }

    public event Action<float, float> OnHealCallback = delegate(float amount, float totalAmount) { };

    protected virtual void OnHeal(float amount, float totalAmount)
    {
        OnHealCallback(amount, totalAmount);
    }

    public event Action<GameObject> OnSelectCallback = delegate(GameObject actor) { };

    /// <summary>
    /// Method called when a selection occurs on this target (Default = [Mouse] Left click).
    /// </summary>
    /// <param name="actor">Who did the selection.</param>
    public virtual void Select(GameObject actor)
    {
        OnSelectCallback(actor);
    }

    public event Action<Unit> OnKillUnitCallback = delegate(Unit unit) { };

    protected virtual void OnKillUnit(Unit target)
    {
        OnKillUnitCallback(target);
    }

    public event Action OnAttackCallback = delegate { };

    protected virtual void OnAttack()
    {
        OnAttackCallback();
    }

    public event Action<Unit> OnAttackTargetChangedCallback = delegate { };

    protected virtual void OnAttackTargetChanged(Unit target)
    {
        OnAttackTargetChangedCallback(target);
    }

    protected virtual AttackInformation[] GetBasicAttackDamage()
    {
        return new[] {new AttackInformation(Statistics.AttackDamage, DamageType.AttackDamage)};
    }

    //Class methods

    /// <summary>
    /// Tries to perform an attack on the desired target.
    /// </summary>
    /// <param name="target">The target who is going to be attacked.</param>
    public void SetAttackTarget(Unit target)
    {
        attackTarget = target;
        OnAttackTargetChanged(attackTarget);
    }

    public void TakeDamage(Unit actor, AttackInformation[] attackInformations)
    {
        //The real damage received, after applying armor and magic resistance calculation.
        var receivedDamageInformation = new AttackInformation[attackInformations.Length];

        for (int i = 0; i < attackInformations.Length; i++)
        {
            //TODO: calculate damage based on damageType, applying armor and magic resistance calculations. Remember: an attack damage can be already 0.
            currentHealth -= attackInformations[i].DamageAmount;

            //TODO: For now, the damage dealt will be 100%.
            receivedDamageInformation[i] = attackInformations[i];
        }

        if (receivedDamageInformation.Length > 0)
        {
            OnTakeDamage(actor, this, receivedDamageInformation);
        }

        actor.OnHit(actor, this, receivedDamageInformation);

        OnHealthChanged(currentHealth, maxHealth);

        if (currentHealth <= 0)
        {
            OnDie(actor, this);
            actor.OnKillUnit(this);
        }
    }

    public void Heal(Unit actor, float amount)
    {
        var previousHealth = currentHealth;

        if (currentHealth + amount >= maxHealth)
        {
            currentHealth = maxHealth;
            OnHealthChangedCallback(currentHealth, maxHealth);
            OnHealCallback(maxHealth - previousHealth, amount);
        }
        else
        {
            currentHealth += amount;
            OnHealthChangedCallback(currentHealth, maxHealth);
            OnHealCallback(amount, amount);
        }
    }

    /// <summary>
    /// Method called when an actions occurs on this target (Default = [Mouse] Right click).
    /// </summary>
    /// <param name="actor">Who did the action.</param>
    public virtual void OnReceiveAction(GameObject actor)
    {
        HighLight();
    }

    private void HighLight()
    {
        //TODO: Needs implementation
    }

    private bool IsAttackTarget(Unit unit)
    {
        return attackTarget && attackTarget.gameObject.GetInstanceID() == unit.gameObject.GetInstanceID();
    }
}