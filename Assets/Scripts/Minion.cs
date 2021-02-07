using System.Linq;
using UnityEngine;
using Utils;

public class Minion : WalkableUnit
{
    [HideInInspector] public Vector3[] destinations;

    private int _destinationIndex;

    private float _goldValue = 20f;

    private float searchEnemiesRange;

    [SerializeField] private LayerMask whatIsEnemy;

    //Unity methods
    protected override void Start()
    {
        base.Start();
        searchEnemiesRange = statistics.AttackSpeed + 1f;
        Move(destinations[_destinationIndex++]);
    }

    protected override void Update()
    {
        base.Update();
        if (_destinationIndex < destinations.Length && Vector3.Distance(PhysicsUtils.Vector3Y0(transform.position),
            destinations[_destinationIndex - 1]) < 1f)
        {
            Move(destinations[_destinationIndex++]);
        }

        // SearchNearEnemies();
    }

    //Override methods
    protected override void OnDie(Unit actor, Entity target)
    {
        if (actor.CompareTag(Tags.Champion.Value))
        {
            var champion = actor as Champion;
            champion.AddGold(_goldValue);
        }

        Destroy(gameObject);
    }

    //Class methods
    private void SearchNearEnemies()
    {
        if (!attackTarget)
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, searchEnemiesRange, whatIsEnemy);

            if (colliders != null)
            {
                //Finding all GameObjects nearby who are enemies
                var nearEnemyGameObjects = colliders.ToList().ConvertAll(col => col.gameObject)
                    .Where(go => LayerUtils.LayerIsOnLayerMask(go.layer, whatIsEnemy)).ToList();

                var transforms = nearEnemyGameObjects.ConvertAll(go => go.transform).ToArray();

                var nearestEnemy = transforms.Length > 1
                    ? PhysicsUtils.FindNearestTransform(transform, transforms)
                    : transforms[0];

                SetAttackTarget(nearestEnemy.GetComponent<Unit>());
            }
        }
    }
}