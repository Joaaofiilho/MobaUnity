using UnityEngine;
using UnityEngine.AI;
using Utils;

[RequireComponent(typeof(NavMeshAgent))]
public abstract class WalkableUnit : Unit
{
    private NavMeshAgent _agent;

    protected virtual void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _agent.updateRotation = false;
    }

    protected override void Update()
    {
        base.Update();
        
        if (!attackTarget) return;

        if (isOnAttackRange)
        {
            StopMoving();
        }
        else
        {
            Move(PhysicsUtils.Vector3Y0(attackTarget.transform.position));
        }
    }

    protected void LateUpdate()
    {
        if (_agent.velocity.sqrMagnitude > Mathf.Epsilon)
        {
            transform.rotation = Quaternion.LookRotation(_agent.velocity.normalized);
        }
    }

    //Class methods
    protected void StopMoving()
    {
        _agent.velocity = Vector3.zero;
        _agent.ResetPath();
    }

    protected void Move(Vector3 destination)
    {
        if (_agent.destination != destination)
        {
            _agent.destination = destination;
        }
    }
}