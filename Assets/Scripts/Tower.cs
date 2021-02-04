using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utils;

public class Tower : Unit
{
    private Unit _priorityAttackChampionTarget;
    private SphereCollider _sphereCollider;

    private readonly List<Unit> _nearMinions = new List<Unit>();
    private readonly List<Unit> _nearChampions = new List<Unit>();
    
    //Unity methods
    protected override void Awake()
    {
        base.Awake();
        Statistics.AttackDamage = 32f;
        Statistics.BasicAttackRange = 8f;
        Statistics.AttackSpeed = 0.65f;
        attackChannelingTime = 0.5f;

        _sphereCollider = GetComponentInChildren<SphereCollider>();
        _sphereCollider.radius = transform.localScale.y / (Statistics.BasicAttackRange / 2);
    }

    private void FixedUpdate()
    {
        Debug.Log("Champion count: " + _nearChampions.Count);
        Debug.Log("Minion count: " + _nearMinions.Count);
        if (_priorityAttackChampionTarget)
        {
            attackTarget = _priorityAttackChampionTarget;
        }
        else if(!attackTarget) {
            if (_nearMinions.Count > 0)
            {
                var nearestMinion = FindNearestUnitByList(_nearMinions);

                attackTarget = nearestMinion;
            } else if (_nearChampions.Count > 0)
            {
                var nearestChampion = FindNearestUnitByList(_nearChampions);

                attackTarget = nearestChampion;
            }
        }
        // else
        // {
        //     if (!attackTarget)
        //     {
        //         
        //     }
        //     // if (!attackTarget)
        //     // {
        //     //     var nearestColliders = Physics.OverlapSphere(PhysicsUtils.Vector3Y0(transform.position),
        //     //         Statistics.BasicAttackRange / 2f);
        //     //
        //     //     var nearestUnits = nearestColliders
        //     //         .Where(col =>
        //     //             GameUtils.TagsOfAllUnits().Contains(col.gameObject.tag) &&
        //     //             !col.gameObject.tag.Equals(Tags.Tower.Value))
        //     //         .ToList()
        //     //         .ConvertAll(col => col.gameObject.GetComponent<Unit>());
        //     //
        //     //     var nearestUnit = FindNearestUnitByList(nearestUnits);
        //     //
        //     //     if (nearestUnit)
        //     //     {
        //     //         attackTarget = nearestUnit;
        //     //     }
        //     // }
        // }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trying to add: " + other.gameObject.name);
        //Se o collider for uma Unit, com exceção de Tower.
        if (other.gameObject.CompareTag(Tags.Minion.Value))
        {
            var unit = other.gameObject.GetComponent<Unit>();

            var hasUnit = _nearMinions.Count(minionUnit => minionUnit.gameObject.GetInstanceID() == unit.gameObject.GetInstanceID()) > 0;

            if (!hasUnit)
            {
                _nearMinions.Add(unit);
                unit.OnDied += OnMinionDie;
                Debug.Log("Added: " + unit.gameObject.name);
            }
        }
        
        if (other.gameObject.CompareTag(Tags.Champion.Value))
        {
            var unit = other.gameObject.GetComponent<Unit>();

            var hasUnit = _nearChampions.Count(championUnit => championUnit.gameObject.GetInstanceID() == unit.gameObject.GetInstanceID()) > 0;

            if (!hasUnit)
            {
                _nearChampions.Add(unit);
                unit.OnHit += OnChampionAttacked;
                unit.OnDied += OnChampionDie;
                Debug.Log("Added: " + unit.gameObject.name);
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

            Debug.Log("Removed: " + unit.gameObject.name);
        }

        if (other.gameObject.CompareTag(Tags.Champion.Value))
        {
            var unit = other.gameObject.GetComponent<Unit>();
            _nearChampions.Remove(unit);
            
            Debug.Log("Removed: " + unit.gameObject.name);
        }
    }

    //Base class methods
    protected override void OnDie(Unit actor)
    {
        
    }
    
    //Class methods
    private Unit FindNearestUnitByList(List<Unit> units)
    {
        var bestDistance = float.MaxValue;
        Unit nearestUnit = null;
        
        var position = PhysicsUtils.Vector3Y0(transform.position);
        
        foreach (var unit in units)
        {
            var newDistance = Vector3.Distance(position, PhysicsUtils.Vector3Y0(unit.transform.position));
            if (newDistance < bestDistance)
            {
                bestDistance = newDistance;
                nearestUnit = unit;
            }
        }

        return nearestUnit;
    }

    private void OnChampionAttacked(Unit actor, Unit target)
    {
        if (!_priorityAttackChampionTarget)
        {
            if (target is Champion)
            {
                _priorityAttackChampionTarget = actor;
                actor.GetComponent<Champion>().OnDied += OnChampionDie;
            }
        }
    }

    private void OnChampionDie(Unit actor, Unit target)
    {
        _nearChampions.Remove(target);
        if (_priorityAttackChampionTarget && _priorityAttackChampionTarget.gameObject.GetInstanceID() == actor.gameObject.GetInstanceID())
        {
            _priorityAttackChampionTarget = null;
        }
    }

    private void OnMinionDie(Unit actor, Unit target)
    {
        _nearMinions.Remove(target);
    }
}