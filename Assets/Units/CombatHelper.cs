using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CombatHelper
{
    public enum AttackType
    {
        MachineGun,
        ArmorPiercing,
        AntiAir,
        HighExplosive
    }

    public enum ArmorType
    {
        HeavyArmor,
        LightArmor,
        Unarmored,
        Fortified
    }

    public enum Team {
        RakosiMatyas,
        KadarJanos,
        NagyImre,
        KunBela
    }

    public static Matrix4x4 damageMatrix = new Matrix4x4(new Vector4(0.5f, 1.5f, 0.75f, 1),
                                                         new Vector4(0.75f, 1.25f, 1.5f, 1),
                                                         new Vector4(1, 1, 1.25f, 2),
                                                         new Vector4(0.5f, 1, 0.75f, 1.5f));

    public static float MultiplyVector4ByVector4(Vector4 left, Vector4 right)
    {
        return left.x * right.x + left.y * right.y + left.z * right.z + left.w * right.w;
    }

    public static Vector4 AttackTypeVector(AttackType attackType)
    {
        return attackType switch
        {
            AttackType.MachineGun => new Vector4(1, 0, 0, 0),
            AttackType.ArmorPiercing => new Vector4(0, 1, 0, 0),
            AttackType.AntiAir => new Vector4(0, 0, 1, 0),
            AttackType.HighExplosive => new Vector4(0, 0, 0, 1),
            _ => new Vector4(0, 0, 0, 0),
        };
    }

    public static Vector4 ArmorTypeVector(ArmorType armorType)
    {
        return armorType switch
        {
            ArmorType.HeavyArmor => new Vector4(1, 0, 0, 0),
            ArmorType.LightArmor => new Vector4(0, 1, 0, 0),
            ArmorType.Unarmored => new Vector4(0, 0, 1, 0),
            ArmorType.Fortified => new Vector4(0, 0, 0, 1),
            _ => new Vector4(0, 0, 0, 0),
        };
    }
}