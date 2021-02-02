using UnityEngine;
using Utils;

public class Minion : WalkableUnit
{
    [SerializeField]
    private Transform destination;

    private float _goldValue = 20f;

    public override void Action(GameObject actor)
    {
        if(actor.CompareTag(Tags.Champion.Value))
        {
            var champion = actor.GetComponent<Champion>();
            champion.SetAttackTarget(this);
        }
    }

    //Base class methods
    protected override void OnDie(Unit actor)
    {
        if (actor.CompareTag(Tags.Champion.Value))
        {
            var champion = actor as Champion;
            champion.gold += _goldValue;
        }

        Destroy(gameObject);
    }

    protected override void Start()
    {
        base.Start();
        Move(destination.position);
    }
}