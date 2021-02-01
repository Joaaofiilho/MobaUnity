using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Minion : MonoBehaviour
{
    [SerializeField]
    private Transform target;
    
    private NavMeshAgent navMeshAgent;

    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        Debug.Log("Andando ate o ponto: " + target.position);
        navMeshAgent.destination = target.position;
    }
}
