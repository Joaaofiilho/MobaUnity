using UnityEngine;
using UnityEngine.InputSystem;
using Utils;

[RequireComponent(typeof(Champion))]
public class Player : MonoBehaviour
{
    [SerializeField] private LayerMask whatIsObstacle;
    
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

        //Verificando se o raio acerta algum GameObject
        //O "~" antes da LayerMask flipa todos os binários, então todas as camadas vão ser selecionadas, exceto a
        //de obstáculos.
        if (Physics.Raycast(ray, out var hit, 100f, ~whatIsObstacle))
        {
            var hitGameObject = hit.collider.gameObject;

            if (!hitGameObject) return;
            
            champion.OnDoAction(hitGameObject, hit.point);
        }
    }

    public void OnStopAllInput(InputAction.CallbackContext context)
    {
        champion.StopAll();
    }
}