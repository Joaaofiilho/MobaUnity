using System;
using UnityEngine;
using Utils;

public class Champion : WalkableUnit
{
    private float _gold;

    protected override void Start()
    {
        base.Start();
        statistics.AttackDamage = 62f;
    }
    
    /// <summary>
    /// Method called when an actions occurs on this target (Default = [Mouse] Right click).
    /// </summary>
    /// <param name="target">The target of the action.</param>
    /// <param name="hitPoint">The position where the ray intercepted the object.</param>
    public void OnDoAction(GameObject target, Vector3 hitPoint)
    {
        SetAttackTarget(null);

        if (target.CompareTag(Tags.Minion.Value))
        {
            var minion = target.GetComponent<Minion>();

            if (minion.team == team)
            {
                StopMoving();
                Move(hitPoint);
            }
            else
            {
                minion.OnReceiveAction(gameObject);
                SetAttackTarget(minion);
            }
        }
        else if (target.CompareTag(Tags.Map.Value))
        {
            StopMoving();
            Move(hitPoint);
        }
    }
    
    //Overrides e Callbacks
    protected override AttackInformation[] GetBasicAttackDamage()
    {
        return new[]
        {
            new AttackInformation(statistics.AttackDamage / 2f, DamageType.AttackDamage),
            new AttackInformation(statistics.AttackDamage, DamageType.SpellDamage),
            new AttackInformation(statistics.AttackDamage / 4f, DamageType.TrueDamage),
        };
    }

    public event Action<float> OnGoldChangedCallback = delegate(float gold) { };

    protected virtual void OnGoldChanged(float gold)
    {
        OnGoldChanged(gold);    
    }
    
    //Class methods
    public void StopAll()
    {
        StopMoving();
        SetAttackTarget(null);
    }

    /// <summary>
    /// Increases the champion's gold amount.
    /// <para>The gold amount must be greater than 0.</para>
    /// </summary>
    /// <param name="amount">The amount of gold to add to the champion.</param>
    public void AddGold(float amount)
    {
        _gold += amount;
        OnGoldChangedCallback(_gold);
    }
}