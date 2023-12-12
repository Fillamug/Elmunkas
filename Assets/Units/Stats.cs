using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CombatHelper;

[CreateAssetMenu(menuName = "Unit/Stats")]
public class Stats : ScriptableObject
{
    public string unitName;
    public int maxHealth;
    public int attackDamage;
    public float attackSpeed;
    public int attackRange;
    public AttackType attackType;
    public ArmorType armorType;
    public Texture2D unitIcon;
}
