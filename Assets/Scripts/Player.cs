using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using Utils;

[RequireComponent(typeof(Champion))]
public class Player : MonoBehaviour
{
    [SerializeField] private LayerMask whatIsInteractable;
    
    [HideInInspector] public Champion champion;

    private void Awake()
    {
        champion = GetComponent<Champion>();
    }

    public void OnActionInput(InputAction.CallbackContext context)
    {
        if (!context.started) return;
        
        //Obtendo o raio apontando da posição da câmera até o ponto da posição do clique do mouse
        var ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

        var raycastHits = Physics.RaycastAll(ray, 100f, whatIsInteractable, QueryTriggerInteraction.Ignore);

        GameObject nearestGameObject = null;
        Vector3 point = Vector3.zero;
        var bestDistance = float.MaxValue;
        
        //This method only picks enemy entities or the map.
        foreach (var raycastHit in raycastHits)
        {
            var entity = raycastHit.collider.gameObject.GetComponent<Entity>();
            var isEnemyEntity = entity && entity.team != champion.team;
            var isMap = raycastHit.collider.gameObject.CompareTag(Tags.Map.Value);

            if (isEnemyEntity || isMap)
            {
                var newDistance = Vector3.Distance(raycastHit.point, transform.position);
                if (newDistance < bestDistance)
                {
                    bestDistance = newDistance;
                    nearestGameObject = raycastHit.collider.gameObject;
                    point = raycastHit.point;
                }
            }
        }

        if (nearestGameObject)
        {
            champion.OnDoAction(nearestGameObject, point);
        }
    }

    public void OnStopAllInput(InputAction.CallbackContext context)
    {
        champion.StopAll();
    }
}