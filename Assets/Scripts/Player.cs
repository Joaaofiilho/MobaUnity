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
    private Unit attackFocus;

    [SerializeField]
    private float basicAttackRange = 6f;

    [SerializeField]
    private float attackSpeed = 1f;

    private float attackSpeedCooldown;

    [SerializeField]
    private float attackChannelingTime = 0.2f;

    private float attackChannelingCooldown;

    [SerializeField]
    private GameObject basicAttackPrefab;

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
        navMeshAgent.updateRotation = false;
    }

    private void Update()
    {
        attackSpeedCooldown += Time.deltaTime;
        if (attackSpeedCooldown >= 1f / attackSpeed)
        {
            attackSpeedCooldown = 1f / attackSpeed;
        }

        if (attackFocus)
        {
            if (Vector3.Distance(transform.position, attackFocus.transform.position) >= basicAttackRange)
            {
                Debug.Log("Moving to attack");
                Move(attackFocus.transform.position);
            }
            else
            {
                StopMoving();

                if(attackSpeedCooldown >= 1f/attackSpeed)
                {
                    attackChannelingCooldown += Time.deltaTime;

                    if (attackChannelingCooldown >= attackChannelingTime)
                    {
                        Debug.Log("Attacking");
                        attackSpeedCooldown = 0;
                        GameObject _basicAttackPrefab = Instantiate(basicAttackPrefab, transform.position, Quaternion.identity);
                        BasicAttack basicAttack = _basicAttackPrefab.GetComponent<BasicAttack>();
                        basicAttack.Follow(attackFocus);
                        basicAttack.OnHit += target =>
                        {
                            Destroy(_basicAttackPrefab);
                        };
                    }
                }
            }
        } else
        {
            attackChannelingCooldown = 0;
        }
    }


    private void LateUpdate()
    {
        if (navMeshAgent.velocity.sqrMagnitude > Mathf.Epsilon)
        {
            transform.rotation = Quaternion.LookRotation(navMeshAgent.velocity.normalized);
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
            attackFocus = null;
            //Obtendo o raio apontando da posição da câmera até o ponto da posição do clique do mouse
            Ray ray = playerCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            RaycastHit hit;

            //Verificando se o raio acerta algum GameObject
            if (Physics.Raycast(ray, out hit))
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
                        StopMoving();
                        Move(hit.point);
                    }

                    interactable?.OnAction(gameObject);
                }
            }
        }
    }

    public void Move(Vector3 destination)
    {
        if (navMeshAgent.destination != destination)
        {
            navMeshAgent.destination = destination;
        }
    }

    public void Attack(Unit target)
    {
        attackFocus = target;
    }

    public void StopAll()
    {
        StopMoving();
        attackFocus = null;
    }

    private void StopMoving()
    {
        navMeshAgent.velocity = Vector3.zero;
        navMeshAgent.ResetPath();
    }
}
