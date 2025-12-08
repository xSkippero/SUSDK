using System;
using SUSDK.Domain.Interfaces;

namespace SUSDK.Domain
{
  [Serializable]
  public class Weapon : IWeapon, ICloneable
  {
    public string Name { get; set; }
    public float Grade { get; set; }
    public DamageType DamageType { get; set; }
    public float ProjectileDamage { get; set; }
    public float ProjectileSpeed { get; set; }
    public float ProjectileSize { get; set; }
    public float ProjectileAmount { get; set; }
    public float ProjectileSpread { get; set; }
    public string WeaponEmptySound { get; set; }
    public float FireRpm { get; set; }
    public float ReloadCooldown { get; set; }
    public float HeatPerShot { get; set; }
    public string FireSound { get; set; }
    public string ReloadSound { get; set; }
    public string OverheatSound { get; set; }
    public bool ConsumesAmmo { get; set; }
    public float AmmoPerShot { get; set; }
    public float MaxAmmo { get; set; }
    public float CurrentAmmo { get; set; }
    public float LoadedAmmo { get; set; }
    public float MaxHeat { get; set; }
    public float CurrentHeat { get; set; }
    public float HeatReductionPerSecond { get; set; }
    
    public object Clone()
    {
      return MemberwiseClone();
    }
  }
}