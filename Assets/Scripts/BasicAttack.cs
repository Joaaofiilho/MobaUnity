using System;
using UnityEngine;
using Utils;

public class BasicAttack : MonoBehaviour
{
    private AttackInformation[] _attackInformations;
    private Unit _actor;
    private Unit _attackTarget;
    private Vector3 _lastKnownPosition;
    private float _basicAttackAnimationSpeed;

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

        transform.position = Vector3.MoveTowards(transform.position, _lastKnownPosition,
            _basicAttackAnimationSpeed * Time.fixedDeltaTime);
    }

    private void LateUpdate()
    {
        transform.LookAt(Camera.main.transform.position);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_attackTarget)
        {
            if (other.gameObject.GetInstanceID() == _attackTarget.gameObject.GetInstanceID() && other is CapsuleCollider)
            {
                _attackTarget.TakeDamage(_actor, _attackInformations);
                Destroy(gameObject);
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Fire(Unit actor, Unit target, float basicAttackAnimationSpeed, AttackInformation[] attackInformations)
    {
        _actor = actor;
        _attackTarget = target;
        _basicAttackAnimationSpeed = basicAttackAnimationSpeed;
        _attackInformations = attackInformations;
    }

    public void Fire(Unit actor, Unit target, float basicAttackAnimationSpeed, AttackInformation attackInformation)
    {
        Fire(actor, target, basicAttackAnimationSpeed, new []{ attackInformation });
    }
}