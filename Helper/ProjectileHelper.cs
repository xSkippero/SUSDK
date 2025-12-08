using System;
using SUSDK.Domain;
using SUSDK.Manager;
using UnityEngine;

namespace SUSDK.Helper
{
  public static class ProjectileHelper
  {
    public static float CalculateProjectileDamage(
      DamageType damageType,
      float projectileDamage,
      float againstShieldAmount = 0f,
      bool againstHeavyArmor = false,
      bool againstEnergyShield = false,
      bool isTargetFrozen = false)
    {
      if (againstShieldAmount <= 0)
        return projectileDamage;

      var multiplier = 1f;

      if (againstHeavyArmor)
      {
        multiplier = damageType switch
        {
          DamageType.Fusion => 2f,       
          DamageType.Antimatter => 1.5f,
          DamageType.Glacio => 1.4f,
          DamageType.Physical => 1f,
          DamageType.Plasma => 0.7f,
          DamageType.Energy => 0.5f,
          _ => 1f
        };
      }
      else if (againstEnergyShield)
      {
        multiplier = damageType switch
        {
          DamageType.Plasma => 1.5f,
          DamageType.Fusion => 0.5f,
          DamageType.Energy => 1.75f,
          DamageType.Glacio => 0.5f,
          DamageType.Antimatter => 1.25f,
          DamageType.Physical => 1f,
          _ => 1f
        };
      }
      
      if (isTargetFrozen)
        multiplier += 0.25f;

      return Mathf.RoundToInt(projectileDamage * multiplier);
    }

    
    public static GameObject GetProjectileType(WeaponController weapon)
    {
      return weapon.weaponInfo.DamageType switch
      {
        DamageType.Physical => ProjectileManager.GetProjectile("Physical"),
        DamageType.Energy => ProjectileManager.GetProjectile("Energy"),
        DamageType.Glacio => ProjectileManager.GetProjectile("Glacio"),
        DamageType.Fusion => ProjectileManager.GetProjectile("Fusion"),
        DamageType.Plasma => ProjectileManager.GetProjectile("Plasma"),
        DamageType.Antimatter => ProjectileManager.GetProjectile("Antimatter"),
        DamageType.Unknown => new GameObject(),
        _ => throw new ArgumentOutOfRangeException()
      };
    }
  }
}