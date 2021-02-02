using UnityEngine;
using UnityEngine.InputSystem;
using Utils;

public class Champion : WalkableUnit
{
    public Camera playerCamera;
    
    public float gold;
    
    protected override void Start()
    {
        base.Start();
        Statistics.AttackDamage = 62f;
    }

    public override void Action(GameObject actor)
    {
        base.Action(actor);

        attackTarget = null;

        //Obtendo o raio apontando da posição da câmera até o ponto da posição do clique do mouse
        var ray = playerCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

        //Verificando se o raio acerta algum GameObject
        if (!Physics.Raycast(ray, out var hit)) return;

        var hitGameObject = hit.collider.gameObject;

        if (!hitGameObject) return;

        if (hitGameObject.CompareTag(Tags.Minion.Value))
        {
            hitGameObject.GetComponent<Minion>().Action(gameObject);
        }
        else if (hitGameObject.CompareTag(Tags.Map.Value))
        {
            StopMoving();
            Move(hit.point);
        }
    }
    
    //Base class methods
    protected override void OnDie(Unit actor)
    {
        //TODO: What should happen when it dies
    }
    
    //Class methods
    public void StopAll()
    {
        StopMoving();
        attackTarget = null;
    }
}