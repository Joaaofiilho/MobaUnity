﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.AI;

public class Player : MonoBehaviour
{
    [SerializeField]
    private LayerMask whatIsGround;

    [SerializeField]
    private Camera playerCamera;

    private NavMeshAgent navMeshAgent;

    private Vector3 destination;

    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        if(destination != null)
            Debug.DrawRay(transform.position, destination.normalized * 2);
    }

    public void Move(InputAction.CallbackContext context)
    {
        //Obtendo o raio apontando da posição da câmera até o ponto da posição do clique do mouse
        Ray ray = playerCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100, whatIsGround))
        {
            navMeshAgent.destination = hit.point;
            destination = hit.point;
        }
    }
}
