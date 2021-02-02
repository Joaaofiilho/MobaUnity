using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicAttack : MonoBehaviour
{
    private Unit target;

    public delegate void HitCallback(GameObject target);
    public HitCallback OnHit;

    [SerializeField]
    private float basicAttackAnimationSpeed = 0.5f;

    private void FixedUpdate()
    {
        if(target)
        {
            transform.position = Vector3.Lerp(transform.position, target.transform.position, basicAttackAnimationSpeed);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Unit unit = other.gameObject.GetComponent<Unit>();
        Debug.Log("Unit basic attack hitted: " + unit);

        if(other.gameObject.CompareTag(Tags.Minion.Value))
        {
            OnHit(gameObject);
        }
    }

    public void Follow(Unit target)
    {
        this.target = target;
    }
}
