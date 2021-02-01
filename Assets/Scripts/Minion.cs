using UnityEngine;
using UnityEngine.AI;

public class Minion : Unit
{
    [SerializeField]
    private Transform target;
    
    private NavMeshAgent navMeshAgent;

    public override void OnAction(GameObject actor)
    {
        if(actor.CompareTag(Tags.Player.Value))
        {
            Highlight();

            Player player = actor.GetComponent<Player>();
            player.Attack(this);
        }
    }

    private void Highlight()
    {

    }

    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.destination = target.position;
        Debug.Log(navMeshAgent);
    }
}
