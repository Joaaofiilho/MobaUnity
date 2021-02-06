using System;
using System.Collections;
using System.Collections.Generic;
using Interface;
using UnityEngine;

public class DamagePopup : MonoBehaviour
{
    private static float _multipleDamageYOffset = 1f;
    
    public static void Create(Transform targetTransform, AttackInformation[] attackInformations)
    {
        var multipleDamageYOffset = 0f;
        
        foreach (var attackInformation in attackInformations)
        {
            var damagePopupPrefab = Instantiate(GlobalPrefabs.i.damagePopupPrefab);

            //Setting the position of the damage popup to be a little ahead from who is taking damage;
            var position = targetTransform.position;
            var topOfTargetTransform = Vector3.up * targetTransform.localScale.y;
            var offsetFromTop = Vector3.up * 0.6f;
            var vectorMultipleDamageOffset = Vector3.up * multipleDamageYOffset;
            position += topOfTargetTransform + offsetFromTop + vectorMultipleDamageOffset;
        
            damagePopupPrefab.transform.position = position;
            damagePopupPrefab.GetComponentInChildren<DamagePopupUIHandler>().StyleText(attackInformation);

            multipleDamageYOffset += _multipleDamageYOffset;
        }
    }

    private void LateUpdate()
    {
        transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position);
    }

    public void OnAnimationEnd()
    {
        Destroy(transform.parent.gameObject);
    }
}
