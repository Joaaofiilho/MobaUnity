using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.PlayerLoop;
using Utils;

public class Tower : Unit
{
    private Unit _priorityAttackChampionTarget;
    private SphereCollider _sphereCollider;

    private readonly List<Unit> _nearMinions = new List<Unit>();
    private readonly List<Unit> _nearChampions = new List<Unit>();

    [SerializeField] private float _increaseDamageCooldown = 1f;
    private float _increaseDamageCooldownRemaining;
    private int _numOfSequentialBasicAttacks;
    [SerializeField] private int _maxSequentialAttackDamageIncrease = 4;
    [SerializeField] [Range(0, 1)] private float increaseDamageRate = 0.2f;

    //Unity methods
    protected override void Awake()
    {
        base.Awake();
        ShouldLoseTargetWhenOutsideRange = true;
        Statistics.AttackDamage = 32f;
        Statistics.BasicAttackRange = 8f;
        Statistics.AttackSpeed = 0.65f;
        attackChannelingTime = 0.5f;
        _increaseDamageCooldownRemaining = _increaseDamageCooldown;

        _sphereCollider = GetComponentInChildren<SphereCollider>();
        _sphereCollider.radius = transform.localScale.y / (Statistics.BasicAttackRange / 2);
    }

    protected override void Update()
    {
        base.Update();

        if (_increaseDamageCooldownRemaining <= 0)
        {
            _numOfSequentialBasicAttacks = 0;
        }

        _increaseDamageCooldownRemaining -= Time.deltaTime;
    }

    private void FixedUpdate()
    {
        if (_priorityAttackChampionTarget)
        {
            SetAttackTarget(_priorityAttackChampionTarget);
        }
        else if (!attackTarget)
        {
            Unit nearestMinion = null;
            if (_nearMinions.Count > 0)
            {
                nearestMinion = FindNearestUnitByList(_nearMinions);

                SetAttackTarget(nearestMinion);
            }
            
            if (!nearestMinion && _nearChampions.Count > 0)
            {
                var nearestChampion = FindNearestUnitByList(_nearChampions);

                SetAttackTarget(nearestChampion);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //Se o collider for uma Unit, com exceção de Tower.
        if (other.gameObject.CompareTag(Tags.Minion.Value))
        {
            var unit = other.gameObject.GetComponent<Unit>();

            var hasUnit = _nearMinions.Count(minionUnit =>
                minionUnit && minionUnit.gameObject.GetInstanceID() == unit.gameObject.GetInstanceID()) > 0;

            if (!hasUnit)
            {
                _nearMinions.Add(unit);
                unit.OnDieCallback += OnMinionDie;
            }
        }

        if (other.gameObject.CompareTag(Tags.Champion.Value))
        {
            var unit = other.gameObject.GetComponent<Unit>();

            var hasUnit = _nearChampions.Count(championUnit =>
                championUnit && championUnit.gameObject.GetInstanceID() == unit.gameObject.GetInstanceID()) > 0;

            if (!hasUnit)
            {
                _nearChampions.Add(unit);
                unit.OnHitCallback += OnChampionHitOther;
                unit.OnDieCallback += OnChampionDie;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        //Se o collider for uma Unit, com exceção de Tower.
        if (other.gameObject.CompareTag(Tags.Minion.Value))
        {
            var unit = other.gameObject.GetComponent<Unit>();
            _nearMinions.Remove(unit);
        }

        if (other.gameObject.CompareTag(Tags.Champion.Value))
        {
            var unit = other.gameObject.GetComponent<Unit>();

            if (_priorityAttackChampionTarget && _priorityAttackChampionTarget.gameObject.GetInstanceID() ==
                other.gameObject.GetInstanceID())
            {
                _priorityAttackChampionTarget = null;
            }
            
            _nearChampions.Remove(unit);
        }
    }

    //Override methods
    protected override AttackInformation[] GetBasicAttackDamage()
    {
        return new[]
        {
            new AttackInformation(
                increaseDamageRate * Statistics.AttackDamage * _numOfSequentialBasicAttacks + Statistics.AttackDamage,
                DamageType.AttackDamage)
        };
    }

    protected override void OnAttack()
    {
        base.OnAttack();
        _increaseDamageCooldownRemaining = _increaseDamageCooldown;

        if (_numOfSequentialBasicAttacks < _maxSequentialAttackDamageIncrease)
        {
            _numOfSequentialBasicAttacks++;
        }
    }

    //Class methods
    private Unit FindNearestUnitByList(List<Unit> units)
    {
        var bestDistance = float.MaxValue;
        Unit nearestUnit = null;

        var position = PhysicsUtils.Vector3Y0(transform.position);

        foreach (var unit in units)
        {
            if (unit)
            {
                var newDistance = Vector3.Distance(position, PhysicsUtils.Vector3Y0(unit.transform.position));
                if (newDistance < bestDistance)
                {
                    bestDistance = newDistance;
                    nearestUnit = unit;
                }
            }
        }

        return nearestUnit;
    }

    private void OnChampionHitOther(Unit actor, Unit target, AttackInformation[] attackInformations)
    {
        if (!_priorityAttackChampionTarget)
        {
            if (target is Champion && attackInformations.Length > 0)
            {
                _priorityAttackChampionTarget = target;
            }
        }
    }

    private void OnChampionDie(Unit actor, Unit target)
    {
        _nearChampions.Remove(target);
        if (_priorityAttackChampionTarget && _priorityAttackChampionTarget.gameObject.GetInstanceID() ==
            actor.gameObject.GetInstanceID())
        {
            _priorityAttackChampionTarget = null;
        }
    }

    private void OnMinionDie(Unit actor, Unit target)
    {
        _nearMinions.Remove(target);
    }
}