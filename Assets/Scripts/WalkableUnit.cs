using UnityEngine;
using UnityEngine.AI;
using Utils;

[RequireComponent(typeof(NavMeshAgent))]
public abstract class WalkableUnit : Unit
{
    private NavMeshAgent _agent;
    
    //Unity methods
    protected override void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _agent.updateRotation = false;
    }

    protected override void Update()
    {
        base.Update();

        if (attackTarget)
        {
            if (isOnAttackRange)
            {
                PauseMovement(true);
            }
            else
            {
                Move(PhysicsUtils.Vector3Y0(attackTarget.transform.position));
            }
        }
        else
        {
            PauseMovement(false);
        }
    }

    protected void LateUpdate()
    {
        if (_agent.velocity.sqrMagnitude > Mathf.Epsilon)
        {
            transform.rotation = Quaternion.LookRotation(_agent.velocity.normalized);
        }
    }
    
    //Abstract methods
    protected abstract void WhenLoseTarget();
    
    //Override methods
    protected override void OnAttackTargetChanged(Entity target)
    {
        if (!target)
        {
            WhenLoseTarget();
        }

        base.OnAttackTargetChanged(target);
    }
    
    //Class methods
    protected void StopMoving()
    {
        _agent.velocity = Vector3.zero;
        _agent.ResetPath();
    }

    protected void PauseMovement(bool shouldPause)
    {
        _agent.isStopped = shouldPause;
    }

    protected void Move(Vector3 destination)
    {
        if (_agent.destination != destination)
        {
            _agent.destination = destination;
        }
    }
}