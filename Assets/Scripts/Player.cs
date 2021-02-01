using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.AI;

public class Player : Unit
{
    [SerializeField]
    private LayerMask whatIsGround;

    [SerializeField]
    private Camera playerCamera;

    private NavMeshAgent navMeshAgent;

    //Attack session
    private Unit attackUnit;

    [SerializeField]
    private float basicAttackRange = 6f;

    [SerializeField]
    private float attackSpeed = 1f;

    private float attackSpeedCooldown;

    [SerializeField]
    private GameObject basicAttackPrefab;

    //Gold session
    [SerializeField]
    private float gold;

    //Debug section
    [SerializeField]
    private bool showAttackRange;

    private void Awake()
    {
        attackSpeedCooldown = 1f / attackSpeed;
    }

    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        attackSpeedCooldown += Time.deltaTime;
        if (attackSpeedCooldown >= 1f / attackSpeed)
        {
            attackSpeedCooldown = 1f / attackSpeed;
        }

            if (attackUnit)
        {
            if (Vector3.Distance(transform.position, attackUnit.transform.position) >= basicAttackRange)
            {
                Debug.Log("Moving!");
                Move(attackUnit.transform.position);
            }
            else
            {
                navMeshAgent.destination = transform.position;

                if(attackSpeedCooldown >= 1f/attackSpeed)
                {
                    attackSpeedCooldown = 0;
                    GameObject _basicAttackPrefab = Instantiate(basicAttackPrefab, transform.position, Quaternion.identity);
                    BasicAttack basicAttack = _basicAttackPrefab.GetComponent<BasicAttack>();
                    basicAttack.Follow(attackUnit);
                    basicAttack.OnHit += target =>
                    {
                        gold += 1f;
                        Destroy(basicAttack);
                    };
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (showAttackRange)
        {
            Gizmos.DrawWireSphere(transform.position, basicAttackRange);
        }
    }

    public void OnActionInput(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            attackUnit = null;
            //Obtendo o raio apontando da posição da câmera até o ponto da posição do clique do mouse
            Ray ray = playerCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            RaycastHit hit;

            //Verificando se o raio acerta algum GameObject
            if (Physics.Raycast(ray, out hit, whatIsGround))
            {
                GameObject hitGameObject = hit.collider.gameObject;
                if (hitGameObject)
                {
                    Interactable interactable = null;

                    if(hitGameObject.CompareTag(Tags.Minion.Value))
                    {
                        interactable = hitGameObject.GetComponent<Minion>() as Interactable;
                    } else if(hitGameObject.CompareTag(Tags.Map.Value))
                    {
                        Move(hit.point);
                    }

                    interactable?.OnAction(gameObject);
                }
            }
        }
    }

    public void Move(Vector3 destination)
    {
        if(navMeshAgent.destination != destination)
            navMeshAgent.destination = destination;
    }

    public void Attack(Unit target)
    {
        attackUnit = target;
    }
}
