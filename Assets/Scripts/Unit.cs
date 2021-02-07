using System;
using UnityEngine;
using Utils;

[RequireComponent(typeof(Unit))]
public abstract class Unit : Entity
{
    protected bool ShouldLoseTargetWhenOutsideRange;

    //Stats section
    [SerializeField] protected Statistics statistics = new Statistics();

    //Attack section
    protected Entity attackTarget { get; private set; }

    [HideInInspector] public bool isOnAttackRange;

    private float _attackSpeedCounter;

    [SerializeField] protected float attackChannelingTime = 0.2f;

    private float _attackChannelingCounter;

    [SerializeField] private float basicAttackAnimationSpeed = 15f;

    [SerializeField] private GameObject basicAttackPrefab;

    //Debug section
    [SerializeField] private bool showAttackRange;

    //Unity methods
    protected override void Awake()
    {
        base.Awake();
        _attackSpeedCounter = 1f / statistics.AttackSpeed;
    }

    protected virtual void Start()
    {
    }

    protected override void Update()
    {
        base.Update();
        if (_attackSpeedCounter < 1f / statistics.AttackSpeed)
        {
            _attackSpeedCounter += Time.deltaTime;
        }

        if (attackTarget)
        {
            isOnAttackRange = Vector3.Distance(PhysicsUtils.Vector3Y0(transform.position),
                PhysicsUtils.Vector3Y0(attackTarget.transform.position)) <= statistics.BasicAttackRange;

            if (isOnAttackRange)
            {
                if (_attackSpeedCounter >= 1f / statistics.AttackSpeed)
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
            Gizmos.DrawWireSphere(transform.position, statistics.BasicAttackRange);
        }
    }

    //Actions, abstract and overridable methods
    protected override void OnDie(Unit actor, Entity target)
    {
        base.OnDie(actor, target);
        
        if (actor && actor.IsAttackTarget(target))
        {
            actor.SetAttackTarget(null);
        }
    }

    protected override void OnTakeDamage(Unit actor, Entity target, AttackInformation[] originalAttackInformations,
        AttackInformation[] realAttackInformations)
    {
        base.OnTakeDamage(actor, target, originalAttackInformations, realAttackInformations);
        OnHit(actor, this, realAttackInformations);
    }

    //Callbacks
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

    public event Action<GameObject> OnSelectCallback = delegate(GameObject actor) { };

    /// <summary>
    /// Method called when a selection occurs on this target (Default = [Mouse] Left click).
    /// </summary>
    /// <param name="actor">Who did the selection.</param>
    public virtual void Select(GameObject actor)
    {
        OnSelectCallback(actor);
    }

    public event Action OnAttackCallback = delegate { };

    protected virtual void OnAttack()
    {
        OnAttackCallback();
    }

    public event Action<Entity> OnAttackTargetChangedCallback = delegate(Entity target) { };

    protected virtual void OnAttackTargetChanged(Entity target)
    {
        OnAttackTargetChangedCallback(target);
    }

    protected virtual AttackInformation[] GetBasicAttackDamage()
    {
        return new[] {new AttackInformation(statistics.AttackDamage, DamageType.AttackDamage)};
    }

    //Class methods

    /// <summary>
    /// Tries to perform an attack on the desired target.
    /// </summary>
    /// <param name="target">The target who is going to be attacked.</param>
    public void SetAttackTarget(Entity target)
    {
        attackTarget = target;
        OnAttackTargetChanged(attackTarget);
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

    private bool IsAttackTarget(Entity entity)
    {
        return attackTarget && attackTarget.gameObject.GetInstanceID() == entity.gameObject.GetInstanceID();
    }
}