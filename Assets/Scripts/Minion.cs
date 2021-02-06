using UnityEngine;
using UnityEngine.AI;
using Utils;

public class Minion : WalkableUnit
{
    [HideInInspector] public Vector3[] destinations;

    private int _destinationIndex;

    private float _goldValue = 20f;
    
    public override void OnReceiveAction(GameObject actor)
    {
        base.OnReceiveAction(actor);
        if (actor.CompareTag(Tags.Champion.Value))
        {
            var champion = actor.GetComponent<Champion>();
            champion.SetAttackTarget(this);
        }
    }

    //Base class methods
    protected override void OnDie(Unit actor, Unit target)
    {
        if (actor.CompareTag(Tags.Champion.Value))
        {
            var champion = actor as Champion;
            champion.AddGold(_goldValue);
        }

        Destroy(gameObject);
    }

    protected override void Start()
    {
        base.Start();
        Move(destinations[_destinationIndex++]);
    }

    protected override void Update()
    {
        base.Update();
        if (_destinationIndex < destinations.Length && Vector3.Distance(PhysicsUtils.Vector3Y0(transform.position), destinations[_destinationIndex-1]) < 1f)
        {
            Move(destinations[_destinationIndex++]);
        }
    }
}