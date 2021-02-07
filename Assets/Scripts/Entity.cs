using System;
using JetBrains.Annotations;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public abstract class Entity : MonoBehaviour
{
    [SerializeField] public Teams team = Teams.Blue;
    
    /// <summary>
    /// This variable is used to know when team variable has been changed on the Inspector.
    /// </summary>
    private Teams _previousTeam = Teams.Blue;

    private MeshRenderer _meshRenderer;

    [SerializeField] private float maxHealth;
    [SerializeField] private float currentHealth;
    
    //Unity methods
    protected virtual void Awake()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
        ConfigureTeams();
    }

    protected virtual void Update()
    {
        if (_previousTeam != team)
        {
            _previousTeam = team;
            ConfigureTeams();
        }
    }

    //Callbacks
    
    
    public event Action<Entity, float, float> OnGetHealdedCallback = delegate(Entity actor, float amount, float totalAmount) { };
    
    /// <summary>
    /// Called everytime this entity is healed.
    /// </summary>
    /// <param name="actor">Who healed this entity.</param>
    /// <param name="amount">The real amount the entity was healed after calculating Grievous Wounds.</param>
    /// <param name="totalAmount">The total amount the entity was healed before calculating Grievous Wounds.</param>
    protected virtual void OnGetHealed([CanBeNull] Entity actor, float amount, float totalAmount)
    {
        OnGetHealdedCallback(actor, amount, totalAmount);
    }
    
    public event Action<Unit, Entity> OnDieCallback = delegate(Unit actor, Entity target) { };
    
    /// <summary>
    /// Called everytime this entity dies, but before Destroy gets called.
    /// </summary>
    /// <param name="actor">Who killed the entity. This can be null</param>
    /// <param name="target"></param>
    protected virtual void OnDie([CanBeNull] Unit actor, [NotNull] Entity target)
    {
        OnDieCallback(actor, target);
    }
    
    public event Action<Unit, Entity> OnAfterDieCallback = delegate(Unit actor, Entity target) { };

    protected virtual void OnAfterDie([CanBeNull] Unit actor, Entity target)
    {
        OnAfterDieCallback(actor, target);
        Destroy(gameObject);
    }

    public event Action<Unit, Entity, AttackInformation[], AttackInformation[]> OnTakeDamageCallback =
        delegate(Unit actor, Entity target, AttackInformation[] originalAttackInformations, AttackInformation[] realAttackInformations) { };

    /// <summary>
    /// Called whenever this unit takes any damage.
    /// </summary>
    /// <param name="actor">Who did the damage.</param>
    /// <param name="target">Who took the damage.</param>
    /// <param name="originalAttackInformations">The attack information before the armor and magic resistance calculations.</param>
    /// <param name="realAttackInformations">The attack information after the armor and magic resistance calculations. Empty if no damage.</param>
    protected virtual void OnTakeDamage([CanBeNull] Unit actor, [NotNull] Entity target, AttackInformation[] originalAttackInformations, AttackInformation[] realAttackInformations)
    {
        OnTakeDamageCallback(actor, target, originalAttackInformations, realAttackInformations);
        
        if (realAttackInformations.Length > 0)
        {
            DamagePopup.Create(transform, realAttackInformations);
        }
    }

    public event Action<float, float> OnHealthChangedCallback = delegate(float currentHealth, float maxHealth) { };

    protected virtual void OnHealthChanged(float currentHealth, float maxHealth)
    {
        OnHealthChangedCallback(currentHealth, maxHealth);
    }

    //Class methods
    public void TakeDamage([CanBeNull] Unit actor, AttackInformation[] attackInformations)
    {
        //The real damage received, after applying armor and magic resistance calculation.
        var realDamageInformation = new AttackInformation[attackInformations.Length];

        var previousHealth = currentHealth;
        
        for (var i = 0; i < attackInformations.Length; i++)
        {
            //TODO: calculate damage based on damageType, applying armor and magic resistance calculations. Remember: an attack damage can be already 0.
            currentHealth -= attackInformations[i].DamageAmount;

            //TODO: For now, the damage dealt will be 100%.
            realDamageInformation[i] = attackInformations[i];
        }

        OnTakeDamage(actor, this, attackInformations, realDamageInformation);

        if (previousHealth != currentHealth)
        {
            OnHealthChanged(currentHealth, maxHealth);
        }

        if (currentHealth <= 0)
        {
            OnDie(actor, this);
            OnAfterDie(actor, this);
        }
    }

    public void GetHeal(Unit actor, float amount)
    {
        var previousHealth = currentHealth;

        if (currentHealth + amount >= maxHealth)
        {
            currentHealth = maxHealth;
            OnHealthChanged(currentHealth, maxHealth);
            OnGetHealed(actor, maxHealth - previousHealth, amount);
        }
        else
        {
            currentHealth += amount;
            OnHealthChanged(currentHealth, maxHealth);
            OnGetHealed(actor, amount, amount);
        }
    }
    
    private void ConfigureTeams()
    {
        if (!_meshRenderer)
        {
            _meshRenderer = GetComponent<MeshRenderer>();
        }
        
        ApplyTeamColor(_meshRenderer);
        
        var childrenMeshRenderers = GetComponentsInChildren<MeshRenderer>();

        foreach (var childrenMeshRenderer in childrenMeshRenderers)
        {
            ApplyTeamColor(childrenMeshRenderer);
        }
    }

    private void ApplyTeamColor(MeshRenderer meshRenderer)
    {
        switch (team)
        {
            case Teams.Blue:
                meshRenderer.material = GameAssets.i.blueTeamMaterial;
                break;
            case Teams.Red:
                meshRenderer.material = GameAssets.i.redTeamMaterial;
                break;
        }
    }
}
