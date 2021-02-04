using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Utils;

public class Champion : WalkableUnit
{
    private float _gold;
    
    //Callbacks
    public event Action<float> OnGoldChanged = delegate(float gold) { };

    protected override void Start()
    {
        base.Start();
        Statistics.AttackDamage = 62f;
    }
    
    /// <summary>
    /// Method called when an actions occurs on this target (Default = [Mouse] Right click).
    /// </summary>
    /// <param name="target">The target of the action.</param>
    /// <param name="hitPoint">The position where the ray intercepted the object.</param>
    public void OnDoAction(GameObject target, Vector3 hitPoint)
    {
        attackTarget = null;

        if (target.CompareTag(Tags.Minion.Value))
        {
            target.GetComponent<Minion>().OnReceiveAction(gameObject);
        }
        else if (target.CompareTag(Tags.Map.Value))
        {
            StopMoving();
            Move(hitPoint);
        }
    }

    //Base class methods
    protected override void OnDie(Unit actor)
    {
        //TODO: What should happen when it dies
    }

    //Class methods
    public void StopAll()
    {
        StopMoving();
        attackTarget = null;
    }


    /// <summary>
    /// Increases the champion's gold amount.
    /// <para>The gold amount must be greater than 0.</para>
    /// </summary>
    /// <param name="amount">The amount of gold to add to the champion.</param>
    public void AddGold(float amount)
    {
        _gold += amount;
        OnGoldChanged(_gold);
    }
}