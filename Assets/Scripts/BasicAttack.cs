using System;
using UnityEngine;
using Utils;

public class BasicAttack : MonoBehaviour
{
    private float _attackDamage;
    private Unit _actor;
    private Unit _attackTarget;
    private Vector3 _lastKnownPosition;

    public delegate void HitCallback(Unit target);

    public HitCallback OnHit;

    [SerializeField] private float basicAttackAnimationSpeed;

    private void FixedUpdate()
    {
        if (_attackTarget)
        {
            _lastKnownPosition = _attackTarget.transform.position;
        }
        else
        {
            var distance = Vector3.Distance(transform.position, _lastKnownPosition);
            if (distance < 0.1f)
            {
                Destroy(gameObject);
            }
        }

        transform.position += (_lastKnownPosition - transform.position).normalized * basicAttackAnimationSpeed * Time.fixedDeltaTime;
    }

    private void OnDestroy()
    {
        OnHit = null;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Triggering! | targetID = " + _attackTarget.GetInstanceID() + " | collision ID = " + other.gameObject
            .GetInstanceID());
        if (other.gameObject.GetInstanceID() == _attackTarget.gameObject.GetInstanceID())
        {
            _attackTarget.TakeDamage(_actor, _attackDamage);
            OnHit?.Invoke(_attackTarget);
            Destroy(gameObject);
        }
    }

    public void Follow(Unit actor, Unit target, float damage, float basicAttackAnimationSpeed)
    {
        _attackDamage = damage;
        _actor = actor;
        _attackTarget = target;
        this.basicAttackAnimationSpeed = basicAttackAnimationSpeed;
    }
}