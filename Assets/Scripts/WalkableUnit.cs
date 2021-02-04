using UnityEngine;
using UnityEngine.AI;
using Utils;

[RequireComponent(typeof(NavMeshAgent))]
public abstract class WalkableUnit : Unit
{
    protected NavMeshAgent Agent;
    
    /// <summary>
    /// If true, the unit will seek the target;
    /// </summary>
    public bool seekTarget = true;

    protected virtual void Start()
    {
        Agent = GetComponent<NavMeshAgent>();
        Agent.updateRotation = false;
    }

    protected override void Update()
    {
        var previousAttackTarget = attackTarget;
        base.Update();
        
        if (seekTarget)
        {
            attackTarget = previousAttackTarget;
        }
        
        if (attackTarget)
        {
            if (isOnAttackRange)
            {
                StopMoving();
            }
            else
            {
                Move(PhysicsUtils.Vector3Y0(attackTarget.transform.position));
            }
        }
    }

    protected void LateUpdate()
    {
        if (Agent.velocity.sqrMagnitude > Mathf.Epsilon)
        {
            transform.rotation = Quaternion.LookRotation(Agent.velocity.normalized);
        }
    }

    //Class methods
    protected void StopMoving()
    {
        Agent.velocity = Vector3.zero;
        Agent.ResetPath();
    }

    protected void Move(Vector3 destination)
    {
        if (Agent.destination != destination)
        {
            Agent.destination = destination;
        }
    }
}