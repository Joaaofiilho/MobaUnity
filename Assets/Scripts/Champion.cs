using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Utils;

public class Champion : WalkableUnit
{
    public Camera playerCamera;
    
    private float _gold;
    
    //Callbacks
    public event Action<float> OnGoldChanged = delegate(float gold) {  };
    
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

    
    /// <summary>
    /// Increases the champion's gold amount.
    /// <para>The gold amount must be greater than 0.</para>
    /// </summary>
    /// <param name="amount">The amount of gold to add to the champion.</param>
    public void AddGold(float amount)
    {
        _gold += amount;
        OnGoldChanged(_gold);
    }
}