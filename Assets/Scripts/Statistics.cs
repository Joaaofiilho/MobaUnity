using System;

[Serializable]
public class Statistics
{
    public float AttackDamage { get; set; }
    public float BasicAttackRange = 6f;
    public float AttackSpeed = 1f;
    
    public float SpellDamage { get; set; }

    public float Armor { get; set; }
    public float MagicResistance { get; set; }
}