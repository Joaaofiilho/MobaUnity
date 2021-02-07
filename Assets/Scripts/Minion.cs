using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utils;

[RequireComponent(typeof(SphereCollider))]
public class Minion : WalkableUnit
{
    private SphereCollider _sphereCollider;
    [HideInInspector] public Vector3[] destinations;
    
    private Unit _priorityAttackChampionTarget;
    
    private readonly List<Entity> _nearEnemyEntities = new List<Entity>();
    private readonly List<Unit> _nearEnemyChampions = new List<Unit>();

    private float _forgetChampionMaxRange;

    private int _destinationIndex;

    private float _goldValue = 20f;

    [SerializeField] private LayerMask whatIsEnemy;

    //Unity methods
    protected override void Start()
    {
        base.Start();
        _sphereCollider = GetComponent<SphereCollider>();
        _sphereCollider.radius = statistics.basicAttackRange / transform.localScale.y;
        _forgetChampionMaxRange = statistics.basicAttackRange + 1f;
        Move(destinations[_destinationIndex]);
    }

    protected override void Update()
    {
        base.Update();

        if (_priorityAttackChampionTarget)
        {
            var distanceToChampion = Vector3.Distance(PhysicsUtils.Vector3Y0(transform.position),
                PhysicsUtils.Vector3Y0(_priorityAttackChampionTarget.transform.position));
            
            if (distanceToChampion > _forgetChampionMaxRange)
            {
                _priorityAttackChampionTarget = null;
                SetAttackTarget(null);
            }
        } else if (attackTarget)
        {
            if (attackTarget is Champion)
            {
                var distanceToChampion = Vector3.Distance(PhysicsUtils.Vector3Y0(transform.position),
                    PhysicsUtils.Vector3Y0(attackTarget.transform.position));
            
                if (distanceToChampion > _forgetChampionMaxRange)
                {
                    _priorityAttackChampionTarget = null;
                    SetAttackTarget(null);
                }
            }
        }

        if (!attackTarget)
        {
            Entity nearestEnemyEntity = null;
            if (_nearEnemyEntities.Count > 0)
            {
                nearestEnemyEntity = PhysicsUtils.FindNearestEnemyEntity(transform, team, _nearEnemyEntities);

                SetAttackTarget(nearestEnemyEntity);
            }
            
            if (!nearestEnemyEntity && _nearEnemyChampions.Count > 0)
            {
                var nearestChampion = PhysicsUtils.FindNearestEnemyEntity(transform, team, _nearEnemyChampions.ConvertAll(unit => unit as Entity));

                SetAttackTarget(nearestChampion);
            }
        }
        
        if (!attackTarget && !isOnAttackRange)
        {
            Move(destinations[_destinationIndex]);
        }
        
        if (_destinationIndex < destinations.Length && Vector3.Distance(PhysicsUtils.Vector3Y0(transform.position),
            destinations[_destinationIndex]) < 2f)
        {
            Move(destinations[++_destinationIndex]);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag(Tags.Minion.Value) || other.gameObject.CompareTag(Tags.Tower.Value))
        {
            var entity = other.gameObject.GetComponent<Entity>();
            var isEnemyEntity = entity && entity.team != team;

            if (isEnemyEntity)
            {
                var hasUnit = _nearEnemyEntities.Count(minionUnit =>
                    minionUnit && minionUnit.gameObject.GetInstanceID() == entity.gameObject.GetInstanceID()) > 0;

                if (!hasUnit)
                {
                    _nearEnemyEntities.Add(entity);
                    entity.OnDieCallback += OnEntityDie;
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
        if (other.gameObject.CompareTag(Tags.Minion.Value) || other.gameObject.CompareTag(Tags.Tower.Value))
        {
            var unit = other.gameObject.GetComponent<Unit>();
            _nearEnemyEntities.Remove(unit);
        }

        if (other.gameObject.CompareTag(Tags.Champion.Value))
        {
            var unit = other.gameObject.GetComponent<Unit>();

            _nearEnemyChampions.Remove(unit);
        }
    }

    //Override methods
    protected override void WhenLoseTarget()
    {
        PauseMovement(false);
        Move(destinations[_destinationIndex]);
    }

    protected override void OnDie(Unit actor, Entity target)
    {
        if (actor && actor.CompareTag(Tags.Champion.Value))
        {
            var champion = actor as Champion;
            champion.AddGold(_goldValue);
        }
        
        base.OnDie(actor, target);
    }

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        foreach (var destination in destinations)
        {
            Gizmos.DrawWireSphere(destination, 1f);
        }
    }

    //Class methods
    private void OnChampionHitOther(Unit actor, Unit target, AttackInformation[] attackInformations)
    {
        if (!_priorityAttackChampionTarget)
        {
            if (target is Champion && attackInformations.Length > 0)
            {
                _priorityAttackChampionTarget = target;
                SetAttackTarget(_priorityAttackChampionTarget);
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
            SetAttackTarget(null);
        }
    }

    private void OnEntityDie(Unit actor, Entity target)
    {
        _nearEnemyEntities.Remove(target as Unit);
    }
}