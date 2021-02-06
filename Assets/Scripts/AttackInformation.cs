public sealed class AttackInformation
{
    public AttackInformation(float damageAmount, DamageType damageType)
    {
        DamageAmount = damageAmount;
        DamageType = damageType;
    }
    
    public float DamageAmount { get; }
    public DamageType DamageType { get; }
}