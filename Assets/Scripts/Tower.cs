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

    private readonly List<Unit> _nearEnemyMinions = new List<Unit>();
    private readonly List<Unit> _nearEnemyChampions = new List<Unit>();

    [SerializeField] private float increaseDamageCooldown = 1f;
    private float _increaseDamageCooldownRemaining;
    private int _numOfSequentialBasicAttacks;
    [SerializeField] private int maxSequentialAttackDamageIncrease = 4;
    [SerializeField] [Range(0, 1)] private float increaseDamageRate = 0.2f;

    //Unity methods
    protected override void Awake()
    {
        base.Awake();
        ShouldLoseTargetWhenOutsideRange = true;
        statistics.AttackDamage = 32f;
        statistics.BasicAttackRange = 8f;
        statistics.AttackSpeed = 0.65f;
        attackChannelingTime = 0.5f;
        _increaseDamageCooldownRemaining = increaseDamageCooldown;

        _sphereCollider = GetComponentInChildren<SphereCollider>();
        _sphereCollider.radius = transform.localScale.y / (statistics.BasicAttackRange / 2);
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
            Unit nearestEnemyMinion = null;
            if (_nearEnemyMinions.Count > 0)
            {
                nearestEnemyMinion = FindNearestEnemyUnit(_nearEnemyMinions);

                SetAttackTarget(nearestEnemyMinion);
            }
            
            if (!nearestEnemyMinion && _nearEnemyChampions.Count > 0)
            {
                var nearestChampion = FindNearestEnemyUnit(_nearEnemyChampions);

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
            var isEnemyUnit = unit && unit.team != team;

            if (isEnemyUnit)
            {
                var hasUnit = _nearEnemyMinions.Count(minionUnit =>
                    minionUnit && minionUnit.gameObject.GetInstanceID() == unit.gameObject.GetInstanceID()) > 0;

                if (!hasUnit)
                {
                    _nearEnemyMinions.Add(unit);
                    unit.OnDieCallback += OnMinionDie;
                }
            }
        }

        if (other.gameObject.CompareTag(Tags.Champion.Value))
        {
            var unit = other.gameObject.GetComponent<Unit>();
            var isEnemyUnit = unit && unit.team != team;

            if (isEnemyUnit)
            {
                var hasUnit = _nearEnemyChampions.Count(championUnit =>
                    championUnit && championUnit.gameObject.GetInstanceID() == unit.gameObject.GetInstanceID()) > 0;

                if (!hasUnit)
                {
                    _nearEnemyChampions.Add(unit);
                    unit.OnHitCallback += OnChampionHitOther;
                    unit.OnDieCallback += OnChampionDie;
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        //Se o collider for uma Unit, com exceção de Tower.
        if (other.gameObject.CompareTag(Tags.Minion.Value))
        {
            var unit = other.gameObject.GetComponent<Unit>();
            _nearEnemyMinions.Remove(unit);
        }

        if (other.gameObject.CompareTag(Tags.Champion.Value))
        {
            var unit = other.gameObject.GetComponent<Unit>();

            if (_priorityAttackChampionTarget && _priorityAttackChampionTarget.gameObject.GetInstanceID() ==
                other.gameObject.GetInstanceID())
            {
                _priorityAttackChampionTarget = null;
            }
            
            _nearEnemyChampions.Remove(unit);
        }
    }

    //Override methods
    protected override AttackInformation[] GetBasicAttackDamage()
    {
        return new[]
        {
            new AttackInformation(
                increaseDamageRate * statistics.AttackDamage * _numOfSequentialBasicAttacks + statistics.AttackDamage,
                DamageType.AttackDamage)
        };
    }

    protected override void OnAttack()
    {
        base.OnAttack();
        _increaseDamageCooldownRemaining = increaseDamageCooldown;

        if (_numOfSequentialBasicAttacks < maxSequentialAttackDamageIncrease)
        {
            _numOfSequentialBasicAttacks++;
        }
    }

    //Class methods
    private Unit FindNearestEnemyUnit(List<Unit> units)
    {
        var bestDistance = float.MaxValue;
        Unit nearestUnit = null;

        var position = PhysicsUtils.Vector3Y0(transform.position);

        foreach (var unit in units)
        {
            if (unit && unit.team != team)
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

    private void OnChampionDie(Unit actor, Entity target)
    {
        _nearEnemyChampions.Remove(target as Unit);
        if (_priorityAttackChampionTarget && _priorityAttackChampionTarget.gameObject.GetInstanceID() ==
            actor.gameObject.GetInstanceID())
        {
            _priorityAttackChampionTarget = null;
        }
    }

    private void OnMinionDie(Unit actor, Entity target)
    {
        _nearEnemyMinions.Remove(target as Unit);
    }
}